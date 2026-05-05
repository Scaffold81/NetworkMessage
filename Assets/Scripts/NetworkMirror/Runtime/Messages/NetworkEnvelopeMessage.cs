#nullable enable

using Mirror;

namespace NetworkMirror.Runtime.Messages
{
    public struct NetworkEnvelopeMessage : NetworkMessage
    {
        public string MessageTypeId;
        public byte[] Payload;
    }
}
