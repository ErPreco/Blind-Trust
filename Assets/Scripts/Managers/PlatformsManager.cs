using System;
using UnityEngine;

public class PlatformsManager : MonoBehaviour
{
    [SerializeField]
    private Transform whitePlatforms;
    [SerializeField]
    private Transform blackPlatforms;
    [SerializeField]
    private ConnectionMenuUI connectionMenuUI;

    void OnEnable()
    {
        connectionMenuUI.OnPlayerAsHostStarted += ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted += ConnectionMenuUI_OnClientStarted;
    }

    private void ConnectionMenuUI_OnHostStarted(object _sender, EventArgs _event)
    {
        ShowPlatforms(whitePlatforms);
        HidePlatforms(blackPlatforms);
    }

    private void ConnectionMenuUI_OnClientStarted(object _sender, EventArgs _event)
    {
        HidePlatforms(whitePlatforms);
        ShowPlatforms(blackPlatforms);
    }

    private void ShowPlatforms(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            ColoredPlatform platform = child.GetComponent<ColoredPlatform>();
            platform.Show();
        }
    }

    private void HidePlatforms(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            ColoredPlatform platform = child.GetComponent<ColoredPlatform>();
            platform.Hide();
        }
    }

    void OnDisable()
    {
        connectionMenuUI.OnPlayerAsHostStarted -= ConnectionMenuUI_OnHostStarted;
        connectionMenuUI.OnPlayerAsClientStarted -= ConnectionMenuUI_OnClientStarted;
    }
}
