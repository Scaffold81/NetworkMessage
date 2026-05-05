#nullable enable

using System;
using Mirror;

namespace NetworkMirror.Runtime.Service
{
    public interface INetworkMessageService
    {
        event Action<NetworkConnectionToClient, string> OnClientSubscribed;

        void ServerSend<T>(T message, int channelId = Channels.Reliable)
            where T : struct, NetworkMessage;

        void ServerSend<T>(NetworkConnectionToClient connection, T message, int channelId = Channels.Reliable)
            where T : struct, NetworkMessage;

        void ClientSubscribe<T>(Action<T> handler)
            where T : struct, NetworkMessage;

        void ClientUnsubscribe<T>()
            where T : struct, NetworkMessage;
    }
}
