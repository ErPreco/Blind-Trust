using System;
using UnityEngine;

public class CameraBrain : MonoBehaviour
{
    [SerializeField]
    private Transform cameraOrigin;

    void OnEnable()
    {
        GameInput.Instance.OnMenuPerformed += GameInput_OnMenuPerformed;
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnOnePlayerDisconnected += GameManager_OnOneClientDisconnected;
    }

    void Start()
    {
        transform.SetPositionAndRotation(cameraOrigin.position, cameraOrigin.rotation);
    }

    private void GameInput_OnMenuPerformed(object _sender, GameInput.OnMenuPerformedEventArgs _event)
    {
        Cursor.lockState = _event.IsMenuOpened ? CursorLockMode.None : CursorLockMode.Locked;
    }

    private void GameManager_OnGameStarted(object _sender, EventArgs _event)
    {
        Cursor.lockState = CursorLockMode.Locked;
    }


    private void GameManager_OnOneClientDisconnected(object _sender, EventArgs _event)
    {
        transform.SetPositionAndRotation(cameraOrigin.position, cameraOrigin.rotation);
        Cursor.lockState = CursorLockMode.None;
    }
}
