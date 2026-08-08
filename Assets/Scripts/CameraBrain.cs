using System;
using UnityEngine;

public class CameraBrain : MonoBehaviour
{
    [SerializeField]
    private Transform cameraOrigin;
    [SerializeField]
    private MenuUI menuUI;

    void OnEnable()
    {
        menuUI.OnHostStarted += MenuUI_GameStarted;
        menuUI.OnClientStarted += MenuUI_GameStarted;
    }

    void Start()
    {
        transform.SetPositionAndRotation(cameraOrigin.position, cameraOrigin.rotation);
    }

    private void MenuUI_GameStarted(object _sender, EventArgs _event)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void OnDisable()
    {
        menuUI.OnHostStarted -= MenuUI_GameStarted;
        menuUI.OnClientStarted -= MenuUI_GameStarted;
    }
}
