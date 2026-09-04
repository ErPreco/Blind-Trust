using System;
using TMPro;
using Unity.Netcode;
using Unity.Services.Relay;
using UnityEngine;
using UnityEngine.UI;

public class ConnectionMenuUI : MonoBehaviour
{
    public event EventHandler OnPlayerAsHostStarted;
    public event EventHandler OnPlayerAsClientStarted;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button clientButton;
    [SerializeField]
    private TMP_InputField codeInputField;
    [SerializeField]
    private TMP_Text waitingText;
    [SerializeField]
    private TMP_Text invalidCodeText;

    private string waitingTextStringFormat;

    void OnEnable()
    {
        hostButton.onClick.AddListener(HostButtonPressed);
        clientButton.onClick.AddListener(ClientButtonPressed);
        codeInputField.onEndEdit.AddListener(OnCodeInserted);

        GameManager.Instance.OnOneClientDisconnected += GameManager_OnOneClientDisconnected;
    }

    void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;

        waitingTextStringFormat = waitingText.text;
        ResetPanel();
    }

    private async void HostButtonPressed()
    {
        invalidCodeText.gameObject.SetActive(false);
        if (RelayManager.Instance.IsRelayEnabled)
        {
            RelayHostData relayHostData = await RelayManager.Instance.SetupRelay();
            waitingText.text = string.Format(waitingTextStringFormat, relayHostData.JoinCode);
        }
        else
        {
            waitingText.text = "Waiting for another player...";
        }

        NetworkManager.Singleton.StartHost();

        hostButton.interactable = false;
        clientButton.interactable = false;
        codeInputField.interactable = false;
        waitingText.gameObject.SetActive(true);
    }

    private async void ClientButtonPressed()
    {
        if (RelayManager.Instance.IsRelayEnabled)
        {
            if (string.IsNullOrEmpty(codeInputField.text))
            {
                waitingText.gameObject.SetActive(false);
                invalidCodeText.gameObject.SetActive(true);

                return;
            }

            try
            {
                await RelayManager.Instance.JoinRelay(codeInputField.text);
            }
            catch (RelayServiceException error)
            {
                waitingText.gameObject.SetActive(false);
                invalidCodeText.gameObject.SetActive(true);
                Debug.LogWarning($"Failed to join relay with code '{codeInputField.text}': {error.Message}");
                return;
            }
        }

        NetworkManager.Singleton.StartClient();
        panel.SetActive(false);

        OnPlayerAsClientStarted?.Invoke(this, EventArgs.Empty);
    }

    private void OnCodeInserted(string _)
    {
        ClientButtonPressed();
    }

    private void GameManager_OnOneClientDisconnected(object _sender, EventArgs _event)
    {
        ResetPanel();
    }

    private void NetworkManager_OnConnectionEvent(NetworkManager _networkManager, ConnectionEventData _data)
    {
        if (_data.EventType == ConnectionEvent.PeerConnected)
        {
            // The second player connected to the host
            panel.SetActive(false);

            OnPlayerAsHostStarted?.Invoke(this, EventArgs.Empty);
        }
    }

    private void ResetPanel()
    {
        panel.SetActive(true);

        hostButton.interactable = true;
        clientButton.interactable = true;
        codeInputField.interactable = true;
        codeInputField.text = "";

        waitingText.gameObject.SetActive(false);
        invalidCodeText.gameObject.SetActive(false);
    }

    void OnDisable()
    {
        hostButton.onClick.RemoveListener(HostButtonPressed);
        clientButton.onClick.RemoveListener(ClientButtonPressed);
        codeInputField.onEndEdit.RemoveListener(OnCodeInserted);
    }
}
