using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public event EventHandler OnGameStarted;
    public event EventHandler OnWinConditionMet;
    public event EventHandler OnOneClientDisconnected;
    public bool IsGameStarted { get; private set; }
    public bool IsGamePaused { get; private set; }

    [SerializeField]
    private NetworkObject agentPrefab;
    [SerializeField]
    private Transform agentSpawnPoint;
    [SerializeField]
    private ConnectionMenuUI connectionMenuUI;

    private NetworkObject agent;

    void OnEnable()
    {
        connectionMenuUI.OnPlayerAsHostStarted += ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted += ConnectionMenuUI_OnClientStarted;
        GameInput.Instance.OnMenuPerformed += GameInput_OnMenuPerformed;
    }

    void Start()
    {
        NetworkManager.OnConnectionEvent += NetworkManager_OnConnectionEvent;
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
        IsGamePaused = _event.IsMenuOpened;
    }

    private void NetworkManager_OnConnectionEvent(NetworkManager _networkManager, ConnectionEventData _data)
    {
        if (_data.EventType == ConnectionEvent.ClientDisconnected)
        {
            // One client disconnected, it might be the clocal client or the other one
            IsGameStarted = false;
            IsGamePaused = false;

            if (_data.ClientId != NetworkManager.Singleton.LocalClientId)
            {
                NetworkManager.Shutdown();
            }

            OnOneClientDisconnected?.Invoke(this, EventArgs.Empty);
        }
    }

    [Rpc(SendTo.Server)]
    private void DespawnAgentRpc()
    {
        agent.Despawn();
    }

    private void StartGame()
    {
        SpawnAgent();
        IsGameStarted = true;

        OnGameStarted?.Invoke(this, EventArgs.Empty);
    }

    private void SpawnAgent()
    {
        if (!NetworkManager.IsServer) return;

        agent = Instantiate(agentPrefab, agentSpawnPoint.position, agentSpawnPoint.rotation).GetComponent<NetworkObject>();
        agent.Spawn(true);
    }

    public void WinConditionMet()
    {
        OnWinConditionMet?.Invoke(this, EventArgs.Empty);
    }

    public void DisconnectClient()
    {
        DespawnAgentRpc();
        NetworkManager.Shutdown();
    }

    void OnDisable()
    {
        connectionMenuUI.OnPlayerAsHostStarted -= ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted -= ConnectionMenuUI_OnClientStarted;
    }
}
