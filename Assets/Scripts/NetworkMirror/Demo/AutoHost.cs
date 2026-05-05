using Mirror;
using System.Collections;
using UnityEngine;

namespace NetworkMirror.Demo
{
    public sealed class AutoHost : MonoBehaviour
    {
        private IEnumerator Start()
        {
            yield return null;
            NetworkManager.singleton.StartHost();
        }
    }
}
