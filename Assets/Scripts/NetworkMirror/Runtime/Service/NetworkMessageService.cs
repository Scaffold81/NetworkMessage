#nullable enable

using System;
using System.Collections.Generic;
using Mirror;
using NetworkMirror.Runtime.Messages;
using Zenject;

namespace NetworkMirror.Runtime.Service
{
    public sealed class NetworkMessageService : INetworkMessageService, IInitializable, IDisposable
    {
        public event Action<NetworkConnectionToClient, string>? OnClientSubscribed;

        private readonly Dictionary<string, HashSet<NetworkConnectionToClient>> _serverSubscribers = new();
        private readonly Dictionary<string, Action<byte[]>> _clientHandlers = new();

        public void Initialize()
        {
            NetworkServer.RegisterHandler<SubscribeRequestMessage>(OnServerReceiveSubscribe);
            NetworkServer.RegisterHandler<UnsubscribeRequestMessage>(OnServerReceiveUnsubscribe);
            NetworkClient.RegisterHandler<NetworkEnvelopeMessage>(OnClientReceiveEnvelope);
        }

        public void Dispose()
        {
            NetworkServer.UnregisterHandler<SubscribeRequestMessage>();
            NetworkServer.UnregisterHandler<UnsubscribeRequestMessage>();
            NetworkClient.UnregisterHandler<NetworkEnvelopeMessage>();
            _serverSubscribers.Clear();
            _clientHandlers.Clear();
        }

        public void ServerSend<T>(T message, int channelId = Channels.Reliable)
            where T : struct, NetworkMessage
        {
            string typeId = typeof(T).FullName!;
            if (!_serverSubscribers.TryGetValue(typeId, out HashSet<NetworkConnectionToClient>? conns))
                return;

            NetworkEnvelopeMessage envelope = BuildEnvelope(message, typeId);
            foreach (NetworkConnectionToClient conn in conns)
                conn.Send(envelope, channelId);
        }

        public void ServerSend<T>(NetworkConnectionToClient connection, T message, int channelId = Channels.Reliable)
            where T : struct, NetworkMessage
        {
            string typeId = typeof(T).FullName!;
            NetworkEnvelopeMessage envelope = BuildEnvelope(message, typeId);
            connection.Send(envelope, channelId);
        }

        public void ClientSubscribe<T>(Action<T> handler)
            where T : struct, NetworkMessage
        {
            string typeId = typeof(T).FullName!;
            _clientHandlers[typeId] = payload =>
            {
                NetworkReaderPooled reader = NetworkReaderPool.Get(payload);
                try
                {
                    T msg = reader.Read<T>();
                    handler(msg);
                }
                finally
                {
                    NetworkReaderPool.Return(reader);
                }
            };
            NetworkClient.Send(new SubscribeRequestMessage { MessageTypeId = typeId });
        }

        public void ClientUnsubscribe<T>()
            where T : struct, NetworkMessage
        {
            string typeId = typeof(T).FullName!;
            _clientHandlers.Remove(typeId);
            NetworkClient.Send(new UnsubscribeRequestMessage { MessageTypeId = typeId });
        }

        private void OnServerReceiveSubscribe(NetworkConnectionToClient conn, SubscribeRequestMessage msg)
        {
            if (!_serverSubscribers.TryGetValue(msg.MessageTypeId, out HashSet<NetworkConnectionToClient>? set))
            {
                set = new HashSet<NetworkConnectionToClient>();
                _serverSubscribers[msg.MessageTypeId] = set;
            }
            set.Add(conn);
            OnClientSubscribed?.Invoke(conn, msg.MessageTypeId);
        }

        private void OnServerReceiveUnsubscribe(NetworkConnectionToClient conn, UnsubscribeRequestMessage msg)
        {
            if (_serverSubscribers.TryGetValue(msg.MessageTypeId, out HashSet<NetworkConnectionToClient>? set))
                set.Remove(conn);
        }

        private void OnClientReceiveEnvelope(NetworkEnvelopeMessage envelope)
        {
            if (_clientHandlers.TryGetValue(envelope.MessageTypeId, out Action<byte[]>? handler))
                handler(envelope.Payload);
        }

        private static NetworkEnvelopeMessage BuildEnvelope<T>(T message, string typeId)
            where T : struct, NetworkMessage
        {
            NetworkWriterPooled writer = NetworkWriterPool.Get();
            try
            {
                writer.Write(message);
                return new NetworkEnvelopeMessage
                {
                    MessageTypeId = typeId,
                    Payload = writer.ToArray()
                };
            }
            finally
            {
                NetworkWriterPool.Return(writer);
            }
        }
    }
}
