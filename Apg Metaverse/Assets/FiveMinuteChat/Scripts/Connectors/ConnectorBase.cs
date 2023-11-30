using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using FiveMinuteChat.Helpers;
using FiveMinuteChat.Interfaces;
using Shared.Model;
using Shared.Model.Messages;
using Shared.Model.Messages.Client;
using Shared.Model.Messages.Server;
using UnityEngine;

namespace FiveMinuteChat.Connectors
{
    public abstract class ConnectorBase : MonoBehaviour, IConnector
    {
        protected class AckQueueMessageContainer
        {
            public DateTime CreatedAt { get; } = DateTime.Now;
            public ClientMessageBase Message { get; }
            public TimeSpan Age => DateTime.Now - CreatedAt;

            public AckQueueMessageContainer( ClientMessageBase message )
            {
                Message = message;
            }
        }

        public event OnChannelJoined ChannelJoined;
        public event OnChannelLeft ChannelLeft;
        public event OnConnectionAccepted ConnectionAccepted;
        public event OnChannelInfo ChannelInfoReceived;
        public void On( MessageBase message )
        {
            var type = message.GetType();
            if( _messageCallbacks.ContainsKey( type ) )
            {
                foreach (var callback in _messageCallbacks[type].Values)
                {
                    callback( message );
                }
            }
        }

        private readonly Dictionary<Type, Dictionary<Guid, Action<MessageBase>>> _messageCallbacks =
            new Dictionary<Type, Dictionary<Guid, Action<MessageBase>>>();

        public void Awake()
        {
            ConnectionAccepted += (ch) => OnConnected();
        }

        public Guid Subscribe<T>( Action<T> callback ) where T : MessageBase
        {
            var type = typeof(T);
            if( !_messageCallbacks.ContainsKey(type) )
            {
                _messageCallbacks.Add(type, new Dictionary<Guid, Action<MessageBase>>());
            }

            var callbackId = Guid.NewGuid();
            _messageCallbacks[type].Add(callbackId, ( message ) => callback( message as T ) );

            return callbackId;
        }
        
        public void Unsubscribe( Guid callbackId )
        {
            _messageCallbacks.Values.ToList().ForEach( d =>
            {
                if( d.ContainsKey( callbackId ) )
                {
                    d.Remove( callbackId );
                }
            } );
        }
        
        public abstract void Connect( string backendEndpoint, int backendPort );

        private IConnectorClient _client;
        protected MessageHandler _messageHandler;
        protected readonly ConcurrentDictionary<Guid,AckQueueMessageContainer> AckQueue = new ConcurrentDictionary<Guid,AckQueueMessageContainer>();

        public void Init( IConnectorClient connectorClient, MessageHandler messageHandler )
        {
            _client = connectorClient;
            _messageHandler = messageHandler;
        }
        
        public virtual void SetUsername( string username )
        {
            if( string.IsNullOrWhiteSpace( username ) )
            {
                throw new ArgumentException( $"FiveMinuteChat: Username '{username}' is invalid. It cannot be empty.");
            }

            _messageHandler.SetUsername( username );
            if( _client.Connected )
            {
                Send( new ClientSetUsernameRequest{ Username = username } );
            }
        }

        public abstract void Send( ClientMessageBase message, bool shouldEncrypt = true );

        public abstract bool Connected { get; }
        public abstract void Reconnect();

        public abstract void Disconnect( bool b );

        public void AddToAckQueue( ClientMessageBase message )
        {
            AckQueue.TryAdd( message.MessageId, new AckQueueMessageContainer( message ) );
        }

        public void ClearAckQueue()
        {
            AckQueue.Clear();
        }

        public void AckMessage( Guid messageId )
        {
            if( AckQueue.ContainsKey( messageId ) )
            {
                AckQueue.TryRemove( messageId, out _ );
            }
            else
            {
                Logger.LogWarning($"FiveMinuteChat: Got ack for message id {messageId}, which was not in the ack queue.");
            }
        }

        protected void OnConnected()
        {
            var queue = AckQueue
                .Values
                .ToList()
                .OrderByDescending( i => i.Age );
            foreach( var itemKv in queue )
            {
                Send(itemKv.Message);
            }
        }

        public virtual void SetCredentials( string applicationName, string applicationSecret, string uniqueUserId ) 
            => _messageHandler.SetCredentials( applicationName, applicationSecret, uniqueUserId );
        
        public virtual void OnChannelJoined( ChannelInfo channelInfo, bool canLeave, bool isSilenced )
            => ChannelJoined?.Invoke( channelInfo, canLeave, isSilenced );

        public virtual void OnChannelLeft( string channelName ) 
            => ChannelLeft?.Invoke( channelName );

        public virtual void OnConnectionAccepted( List<ChannelInfo> welcomeAvailableChannels ) 
            => ConnectionAccepted?.Invoke( welcomeAvailableChannels );

        public virtual void OnChannelInfoReceived( ChannelInfo channelInfo )
            => ChannelInfoReceived?.Invoke( channelInfo );
    }
}
