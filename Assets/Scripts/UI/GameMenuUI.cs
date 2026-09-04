using System;
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

        GameInput.Instance.OnMenuPerformed += GameInput_OnMenuPerformed;
        GameManager.Instance.OnOneClientDisconnected += GameManager_OnOneClientDisconnected;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    private void DisconnectButtonPressed()
    {
        GameManager.Instance.DisconnectClient();
    }

    private void QuitButtonPressed()
    {
        GameManager.Instance.DisconnectClient();
        Application.Quit();
    }

    private void GameInput_OnMenuPerformed(object _sender, GameInput.OnMenuPerformedEventArgs _event)
    {
        panel.SetActive(_event.IsMenuOpened);
    }

    private void GameManager_OnOneClientDisconnected(object _sender, EventArgs _event)
    {
        panel.SetActive(false);
    }

    void OnDisable()
    {
        disconnectButton.onClick.RemoveListener(DisconnectButtonPressed);
        quitButton.onClick.RemoveListener(QuitButtonPressed);
    }
}
