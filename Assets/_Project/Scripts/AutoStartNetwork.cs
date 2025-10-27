using FishNet.Managing;
using ParrelSync;
using UnityEngine;

public class AutoStartNetwork : MonoBehaviour
{
    private NetworkManager networkManager;
    private void Start()
    {
        networkManager = GetComponent<NetworkManager>();
#if UNITY_EDITOR
        if (!ClonesManager.IsClone())
            networkManager.ServerManager.StartConnection();
        networkManager.ClientManager.StartConnection();
#elif !UNITY_EDITOR && UNITY_SERVER
            _networkManager.ServerManager.StartConnection();
#else
            _networkManager.ClientManager.StartConnection();
#endif
    }
}
