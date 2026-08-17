using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
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
