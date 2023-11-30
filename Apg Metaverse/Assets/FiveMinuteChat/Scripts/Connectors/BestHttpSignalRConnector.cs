#if FiveMinuteChat_BestHttpEnabled
using System;
using BestHTTP.SignalRCore;
using BestHTTP.SignalRCore.Encoders;
using FiveMinuteChat.Helpers;
using FiveMinuteChat.Interfaces;
using Shared.Model.Messages;
using Shared.Model.Messages.Client;
using UnityEngine;

namespace FiveMinuteChat.Connectors
{
    public class BestHttpSignalRConnector : ConnectorBase
    {
        private readonly SignalRClientContainer _signalRClient = new SignalRClientContainer();

        private class SignalRClientContainer : IConnectorClient
        {
            public HubConnection Hub { get; set; }
            
            public bool Connected => Hub.State == ConnectionStates.Connected;
            public MessageHandler MessageHandler { get; set; }

            public async void Connect( string backendEndpoint, int backendPort )
            {
                Hub = new HubConnection(new Uri($"{backendEndpoint}:{backendPort}/signalr"), new JsonProtocol( new LitJsonEncoder()) );
                // Hub = new HubConnection(new Uri($"{backendEndpoint}:{backendPort}/signalr"), new JsonProtocol(new CustomJsonEncoder()));

                Hub.OnConnected += Hub_OnConnected;
                Hub.OnError += Hub_OnError;
                Hub.OnClosed += Hub_OnClosed;
                Hub.OnRedirected += Hub_Redirected;
                Hub.OnTransportEvent += Hub_OnTransportEvent;
                Hub.On<MessageContainer>( "GenericContainerized", OnMessage );
                await Hub.ConnectAsync();
            }

            public async void Disconnect()
                => await Hub.CloseAsync();

            public bool Send( MessageBase message, bool shouldEncrypt = true )
            {
                Hub.Send( "GenericContainerized", new MessageContainer(message) );
                return true;
            }

            public bool Send( byte[] data, bool shouldEncrypt = true ) 
                => throw new NotImplementedException();

            private void Hub_OnError(HubConnection hub, string error)
            {
                Console.WriteLine($"Hub error: {error}");
            }

            private void Hub_OnConnected(HubConnection hub)
            {
                Logger.Log( $"Hub Connected with <color=green>{hub.Transport.TransportType}</color> transport using the <color=green>{hub.Protocol.Name}</color> encoder.");
            }

            private void Hub_Redirected(HubConnection hub, Uri from, Uri to)
            {
                Logger.Log( $"Hub connection redirected to '<color=green>{hub.Uri}</color>' with Access Token: '<color=green>{hub.NegotiationResult.AccessToken}</color>'");
            }

            private void Hub_OnClosed(HubConnection hub)
            {
                Logger.Log( "Hub closed" );
            }

            private void Hub_OnTransportEvent(HubConnection hub, ITransport transport, TransportEvents @event)
            {
                Logger.Log( $"Transport event: {@event}" );
            }

            private void OnMessage( MessageContainer container )
            {
                MessageHandler.Handle(this, container.GetAs<MessageBase>());
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
                _signalRClient.Hub.Send( "GenericContainerized", new MessageContainer(message) );
            }
            catch( Exception e )
            {
                Logger.LogError($"FiveMinuteChat: Caught exception: {e.Message}\nReconnecting...");
                throw;
            }
        }

        public override bool Connected => _signalRClient.Hub.State == ConnectionStates.Connected;  
        public override async void Reconnect()
        {
            await _signalRClient.Hub.ConnectAsync();
        }

        public override async void Disconnect( bool allowReconnect )
        {
            await _signalRClient.Hub.CloseAsync();
            if( allowReconnect )
            {
                Reconnect();
            }
        }
    }
}
#endif
