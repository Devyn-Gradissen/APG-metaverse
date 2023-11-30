using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Shared.Model;
using Shared.Model.Messages.Server;
using UnityEngine;
using UnityEngine.UI;

namespace FiveMinuteChat
{
    /***
     * This behavior is used as an example of how to collect all chat messages regardless of channel,
     * allowing for additional filtering to happen in the calling client code instead.  
     */
    public class MasterChatLogBehavior : MonoBehaviour
    {
        private class ChatEntry
        {
            public Guid MessageId { get; set; }
            public DateTime SentAt { get; set; }
            public UserInfo FromUser { get; set; }
            public string Content { get; set; }
            public string ChannelName { get; set; }
        }

        private readonly Dictionary<string, ChannelInfo> _channelInfos = new Dictionary<string, ChannelInfo>();
        private readonly Queue<ChatEntry> _chatEntries = new Queue<ChatEntry>();

        public bool IsConnected;

        public ChatConnectionBehavior Connection;
        private Guid _serverChatMessageCallbackId;

        void Start()
        {
            IsConnected = false;
            if( !Connection )
            {
                Connection = GetComponentInParent<ChatConnectionBehavior>();
            }
            if( Connection )
            {
                _serverChatMessageCallbackId = Connection.Subscribe<ServerChatMessage>(OnChatMessageReceived);

                Connection.ChannelInfoReceived += OnChannelInfoReceived;
                Connection.ConnectionAccepted += OnConnectionOnConnectionAccepted;
            }
            else
            {
                Logger.LogError($"No {nameof(ChatConnectionBehavior)} has been assigned to the Connection field. It must either be found as a parent of this GameObject or set explicitly via the Connection field on this behavior.");
            }
        }

        private void OnDestroy()
        {
            IsConnected = false;
            if( Connection )
            {
                Connection.Unsubscribe(_serverChatMessageCallbackId);
                Connection.ChannelInfoReceived -= OnChannelInfoReceived;
                Connection.ConnectionAccepted -= OnConnectionOnConnectionAccepted;
            }
        }

        private void OnApplicationQuit()
        {
            IsConnected = false;
            if( Connection )
            {
                Connection.Unsubscribe(_serverChatMessageCallbackId);
                Connection.ChannelInfoReceived -= OnChannelInfoReceived;
                Connection.ConnectionAccepted -= OnConnectionOnConnectionAccepted;
            }
        }

        private static int _maxMessages = 60;

        private void OnChatMessageReceived( ServerChatMessage message )
        {
            if( message.Content.Length > 1 )
            {
                if( _chatEntries.Any( ce => message.MessageId != Guid.Empty && ce.MessageId == message.MessageId ) )
                {
                    return;
                }
                _chatEntries.Enqueue( new ChatEntry
                {
                    MessageId = message.MessageId,
                    ChannelName = message.ChannelName,
                    FromUser = message.FromUser,
                    SentAt = message.SentAt,
                    Content = message.Content
                } );
                if( _chatEntries.Count > _maxMessages )
                {
                    _chatEntries.Dequeue();
                }
            }
        }

        private void OnChannelInfoReceived( ChannelInfo channelInfo )
        {
            if( _channelInfos.ContainsKey( channelInfo.Name ) )
            {
                _channelInfos.Remove( channelInfo.Name );
            }

            Logger.Log( $"Got channel info for {channelInfo.Name} with {channelInfo.Members.Count} members" );
            _channelInfos.Add( channelInfo.Name, channelInfo );
        }
        
        private void OnConnectionOnConnectionAccepted( List<ChannelInfo> availableChannels )
        {
            IsConnected = true;
        }

        public string[] GetChannelMembersFor( string channelName )
        {
            if( _channelInfos.ContainsKey( channelName ) )
            {
                return _channelInfos[channelName]
                    .Members
                    .Select( m => m.Name )
                    .ToArray();
            }

            return new string[0];
        }

        public string GetLatestMessageFrom( string username, string channelName )
        {
            var latestMessage = _chatEntries
                .ToList()
                .Where( e => e.FromUser != null && e.FromUser.Name == username && e.ChannelName == channelName )
                .OrderByDescending( e => e.SentAt )
                .FirstOrDefault();
            return latestMessage == null ? string.Empty : latestMessage.Content;
        }
    }
}
