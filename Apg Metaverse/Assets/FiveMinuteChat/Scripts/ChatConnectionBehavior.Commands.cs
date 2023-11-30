using System;
using Shared.Model.Messages.Client;
using UnityEngine;

namespace FiveMinuteChat
{
    public partial class ChatConnectionBehavior
    {
        public void SetUsername( string username ) => _connector?.SetUsername( username );
        
        public void CreateChannel( string channelName )
        {
            var message = string.IsNullOrEmpty( channelName )
                ? new ClientGenerateChannelRequest()
                : new ClientGenerateNamedChannelRequest
                {
                    ChannelName = channelName
                };
            _connector.Send( message );
        }
        
        public void JoinChannel( string channelName )
        {
            _connector.Send( new ClientJoinChannelRequest()
            {
                ChannelName = channelName
            } );
        }
        
        public void LeaveChannel( string channelName )
        {
            _connector.Send( new ClientLeaveChannelRequest()
            {
                ChannelName = channelName
            } );
        }
        
        public void Whisper( string recipientDisplayId, string message )
        {
            if( !_displayIdRegex.IsMatch( recipientDisplayId ) )
            {
                Logger.LogError($"FiveMinuteChat: {recipientDisplayId} is not a valid user display id.");
                return;
            }
            _connector.Send(  new ClientWhisperMessage
            {
                Recipient = recipientDisplayId,
                Content = message
            } );
        }

        public void Whois( string userDisplayId )
        {
            _connector.Send( new ClientUserInfoRequest
            {
                UserDisplayId =  userDisplayId
            } );
        }

        public void GetChannelInfo( string channelName )
        {
            _connector.Send( new ClientChannelInfoRequest()
            {
                ChannelName = channelName
            } );
        }

        public void SendChatMessage( string channelName, string message )
        {
            _connector.Send( new ClientChatMessage
            {
                Content = message,
                ChannelName = channelName
            } );
        }

        public void SendMessageReport( Guid messageId, string message )
        {
            _connector.Send( new ClientReportChatMessageRequest()
            {
                ReportDescription = message,
                ReportedMessageId = messageId
            } );
        }
    }
}
