using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using FiveMinuteChat.Helpers;
using FiveMinuteChat.Interfaces;
using Shared.Helpers;
using Shared.Model;
using Shared.Model.Messages;
using Shared.Model.Messages.Client;
using Shared.Model.Messages.Server;
using FiveMinuteChat.Telepathy;
using UnityEngine;
using Timer = System.Timers.Timer;

namespace FiveMinuteChat.Connectors
{
    public class TcpConnector : ConnectorBase
    {
        public override bool Connected => _client.Connected;

        private Timer _heartbeatTimer;
        private string _backendEndpoint;
        private int _backendPort;        
        private string _username;
        private bool _shouldReconnect = true;
        private TcpMessageHandler _tcpMessageHandler => _messageHandler as TcpMessageHandler; 
        private readonly ClientContainer _client = new ClientContainer();
        
        private class ClientContainer : IConnectorClient
        {
            public readonly Client Client = new Client();
            public bool Connected => Client.Connected;

            public void Connect( string backendEndpoint, int backendPort )
                => Client.Connect( backendEndpoint, backendPort );
            
            public void Disconnect() 
                => Client.Disconnect();

            public bool Send( MessageBase message, bool shouldEncrypt = true)
                => Client.Send( Serializer.Serialize(message), shouldEncrypt );
            
            public bool Send( byte[] data, bool shouldEncrypt = true )
                => Client.Send( data, shouldEncrypt );
        }

        private void Awake()
        {
            Init( _client, new TcpMessageHandler( this ));
        }

        public override void OnConnectionAccepted( List<ChannelInfo> welcomeAvailableChannels )
        {
            base.OnConnectionAccepted( welcomeAvailableChannels );
            _heartbeatTimer = new Timer(5000);
            _heartbeatTimer.AutoReset = true;
            _heartbeatTimer.Elapsed += ( sender, args ) => Send( new HeartbeatMessage() );
            _heartbeatTimer.Start();
        }
        
        public override void Connect(string backendEndpoint, int backendPort)
        {
            Logger.Log($"FiveMinuteChat: Connecting to TCP endpoint {backendEndpoint} on port {backendPort}");
            _backendEndpoint = backendEndpoint;
            _backendPort = backendPort;
            _client.Connect(backendEndpoint, backendPort);
        }

        public override void SetUsername( string username )
        {
            if( string.IsNullOrWhiteSpace( username ) )
            {
                throw new ArgumentException( $"FiveMinuteChat: Username '{username}' is invalid. It cannot be empty.");
            }

            _tcpMessageHandler.SetUsername( username );
            if( _client.Connected )
            {
                Send( new ClientSetUsernameRequest{ Username = username } );
            }
        }

        public override void Send(ClientMessageBase message, bool shouldEncrypt = true )
        {
            try
            {
                // Logger.Log($"Sending {message.GetType().Name} - Encryption: {shouldEncrypt}");
                if( !_client.Send( Serializer.Serialize( message ), shouldEncrypt ) )
                {
                    Logger.LogWarning("FiveMinuteChat: Disconnected from server!\nReconnecting...");
                    Reconnect();
                }
                if( message.IsAckRequested )
                {
                    AddToAckQueue( message );
                }
            }
            catch (Exception e)
            {
                Logger.LogError($"FiveMinuteChat: Caught exception: {e.Message}\nReconnecting...");
                Reconnect();
            }
        }

        public override void Disconnect( bool allowReconnect )
        {
            _shouldReconnect &= allowReconnect;
            _heartbeatTimer?.Dispose();
            _client?.Disconnect();
        }

        public override async void Reconnect()
        {
            Disconnect( true );
            await Task.Delay( 2500 );
            Connect( _backendEndpoint, _backendPort );
        }

        private void OnApplicationQuit()
        {
            _heartbeatTimer?.Dispose();
            _shouldReconnect = false;
        }

        private void OnDisable()
        {
            _heartbeatTimer?.Dispose();
            _shouldReconnect = false;
        }

        private void OnDestroy()
        {
            _heartbeatTimer?.Dispose();
            _shouldReconnect = false;
        }

        private void Update()
        {
            while (_client.Client.GetNextMessage(out var msg))
            {
                switch (msg.eventType)
                {
                    case Telepathy.EventType.Connected:
                        Logger.Log("FiveMinuteChat: Connected to server");
                        break;
                    case Telepathy.EventType.Data:
                        _tcpMessageHandler.Handle(_client, msg);
                        break;
                    case Telepathy.EventType.Disconnected:
                        if( _shouldReconnect )
                        {
                            Logger.Log("FiveMinuteChat: Disconnected from server. Reconnecting...");
                            Reconnect();
                        }
                        else
                        {
                            Logger.Log("FiveMinuteChat: Disconnected from server. Shutting down...");
                            _heartbeatTimer?.Dispose();
                        }
                        
                        break;
                }
            }
        }
    }
}
