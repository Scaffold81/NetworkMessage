using System;
using Mirror;

namespace NetworkMirror.Runtime
{
    public sealed class AppNetworkManager : NetworkManager
    {
        public static event Action OnClientConnectedEvent;

        public override void OnClientConnect()
        {
            base.OnClientConnect();
            OnClientConnectedEvent?.Invoke();
        }
    }
}
