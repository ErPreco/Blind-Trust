using System;
using UnityEngine;

public class GameCompletedUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    void OnEnable()
    {
        GameManager.Instance.OnWinConditionMet += GameManager_OnWinConditionMet;
        GameManager.Instance.OnOnePlayerDisconnected += GameManager_OnOnePlayerDisconnected;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    private void GameManager_OnWinConditionMet(object _sender, EventArgs _event)
    {
        panel.SetActive(true);
    }

    private void GameManager_OnOnePlayerDisconnected(object _sender, EventArgs _event)
    {
        panel.SetActive(false);
    }
}
