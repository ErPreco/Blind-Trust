using System;
using Unity.Netcode;
using UnityEngine;

public class GameManager : NetworkSingleton<GameManager>
{
    public event EventHandler OnClientConnected;
    public event EventHandler OnGameStarted;
    public event EventHandler OnWinConditionMet;
    public event EventHandler OnOnePlayerDisconnected;
    public event EventHandler OnPlayerRejected;
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
        if (_data.EventType == ConnectionEvent.PeerConnected)
        {
            // Received by the host, the second player (client) connected
            OnClientConnected?.Invoke(this, EventArgs.Empty);
        }
        else if (_data.EventType == ConnectionEvent.ClientDisconnected)
        {
            if (_data.ClientId >= 2)
            {
                // From the host POV, the disconnected client was the third one in the room, so actually it was rejected
                return;
            }

            // One client disconnected, it might be the clocal client or the other one,
            // but in both cases the game has to stop
            IsGameStarted = false;
            IsGamePaused = false;

            if (_data.ClientId == 0 && !NetworkManager.IsServer)
            {
                // From the rejected client POV, it gets ID 0 but it is not the server
                OnOnePlayerDisconnected?.Invoke(this, EventArgs.Empty);
                OnPlayerRejected?.Invoke(this, EventArgs.Empty);
                return;
            }

            if (_data.ClientId != NetworkManager.Singleton.LocalClientId)
            {
                NetworkManager.Shutdown();
            }

            OnOnePlayerDisconnected?.Invoke(this, EventArgs.Empty);
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
