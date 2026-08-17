using System;
using UnityEngine;

public class PlatformsManager : MonoBehaviour
{
    [SerializeField]
    private Transform whitePlatforms;
    [SerializeField]
    private Transform blackPlatforms;
    [SerializeField]
    private MenuUI menuUI;

    void OnEnable()
    {
        menuUI.OnPlayerAsHostStarted += MenuUI_OnHostStarted;
        menuUI.OnPlayerAsClientStarted += MenuUI_OnClientStarted;
    }

    private void MenuUI_OnHostStarted(object _sender, EventArgs _event)
    {
        ShowPlatforms(whitePlatforms);
        HidePlatforms(blackPlatforms);
    }

    private void MenuUI_OnClientStarted(object _sender, EventArgs _event)
    {
        HidePlatforms(whitePlatforms);
        ShowPlatforms(blackPlatforms);
    }

    private void ShowPlatforms(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            Platform platform = child.GetComponent<Platform>();
            platform.Show();
        }
    }

    private void HidePlatforms(Transform _parent)
    {
        foreach (Transform child in _parent)
        {
            Platform platform = child.GetComponent<Platform>();
            platform.Hide();
        }
    }

    void OnDisable()
    {
        menuUI.OnPlayerAsHostStarted -= MenuUI_OnHostStarted;
        menuUI.OnPlayerAsClientStarted -= MenuUI_OnClientStarted;
    }
}
