using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class GameMenuUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Button disconnectButton;
    [SerializeField]
    private Button quitButton;

    void OnEnable()
    {
        disconnectButton.onClick.AddListener(DisconnectButtonPressed);
        quitButton.onClick.AddListener(QuitButtonPressed);
    }

    private void DisconnectButtonPressed()
    {
        // NetworkManager.Singleton.DisconnectClient(NetworkManager.Singleton.LocalClientId);
        NetworkManager.Singleton.Shutdown();
    }

    private void QuitButtonPressed()
    {
        NetworkManager.Singleton.Shutdown();
        Application.Quit();
    }

    void Start()
    {
        GameInput.Instance.OnMenuPerformed += GameInput_OnMenuPerformed;

        panel.SetActive(false);
    }

    private void GameInput_OnMenuPerformed(object _sender, GameInput.OnMenuPerformedEventArgs _event)
    {
        if (!GameManager.Instance.IsGameStarted) return;

        panel.SetActive(_event.IsMenuOpened);
    }

    void OnDisable()
    {
        disconnectButton.onClick.RemoveListener(DisconnectButtonPressed);
        quitButton.onClick.RemoveListener(QuitButtonPressed);
    }
}
