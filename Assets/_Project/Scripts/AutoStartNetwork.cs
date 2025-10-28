using FishNet.Managing;
#if UNITY_EDITOR
using ParrelSync;
#endif
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
            networkManager.ServerManager.StartConnection();
#else
            networkManager.ClientManager.StartConnection();
#endif
    }
}
