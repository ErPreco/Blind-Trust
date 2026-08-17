using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindFirstObjectByType<GameManager>();
            }

            return instance;
        }
    }

    private static GameManager instance;

    [SerializeField]
    private NetworkObject agentPrefab;
    [SerializeField]
    private MenuUI menuUI;

    void OnEnable()
    {
        menuUI.OnPlayerAsHostStarted += MenuUI_OnHostStarted;
        menuUI.OnPlayerAsClientStarted += MenuUI_OnClientStarted;
    }

    private void MenuUI_OnHostStarted(object _sender, EventArgs _event)
    {
        SpawnAgent();
    }

    private void MenuUI_OnClientStarted(object _sender, EventArgs _event)
    {
        SpawnAgent();
    }

    private void SpawnAgent()
    {
        if (!IsServer) return;

        Instantiate(agentPrefab).GetComponent<NetworkObject>().Spawn();
    }

    void OnDisable()
    {
        menuUI.OnPlayerAsHostStarted -= MenuUI_OnHostStarted;
        menuUI.OnPlayerAsClientStarted -= MenuUI_OnClientStarted;
    }
}
