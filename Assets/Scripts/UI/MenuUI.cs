using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

public class MenuUI : MonoBehaviour
{
    public event EventHandler OnHostStarted;
    public event EventHandler OnClientStarted;

    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private Button hostButton;
    [SerializeField]
    private Button clientButton;

    void OnEnable()
    {
        hostButton.onClick.AddListener(HostButtonPressed);
        clientButton.onClick.AddListener(ClientButtonPressed);
    }

    void Start()
    {
        panel.SetActive(true);
    }

    private void HostButtonPressed()
    {
        NetworkManager.Singleton.StartHost();
        panel.SetActive(false);

        OnHostStarted?.Invoke(this, EventArgs.Empty);
    }

    private void ClientButtonPressed()
    {
        NetworkManager.Singleton.StartClient();
        panel.SetActive(false);

        OnClientStarted?.Invoke(this, EventArgs.Empty);
    }

    void OnDisable()
    {
        hostButton.onClick.RemoveListener(HostButtonPressed);
        clientButton.onClick.RemoveListener(ClientButtonPressed);
    }
}
