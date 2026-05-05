#nullable enable

using Mirror;
using NetworkMirror.Demo;
using NetworkMirror.Runtime.Service;
using UnityEngine;
using Zenject;

namespace NetworkMirror.Runtime
{
    public sealed class DemoServer : MonoBehaviour
    {
        [Inject] private INetworkMessageService _service = null!;

        private void OnEnable()
        {
            _service.OnClientSubscribed += OnClientSubscribed;
        }

        private void OnDisable()
        {
            _service.OnClientSubscribed -= OnClientSubscribed;
        }

        private void OnClientSubscribed(NetworkConnectionToClient conn, string typeId)
        {
            if (typeId == typeof(HelloMessage).FullName)
                _service.ServerSend(conn, new HelloMessage { Text = "Hello Client!" });
        }
    }
}
