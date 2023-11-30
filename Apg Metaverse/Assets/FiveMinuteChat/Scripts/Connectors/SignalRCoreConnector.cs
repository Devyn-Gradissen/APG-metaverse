using System;
using FiveMinuteChat.Helpers;
using FiveMinuteChat.Interfaces;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.Extensions.DependencyInjection;
using Shared.Model.Messages;
using Shared.Model.Messages.Client;
using UnityEngine;

namespace FiveMinuteChat.Connectors
{
    public class SignalRCoreConnector : ConnectorBase
    {
        private readonly SignalRClientContainer _signalRClient = new SignalRClientContainer();

        private class SignalRClientContainer : IConnectorClient
        {
            public HubConnection Hub { get; set; }
            
            public bool Connected => Hub?.State == HubConnectionState.Connected;
            public MessageHandler MessageHandler { get; set; }

            public async void Connect( string backendEndpoint, int backendPort )
            {
                Logger.Log($"FiveMinuteChat: Connecting to endpoint {backendEndpoint}:{backendPort}/signalr");
                Hub = new HubConnectionBuilder()
                    .WithUrl( $"{backendEndpoint}:{backendPort}/signalr" )
                    .WithAutomaticReconnect()
                    .AddJsonProtocol()
                    .Build();
                
                Hub.On<MessageContainer>( "GenericContainerized", OnMessage );
                await Hub.StartAsync();
            }

            public async void Disconnect()
                => await Hub.StopAsync();

            public bool Send( MessageBase message, bool shouldEncrypt = true )
            {
                Hub.SendAsync("GenericContainerized", new MessageContainer( message ) );
                return true;
            }

            public bool Send( byte[] data, bool shouldEncrypt = true ) 
                => throw new NotImplementedException();

            private void OnMessage( MessageContainer message )
            {
                MessageHandler.Handle(this, message.GetAs<MessageBase>());
            }
        }
        
        private void Awake()
        {
            var messageHandler = new MessageHandler( this );
            _signalRClient.MessageHandler = messageHandler;
            Init( _signalRClient, messageHandler);
        }

        public override void Connect( string backendEndpoint, int backendPort )
            => _signalRClient.Connect( backendEndpoint, backendPort );

        public override void Send( ClientMessageBase message, bool shouldEncrypt = true )
        {
            try
            {
                if( message.IsAckRequested )
                {
                    AddToAckQueue( message );
                }
                _signalRClient.Hub.SendAsync( "GenericContainerized", new MessageContainer(message) );
            }
            catch( Exception e )
            {
                Logger.LogError($"FiveMinuteChat: Caught exception: {e.Message}\nReconnecting...");
                throw;
            }
        }

        public override bool Connected => _signalRClient.Hub.State == HubConnectionState.Connected;  
        public override async void Reconnect()
        {
            await _signalRClient.Hub.StartAsync();
        }

        public override async void Disconnect( bool allowReconnect )
        {
            if( _signalRClient?.Connected ?? false )
            {
                await _signalRClient.Hub.StopAsync();
            }
            if( allowReconnect )
            {
                Reconnect();
            }
        }
    }
}
