using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using FiveMinuteChat.Connectors;
using FiveMinuteChat.Enums;
using FiveMinuteChat.Model;
using FiveMinuteChat.Helpers;
using FiveMinuteChat.Interfaces;
using Shared.Model.Messages.Client;
using Shared.Model;
using Shared.Model.Enums;
using Shared.Model.Messages;
using Shared.Model.Messages.Server;
using UnityEngine;
using UnityEngine.Networking;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FiveMinuteChat
{
    public partial class ChatConnectionBehavior : MonoBehaviour
    {
        private class AcceptAllCertificateHandler : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData) => true;
        }
        
        public event OnChannelJoined ChannelJoined;
        public event OnChannelLeft ChannelLeft;
        public event OnConnectionAccepted ConnectionAccepted;
        public event OnChannelInfo ChannelInfoReceived;

        private const string DiscoveryUrl = "https://api.fiveminutes.io/discover";

        public BackendInfos AvailableBackends = new BackendInfos();
        
        [SerializeField] 
        public string SelectedServerName;
        [SerializeField] 
        public string ApplicationName;
        [SerializeField] 
        public string ApplicationSecret;
        [SerializeField] 
        public ConnectorType PreferredTransport = ConnectorType.SignalRCore;
        [SerializeField] 
        public string UserId;
        [SerializeField] 
        public bool AutoConnect;
        [SerializeField] 
        public bool DontDestroyOnLoad = true;
        [SerializeField] 
        public LogLevel LogLevel = LogLevel.Debug;

        public static string OwnDisplayId { get; private set; }
        
        private IConnector _connector;

        private Regex _commandRegex = new Regex("/(?:(?:(join|leave|create-channel|whisper|whois)) ([A-z0-9\\-]+)?(:? (.*))?)|/(whoami|create-channel|channel-info)");
        private Regex _changeNameRegex = new Regex(@"/nick ([\p{L}\-\d]{3,15}$)");
        private Regex _displayIdRegex = new Regex("[A-z0-9]{6}[0-9]{4}");

        private readonly Dictionary<string, UserInfo> _knownUsers = new Dictionary<string, UserInfo>();

        private void Awake()
        {
            if( DontDestroyOnLoad )
            {
                DontDestroyOnLoad( this );
            }
            Application.runInBackground = true;

            Telepathy.Logger.Log = Logger.Log;
            Telepathy.Logger.LogWarning = Logger.LogWarning;
            Telepathy.Logger.LogError = Logger.LogError;
            Logger.CurrentLogLevel = LogLevel;
            
#if UNITY_EDITOR
            if( Application.isEditor 
                && !Application.isPlaying &&
                string.IsNullOrEmpty(ApplicationName) && 
                string.IsNullOrEmpty(ApplicationSecret) &&
                EditorPrefs.HasKey($"FiveMinuteChat:{nameof(ApplicationName)}") &&
                EditorPrefs.HasKey($"FiveMinuteChat:{nameof(ApplicationSecret)}"))
            {
                ApplicationName = EditorPrefs.GetString(nameof(ApplicationName));
                ApplicationSecret = EditorPrefs.GetString(nameof(ApplicationSecret));
            }
#endif

            if( (int)PreferredTransport > Enum.GetValues( typeof(ConnectorType) ).Cast<int>().Max() )
            {
                PreferredTransport = ConnectorType.Tcp;
            }

            switch( PreferredTransport )
            {
                case ConnectorType.Tcp:
                    _connector = gameObject.AddComponent<TcpConnector>();
                    break;
                case ConnectorType.SignalRCore:
                    _connector = gameObject.AddComponent<SignalRCoreConnector>();
                    break;
#if FiveMinuteChat_BestHttpEnabled
                case ConnectorType.SignalRBestHttp2:
                    _connector = gameObject.AddComponent<BestHttpSignalRConnector>();
                    break;
#endif
                default:
                    throw new ArgumentOutOfRangeException($"Unknown value of parameter {nameof(PreferredTransport)}: {PreferredTransport}");
            }
        }

        private void Start()
        {
            var userId =
                string.IsNullOrWhiteSpace(UserId) ? 
                    SystemInfo.deviceUniqueIdentifier :
                    UserId;

            _connector.SetCredentials(ApplicationName, ApplicationSecret, userId); 
            _connector.ConnectionAccepted += message => ConnectionAccepted?.Invoke(message);
            _connector.ChannelJoined += OnChannelJoined;
            _connector.ChannelLeft += OnChannelLeft;
            _connector.ChannelInfoReceived += OnChannelInfo;

            Subscribe( ( ServerWelcome welcome ) =>
            {
                OwnDisplayId = welcome.DisplayId;
            } );
            Subscribe( ( ServerSetUsernameResponse setUsernameResponse ) =>
            {
                OwnDisplayId = setUsernameResponse.DisplayId;
            } );
            Subscribe( ( ServerWhisperMessage whisperMessage ) =>
            {
                if( whisperMessage.IsNew && 
                    whisperMessage.FromUser?.UserType == UserType.Standard )
                {
                    _connector.Send( new ClientWhisperMessageReceivedRequest()
                    {
                        ReceivedMessageId = whisperMessage.MessageId
                    } );
                }
            } );
            
            if( AutoConnect )
            {
                Connect();
            }
        }

        private void OnChannelJoined(ChannelInfo channelInfo, bool canLeave, bool isSilenced )
        {
            ChannelJoined?.Invoke(channelInfo, canLeave, isSilenced);
            foreach( var member in channelInfo.Members )
            {
                if( !_knownUsers.ContainsKey( member.DisplayId ) )
                {
                    _knownUsers.Add(member.DisplayId, member);
                }
                else
                {
                    _knownUsers[member.DisplayId] = member;
                }
            }
        }
        
        private void OnChannelLeft( string channelName )
        {
            ChannelLeft?.Invoke( channelName );
        }

        private void OnChannelInfo( ChannelInfo channelInfo )
        {
            ChannelInfoReceived?.Invoke( channelInfo);
        }

        public Guid Subscribe<T>( Action<T> callback ) where T : Shared.Model.Messages.MessageBase
            => _connector.Subscribe( callback );

        public void Unsubscribe( Guid callbackId )
            => _connector?.Unsubscribe( callbackId );

        public async void Connect() 
            => await Retryer.RetryUntilAsync( () => RunServerDiscovery(), 500, 4 );

        public void Disconnect() 
            => _connector?.Disconnect( false );
        
        private async Task<bool> RunServerDiscovery()
        {
            Logger.Log("FiveMinuteChat: Discovering servers...");
            if (await DiscoverServers())
            {
                switch( PreferredTransport )
                {
                    case ConnectorType.Tcp:
                        SelectedServerName = AvailableBackends.Backends.Single( s => s.Name == "europe-tcp" ).Name;
                        break;
                    case ConnectorType.SignalRCore:
#if FiveMinuteChat_BestHttpEnabled
                    case ConnectorType.SignalRBestHttp2:
#endif
                        SelectedServerName = AvailableBackends.Backends.Single( s => s.Name == "europe-signalr" ).Name;
                         break;
                    default:
                        throw new ArgumentOutOfRangeException($"Unknown value of parameter {nameof(PreferredTransport)}: {PreferredTransport}");
                }
                
                ConnectToSelectedServer();
                return true;
            }

            Logger.Log("FiveMinuteChat: Servers discovery failed!");
            return false;
        }

        private void ConnectToSelectedServer()
        {
            var backend = AvailableBackends.Backends.SingleOrDefault(b => b.Name == SelectedServerName);
            if (backend == default)
            {
                Logger.LogError($"FiveMinuteChat: List of available servers does not contain the selected server {SelectedServerName}!");
                return;
            }

            _connector.Connect(backend.Endpoint, backend.Port);
        }
        
        public async Task<bool> DiscoverServers()
        {
            using (var req = UnityWebRequest.Get( DiscoveryUrl ))
            {
                req.certificateHandler = new AcceptAllCertificateHandler();
                var operation = req.SendWebRequest();
                while (!operation.isDone)
                {
                    Logger.Log("FiveMinuteChat: Waiting for server discovery response...");
                    await Task.Delay(500);
                }
                Logger.Log("FiveMinuteChat: Got server discovery response!");
                return ServerDiscoveryResult( req );
            }
        }

        private bool ServerDiscoveryResult(UnityWebRequest req)
        {
            if (req.result == UnityWebRequest.Result.Success)
            {
                AvailableBackends = JsonUtility.FromJson<BackendInfos>(req.downloadHandler.text);
                return true;
            }
            Logger.LogWarning($"FiveMinuteChat: Unable to discover remote servers: {req.error}");
            return false;
        }

        public void Send(string message, string parameter)
        {
            if( string.IsNullOrWhiteSpace( message ) )
            {
                Logger.Log( $"FiveMinuteChat: {nameof(ChatConnectionBehavior)}.{nameof(Send)} - Empty messages are not allowed, ignoring..." );
                return;
            }

            var payload = default(ClientMessageBase);
            var match = _commandRegex.Match(message);
            if(match.Success)
            {
                var switchVal = string.IsNullOrWhiteSpace( match.Groups[1].Value )
                    ? match.Groups[5].Value
                    : match.Groups[1].Value;
                switch (switchVal)
                {
                    case "join":
                        JoinChannel( match.Groups[2].Value );
                        break;
                    case "leave":
                        LeaveChannel( match.Groups[2].Value );
                        break;
                    case "create-channel":
                        CreateChannel( match.Groups[2].Value );
                        break;
                    case "whisper":
                        Whisper( match.Groups[2].Value, match.Groups[3].Value );
                        break;
                    case "nick":
                        return;
                    case "whois":
                        Whois( match.Groups[2].Value );
                        break;
                    case "whoami":
                        Whois( null );
                        break;
                    case "channel-info":
                        GetChannelInfo(parameter);
                        break;
                    case "send-report":
                        GetChannelInfo(parameter);
                        break;
                    default:
                        Logger.LogWarning($"FiveMinuteChat: Unknown command {match.Groups[1].Value}");
                        return;
                }
            }
            else if( (match = _changeNameRegex.Match( message ) ).Success )
            {
                SetUsername( match.Groups[1].Value );
            }
            else
            {
                SendChatMessage(parameter, message);
            }

            
        }
        
        void OnApplicationQuit()
        {
            // the client/server threads won't receive the OnQuit info if we are
            // running them in the Editor. they would only quit when we press Play
            // again later. this is fine, but let's shut them down here for consistency
            _connector?.Disconnect( false);
        }
    }
}
