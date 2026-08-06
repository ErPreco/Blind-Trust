using Unity.Netcode;
using UnityEngine;

public class MenuUI : MonoBehaviour
{
    void OnGUI()
    {
        float w = 200, h = 40;
        float x = 10, y = 10;

        if (!NetworkManager.Singleton.IsClient && !NetworkManager.Singleton.IsServer)
        {
            if (GUI.Button(new Rect(x, y, w, h), "Host")) NetworkManager.Singleton.StartHost();
            if (GUI.Button(new Rect(x, y + h + 10, w, h), "Client")) NetworkManager.Singleton.StartClient();
            if (GUI.Button(new Rect(x, y + 2 * (h + 10), w, h), "Server")) NetworkManager.Singleton.StartServer();
        }
    }
}
