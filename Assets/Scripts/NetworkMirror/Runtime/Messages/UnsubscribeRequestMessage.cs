#nullable enable

using Mirror;

namespace NetworkMirror.Runtime.Messages
{
    public struct UnsubscribeRequestMessage : NetworkMessage
    {
        public string MessageTypeId;
    }
}
