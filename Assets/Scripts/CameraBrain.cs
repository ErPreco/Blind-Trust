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
        menuUI.OnPlayerAsHostStarted += MenuUI_GameStarted;
        menuUI.OnPlayerAsClientStarted += MenuUI_GameStarted;
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
        menuUI.OnPlayerAsHostStarted -= MenuUI_GameStarted;
        menuUI.OnPlayerAsClientStarted -= MenuUI_GameStarted;
    }
}
