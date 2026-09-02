using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public event EventHandler OnGameStarted;
    public event EventHandler OnWinConditionMet;
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; private set; }

    [SerializeField]
    private NetworkObject agentPrefab;
    [SerializeField]
    private Transform agentSpawnPoint;
    [SerializeField]
    private ConnectionMenuUI connectionMenuUI;

    void OnEnable()
    {
        connectionMenuUI.OnPlayerAsHostStarted += ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted += ConnectionMenuUI_OnClientStarted;
        GameInput.Instance.OnMenuPerformed += GameInput_OnMenuPerformed;
    }

    private void ConnectionMenuUI_OnHostStarted(object _sender, EventArgs _event)
    {
        StartGame();
    }

    private void ConnectionMenuUI_OnClientStarted(object _sender, EventArgs _event)
    {
        StartGame();
    }

    private void GameInput_OnMenuPerformed(object _sender, GameInput.OnMenuPerformedEventArgs _event)
    {
        if (IsGameStarted)
        {
            IsGamePaused = _event.IsMenuOpened;
        }
    }

    private void StartGame()
    {
        SpawnAgent();
        IsGameStarted = true;

        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnAgent()
    {
        if (!IsServer) return;

        Instantiate(agentPrefab, agentSpawnPoint.position, agentSpawnPoint.rotation).GetComponent<NetworkObject>().Spawn(true);
    }

    public void WinConditionMet()
    {
        OnWinConditionMet?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        connectionMenuUI.OnPlayerAsHostStarted -= ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted -= ConnectionMenuUI_OnClientStarted;
    }
}
