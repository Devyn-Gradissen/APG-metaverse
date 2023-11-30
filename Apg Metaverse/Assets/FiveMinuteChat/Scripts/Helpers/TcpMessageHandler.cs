using System;
using System.Text;
using FiveMinuteChat.Interfaces;
using Shared.Helpers;
using Shared.Model.Messages;
using FiveMinuteChat.Telepathy;
using UnityEngine;

namespace FiveMinuteChat.Helpers
{
    public class TcpMessageHandler : MessageHandler
    {
        public TcpMessageHandler(IConnector connector) 
            : base(connector)
        { }
        
        public void Handle(IConnectorClient client, Message message)
        {
            MessageBase deserializedMessage;
            try
            {
                deserializedMessage = Serializer.Deserialize( message.data );
            }
            catch( Exception e )
            {
                Logger.LogError( $"Data Message from {message.connectionId} could not be deserialized. Payload is '{Encoding.UTF8.GetString(message.data)}'" );
                return;
            }

            Handle( client, deserializedMessage );
        }
    }
}
