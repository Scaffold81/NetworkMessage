#nullable enable

using UnityEngine;

namespace NetworkMirror.Runtime.View
{
    public sealed class NetworkMessageView : MonoBehaviour, INetworkMessageView
    {
        public void ShowMessage(string text)
        {
            Debug.Log(text);
        }
    }
}
