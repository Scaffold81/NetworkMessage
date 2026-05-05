#nullable enable

using Mirror;

namespace NetworkMirror.Runtime.Messages
{
    public struct SubscribeRequestMessage : NetworkMessage
    {
        public string MessageTypeId;
    }
}
