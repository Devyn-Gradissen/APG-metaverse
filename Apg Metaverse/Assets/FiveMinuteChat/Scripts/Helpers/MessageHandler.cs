using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FiveMinuteChat.Connectors;
using FiveMinuteChat.Interfaces;
using Shared.Extensions;
using Shared.Helpers;
using Shared.Model;
using Shared.Model.Enums;
using Shared.Model.Messages;
using Shared.Model.Messages.Client;
using Shared.Model.Messages.Server;
using UnityEditor;
using UnityEngine;

namespace FiveMinuteChat.Helpers
{
    public class MessageHandler
    {
        private readonly IConnector _connector;
        private readonly Dictionary<Type, Action<IConnectorClient, MessageBase>> _handlers = new Dictionary<Type, Action<IConnectorClient, MessageBase>>();
        private string _applicationName;
        private string _applicationSecret;
        private string _uniqueUserId;
        private string _username;

        public MessageHandler( IConnector connector )
        {
            _connector = connector;
            if( !_handlers.Any() )
            {
                _handlers.Add( typeof(ServerHello), (client, message) => OnServerHello(client, message as ServerHello) );
                _handlers.Add( typeof(ServerAck), (client, message) => OnServerAck(client, message as ServerAck) );
                _handlers.Add( typeof(ServerCredentialsRequest), OnServerCredentialsRequest );
                _handlers.Add( typeof(ServerUserInfoRequest), OnServerUserInfoRequest );
                _handlers.Add( typeof(ServerUserInfoResponse), (client, message) => OnServerUserInfoResponse(client, message as ServerUserInfoResponse) );
                _handlers.Add( typeof(ServerWelcome), (client, message) => OnServerWelcome(client, message as ServerWelcome) );
                _handlers.Add( typeof(ServerSetUsernameResponse), (client, message) => OnServerSetUsernameResponse(client, message as ServerSetUsernameResponse) );
                _handlers.Add( typeof(ServerGoodbye), (client, message) => OnServerGoodbye(client, message as ServerGoodbye) );
                _handlers.Add( typeof(ServerChatMessage), ( _, __ ) => { } );
                _handlers.Add( typeof(ServerJoinChannelResponse), (client, message) => OnServerJoinChannelResponse(client, message as ServerJoinChannelResponse) );
                _handlers.Add( typeof(ServerLeaveChannelResponse), (client, message) => OnServerLeaveChannelResponse(client, message as ServerLeaveChannelResponse) );
                _handlers.Add( typeof(ServerGenerateChannelResponse), (client, message) => OnServerGenerateChannelResponse(client, message as ServerGenerateChannelResponse) );
                _handlers.Add( typeof(ServerGenerateNamedChannelResponse), (client, message) => OnServerGenerateNamedChannelResponse(client, message as ServerGenerateNamedChannelResponse) );
                _handlers.Add( typeof(ServerChannelHistoryResponse), (client, message) => OnServerChannelHistoryResponse(client, message as ServerChannelHistoryResponse) );
                _handlers.Add( typeof(ServerChannelInfoResponse), (client, message) => OnServerChannelInfoResponse(client, message as ServerChannelInfoResponse) );
                _handlers.Add( typeof(ServerUserSilenceInfoMessage), ( _, __ ) => { } );
                _handlers.Add( typeof(ServerUserBanInfoMessage), (client, message) => OnServerUserBanInfoMessage(client, message as ServerUserBanInfoMessage) );
                _handlers.Add( typeof(ServerUserKickInfoMessage), (client, message) => OnServerUserKickInfoMessage(client, message as ServerUserKickInfoMessage) );
                _handlers.Add( typeof(ServerWhisperMessage), ( _, __ ) => { } );
                _handlers.Add( typeof(ServerWhisperHistoryResponse), ( client, message ) => OnServerWhisperHistoryResponseMessage( client, message as ServerWhisperHistoryResponse) );
            }
        }

        public void SetCredentials( string applicationName, string applicationSecret, string uniqueUserId )
        {
            Logger.Log($"FiveMinuteChat: Set credentials appName={applicationName}, secret={applicationSecret}, userid={uniqueUserId}");
            _applicationName = applicationName;
            _applicationSecret = applicationSecret;
            _uniqueUserId = uniqueUserId;
        }
        
        public void SetUsername( string username )
        {
            _username = username;
        }
        
        public void Handle(IConnectorClient client, MessageBase message )
        {
            var messageType = message.GetType();
            try
            {
                _connector.On( message );
                if (_handlers.ContainsKey(messageType))
                {
                    _handlers[messageType].Invoke(client, message);
                }
                else
                {
                    Logger.LogWarning($"Message of type {messageType} has no associated handler.");
                }
            }
            catch( Exception e )
            {
                Logger.LogError($"Unhandled exception for message of type {messageType.Name}: {e.InnerMostMessage()}\n{e.StackTrace}");
            }
        }

        private void OnServerHello( IConnectorClient client, ServerHello hello )
        {
            Logger.Log("FiveMinuteChat: Initializing transport encryption...");
            var response = new ClientEncryptedSymmetricKey
            {
                EncryptedSymmetricKey = Crypto.Client.EncryptAsymmetric(Crypto.Client.GetSymmetricKey(), hello.PublicKeyXml),
                EncryptedSymmetricIV = Crypto.Client.EncryptAsymmetric(Crypto.Client.GetSymmetricIV(), hello.PublicKeyXml),
                IsAckRequested = false
            };

            _connector.Send(response, false );
        }

        private void OnServerAck( IConnectorClient client, ServerAck message )
        {
            _connector.AckMessage(message.MessageId);
        }

        private void OnServerCredentialsRequest( IConnectorClient client, MessageBase request )
        {
            Logger.Log("FiveMinuteChat: Encrypted channel established. Supplying server with credentials....");
            var response = new ClientCredentialsResponse
            {
                ApplicationId = _applicationName,
                ApplicationSecret = _applicationSecret,
                IsAckRequested = false
            };
            _connector.Send(response);
        }
        
        private void OnServerUserInfoRequest( IConnectorClient client, MessageBase message )
        {
            Logger.Log("FiveMinuteChat: Supplying server with user info....");
            var response = new ClientUserInfoResponse
            {
                UniqueUserId = _uniqueUserId,
                Username = string.IsNullOrWhiteSpace(_username) ? string.Empty : _username,
                IsAckRequested = false
            };
            _connector.Send(response);
        }
        
        private void OnServerUserInfoResponse( IConnectorClient client, ServerUserInfoResponse message )
        {
            // fake whisper
            var wm = new ServerWhisperMessage
            {
                Content = $"Your userInfo:\nUsername: {message.Username}\nDisplayId: {message.UserDisplayId}",
                FromUser = new UserInfo
                {
                    Name = "SYSTEM",
                    DisplayId = "SYSTEM",
                    UserType = UserType.System
                },
                SentAt = DateTime.UtcNow,
                ToUser = new UserInfo
                {
                    Name = message.Username,
                    DisplayId = message.UserDisplayId
                }
            };
            _connector.On( wm );
        }
        
        private void OnServerWelcome( IConnectorClient client, ServerWelcome welcome )
        {
            Logger.Log("FiveMinuteChat: Server accepted connection!");
            if (welcome.AvailableChannels.Any())
            {
                var alreadyJoinedChannels = welcome.AvailableChannels
                    .Where(c => c.IsMember)
                    .ToList();
                var defaultChannels = welcome.AvailableChannels
                    .Where(c => c.IsDefault)
                    .ToList();
                if (alreadyJoinedChannels.Any())
                {
                    Logger.Log($"FiveMinuteChat: Previously joined channels {alreadyJoinedChannels.Select(c => c.Name).Aggregate( (f,s) => $"{f}, {s}" )}");
                    foreach (var alreadyJoinedChannel in alreadyJoinedChannels)
                    {
                        _connector.OnChannelJoined( alreadyJoinedChannel, !alreadyJoinedChannel.IsDefault, alreadyJoinedChannel.IsSilenced );
                        var historyRequest = new ClientChannelHistoryRequest
                        {
                            ChannelName = alreadyJoinedChannel.Name,
                            MaxMessages = 25
                        };
                        _connector.Send( historyRequest );
                    }
                }
                else if( defaultChannels.Any() ) // assume defaultChannels is a subset of alreadyJoinedChannels
                {
                    Logger.Log($"FiveMinuteChat: Joining default channel(s) {defaultChannels.Select(c => c.Name).Aggregate( (f,s) => $"{f}, {s}" )}");
                    foreach (var defaultChannel in defaultChannels)
                    {
                        var joinChannelRequest = new ClientJoinChannelRequest
                        {
                            ChannelName = defaultChannel.Name,
                        };
                        _connector.Send(joinChannelRequest);
                    }
                }

                var remainingChannels = welcome.AvailableChannels.Except(alreadyJoinedChannels).Except(defaultChannels);
                if (remainingChannels.Any())
                {
                    Logger.Log($"FiveMinuteChat: Remaining available channels are {remainingChannels.Select(c => c.Name).Aggregate( (f,s) => $"{f}, {s}" )}");
                }

                var whisperHistoryRequest = new ClientWhisperHistoryRequest()
                {
                    UniqueUserId = _uniqueUserId
                };
                _connector.Send( whisperHistoryRequest );
                
                _connector.OnConnectionAccepted( welcome.AvailableChannels);
            }
            else
            {
                Logger.LogWarning($"FiveMinuteChat: There are no available channels! Disconnecting...");
                _connector.Disconnect( false );
            }
        }
        
        private void OnServerSetUsernameResponse( IConnectorClient client, ServerSetUsernameResponse setUsernameResponse )
        {
            if( setUsernameResponse.Success )
            {
                Logger.Log($"FiveMinuteChat: Username successfully changed to {setUsernameResponse.Username}");
            }
            else
            {
                Logger.LogWarning($"FiveMinuteChat: Username change failed: {setUsernameResponse.Reason}");
            }
        }
        
        private void OnServerGoodbye( IConnectorClient client, ServerGoodbye goodbye )
        {
            Logger.LogWarning($"FiveMinuteChat: Server closing connection: {goodbye.Reason}");
            if( goodbye.AllowAutoReconnect && 
                Application.isPlaying )
            {
                _connector.Reconnect();
            }
            else
            {
                _connector.Disconnect( false );
            }
        }
        
        private void OnServerJoinChannelResponse( IConnectorClient client, ServerJoinChannelResponse joinChannelResponse )
        {
            if (joinChannelResponse.Success)
            {
                Logger.Log($"FiveMinuteChat: Successfully joined channel: {joinChannelResponse.ChannelInfo.Name}");
                _connector.OnChannelJoined( joinChannelResponse.ChannelInfo, true, joinChannelResponse.IsSilenced );
                var historyRequest = new ClientChannelHistoryRequest
                {
                    ChannelName = joinChannelResponse.ChannelInfo.Name,
                    MaxMessages = 20
                };
                _connector.Send( historyRequest );
            }
            else
            {
                Logger.Log($"FiveMinuteChat: Failed to join channel: {joinChannelResponse.ChannelInfo.Name}");
            }
        }
        
        private void OnServerLeaveChannelResponse( IConnectorClient client, ServerLeaveChannelResponse leaveChannelResponse )
        {
            if (leaveChannelResponse.Success)
            {
                Logger.Log($"FiveMinuteChat: Successfully left channel: {leaveChannelResponse.ChannelName}");
                _connector.OnChannelLeft( leaveChannelResponse.ChannelName);
            }
            else
            {
                Logger.Log($"FiveMinuteChat: Failed to leave channel: {leaveChannelResponse.ChannelName}");
            }
        }

        private void OnServerGenerateNamedChannelResponse( IConnectorClient client, ServerGenerateNamedChannelResponse response )
        {
            if( response.Success )
            {
                Logger.Log($"FiveMinuteChat: Successfully created channel: {response.ChannelInfo.Name}");
            }
            else
            {
                Logger.Log($"FiveMinuteChat: Failed to create channel: {response.Reason}");
            }
        }

        private void OnServerGenerateChannelResponse( IConnectorClient client, ServerGenerateChannelResponse response )
        {
            if( response.Success )
            {
                Logger.Log($"FiveMinuteChat: Successfully created channel: {response.ChannelInfo.Name}");
            }
            else
            {
                Logger.Log($"FiveMinuteChat: Failed to create channel: {response.Reason}");
            }
        }
        
        private async void OnServerChannelHistoryResponse( IConnectorClient client, ServerChannelHistoryResponse historyResponse )
        {
            Logger.Log($"FiveMinuteChat: Got channel history for: {historyResponse.ChannelName} ({historyResponse.ChatMessages.Count} messages)");
            foreach (var cm in historyResponse.ChatMessages.OrderBy( cm => cm.SentAt))
            {
                _connector.On( cm );
                await Task.Delay( 25 );
            }
        }

        private async void OnServerChannelInfoResponse( IConnectorClient client, ServerChannelInfoResponse response )
        {
            Logger.Log($"FiveMinuteChat: Got channel info for: {response.ChannelName} - {response.Users.Count}");
            _connector.OnChannelInfoReceived( new ChannelInfo
            {
                Members = response.Users,
                Name = response.ChannelName
            });
        }

        private void OnServerUserBanInfoMessage( IConnectorClient client, ServerUserBanInfoMessage message )
        {
            if( !string.IsNullOrEmpty( message.ChannelName ) )
            {
                Logger.Log($"FiveMinuteChat: User was kick-banned from: {message.ChannelName}");
                _connector.OnChannelLeft( message.ChannelName );
            }
            else
            {
                Logger.Log($"FiveMinuteChat: User was kick-banned from the server. Shutting down...");
                _connector.Disconnect( false );
            }
        }

        private void OnServerUserKickInfoMessage( IConnectorClient client, ServerUserKickInfoMessage message )
        {
            if( !string.IsNullOrEmpty( message.ChannelName ) )
            {
                Logger.Log($"FiveMinuteChat: User was kick-banned from: {message.ChannelName}");
                _connector.OnChannelLeft( message.ChannelName );
            }
        }
        
        private void OnServerWhisperHistoryResponseMessage( IConnectorClient client, ServerWhisperHistoryResponse response )
        {
            Logger.Log($"FiveMinuteChat: Got {response.WhisperMessages.Count} whispers from history response." );
            foreach( var message in response.WhisperMessages )
            {
                // pass them on
                message.ToUser = new UserInfo
                {
                    Name = _username
                };
                _connector.On( message );
            }
        }
    }
}
