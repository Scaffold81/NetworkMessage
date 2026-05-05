#nullable enable

using NetworkMirror.Runtime.Presenter;
using NetworkMirror.Runtime.Service;
using NetworkMirror.Runtime.View;
using UnityEngine;
using Zenject;

namespace NetworkMirror.Installer
{
    public sealed class NetworkMessageInstaller : MonoInstaller
    {
        [SerializeField] private NetworkMessageView _view = null!;

        public override void InstallBindings()
        {
            Container.BindInterfacesTo<NetworkMessageService>()
                .AsSingle();

            Container.Bind<INetworkMessageView>()
                .FromInstance(_view)
                .AsSingle();

            Container.BindInterfacesTo<NetworkMessagePresenter>()
                .AsSingle();
        }
    }
}
