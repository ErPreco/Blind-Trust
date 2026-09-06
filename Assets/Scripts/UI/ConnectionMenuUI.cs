using System;
using System.Text.RegularExpressions;
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
    private TMP_Text errorText;

    private readonly string relaySetupError = "Error while creating the server";
    private readonly string invalidCodeError = "Invalid code {0} to join as a client";
    private readonly string fullRoomError = "The room already counts 2 players";
    private string waitingTextStringFormat;

    void OnEnable()
    {
        hostButton.onClick.AddListener(HostButtonPressed);
        clientButton.onClick.AddListener(ClientButtonPressed);
        codeInputField.onEndEdit.AddListener(OnCodeInserted);

        GameManager.Instance.OnClientConnected += GameManager_OnClientConnected;
        GameManager.Instance.OnOnePlayerDisconnected += GameManager_OnOneClientDisconnected;
        GameManager.Instance.OnPlayerRejected += GameManager_OnPlayerRejected;
    }

    void Start()
    {
        waitingTextStringFormat = waitingText.text;
        ResetPanel();
    }

    private async void HostButtonPressed()
    {
        SetInteractables(false);
        SetTexts(false);

        if (RelayManager.Instance.IsRelayEnabled)
        {
            try
            {
                RelayHostData relayHostData = await RelayManager.Instance.SetupRelay();
                waitingText.text = string.Format(waitingTextStringFormat, relayHostData.JoinCode);
            }
            catch (RelayServiceException error)
            {
                SetInteractables(true);
                errorText.gameObject.SetActive(true);
                errorText.text = relaySetupError;
                Debug.LogWarning($"Failed to setup the Relay Service: {error.Message}");
            }
        }
        else
        {
            waitingText.text = "Waiting for another player...";
        }

        NetworkManager.Singleton.StartHost();

        SetInteractables(false);
        waitingText.gameObject.SetActive(true);
    }

    private async void ClientButtonPressed()
    {
        SetInteractables(false);
        SetTexts(false);

        if (RelayManager.Instance.IsRelayEnabled)
        {
            if (string.IsNullOrEmpty(codeInputField.text))
            {
                SetInteractables(true);
                errorText.gameObject.SetActive(true);
                errorText.text = string.Format(invalidCodeError, "");
                errorText.text = Regex.Replace(errorText.text, @"\s+", " ");

                return;
            }

            try
            {
                await RelayManager.Instance.JoinRelay(codeInputField.text);
            }
            catch (RelayServiceException error)
            {
                SetInteractables(true);
                errorText.gameObject.SetActive(true);
                errorText.text = string.Format(invalidCodeError, codeInputField.text.ToUpper()).Trim();
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

    private void GameManager_OnClientConnected(object _sender, EventArgs _event)
    {
        panel.SetActive(false);

        OnPlayerAsHostStarted?.Invoke(this, EventArgs.Empty);
    }

    private void GameManager_OnOneClientDisconnected(object _sender, EventArgs _event)
    {
        ResetPanel();
    }

    private void GameManager_OnPlayerRejected(object _sender, EventArgs _event)
    {
        ResetPanel();
        errorText.gameObject.SetActive(true);
        errorText.text = fullRoomError;
    }

    private void SetInteractables(bool _isActive)
    {
        hostButton.interactable = _isActive;
        clientButton.interactable = _isActive;
        codeInputField.interactable = _isActive;
    }

    private void SetTexts(bool _isActive)
    {
        waitingText.gameObject.SetActive(_isActive);
        errorText.gameObject.SetActive(_isActive);
    }

    private void ResetPanel()
    {
        panel.SetActive(true);

        SetInteractables(true);
        codeInputField.text = "";

        SetTexts(false);
    }

    void OnDisable()
    {
        hostButton.onClick.RemoveListener(HostButtonPressed);
        clientButton.onClick.RemoveListener(ClientButtonPressed);
        codeInputField.onEndEdit.RemoveListener(OnCodeInserted);
    }
}
