#nullable enable

using System;
using Mirror;
using NetworkMirror.Demo;
using NetworkMirror.Runtime.Service;
using NetworkMirror.Runtime.View;
using Zenject;

namespace NetworkMirror.Runtime.Presenter
{
    public sealed class NetworkMessagePresenter : IInitializable, IDisposable
    {
        private readonly INetworkMessageService _service;
        private readonly INetworkMessageView _view;

        public NetworkMessagePresenter(INetworkMessageService service, INetworkMessageView view)
        {
            _service = service;
            _view = view;
        }

        public void Initialize()
        {
            AppNetworkManager.OnClientConnectedEvent += OnClientConnected;

            if (NetworkClient.isConnected)
                OnClientConnected();
        }

        public void Dispose()
        {
            AppNetworkManager.OnClientConnectedEvent -= OnClientConnected;

            if (NetworkClient.isConnected)
                _service.ClientUnsubscribe<HelloMessage>();
        }

        private void OnClientConnected()
        {
            _service.ClientSubscribe<HelloMessage>(OnHelloMessage);
        }

        private void OnHelloMessage(HelloMessage msg)
        {
            _view.ShowMessage(msg.Text);
        }
    }
}
