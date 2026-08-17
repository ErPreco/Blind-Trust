using System;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
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

    void OnEnable()
    {
        hostButton.onClick.AddListener(HostButtonPressed);
        clientButton.onClick.AddListener(ClientButtonPressed);
        codeInputField.onEndEdit.AddListener(OnCodeInserted);
    }

    void Start()
    {
        NetworkManager.Singleton.OnConnectionEvent += NetworkManager_OnConnectionEvent;

        panel.SetActive(true);
        waitingText.gameObject.SetActive(false);
    }

    private async void HostButtonPressed()
    {
        if (RelayManager.Instance.IsRelayEnabled)
        {
            RelayHostData relayHostData = await RelayManager.Instance.SetupRelay();
            waitingText.text = string.Format(waitingText.text, relayHostData.JoinCode);
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
        if (RelayManager.Instance.IsRelayEnabled && !string.IsNullOrEmpty(codeInputField.text))
        {
            await RelayManager.Instance.JoinRelay(codeInputField.text);
        }

        NetworkManager.Singleton.StartClient();
        panel.SetActive(false);

        OnPlayerAsClientStarted?.Invoke(this, EventArgs.Empty);
    }


    private void OnCodeInserted(string _)
    {
        ClientButtonPressed();
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

    void OnDisable()
    {
        hostButton.onClick.RemoveListener(HostButtonPressed);
        clientButton.onClick.RemoveListener(ClientButtonPressed);
        codeInputField.onEndEdit.RemoveListener(OnCodeInserted);
    }
}
