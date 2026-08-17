using System;
using UnityEngine;

public class GameCompletedUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;

    void OnEnable()
    {
        GameManager.Instance.OnWinConditionMet += GameManager_OnWinConditionMet;
    }

    void Start()
    {
        panel.SetActive(false);
    }

    private void GameManager_OnWinConditionMet(object _sender, EventArgs _event)
    {
        panel.SetActive(true);
    }
}
