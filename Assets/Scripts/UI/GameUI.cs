using System;
using TMPro;
using UnityEngine;

public class GameUI : MonoBehaviour
{
    [SerializeField]
    private GameObject panel;
    [SerializeField]
    private TMP_Text stopWatchText;

    private bool startStopWatch;
    private float stopWatchValue;
    private string stopWatchStringFormat;

    void OnEnable()
    {
        GameManager.Instance.OnGameStarted += GameManager_OnGameStarted;
        GameManager.Instance.OnWinConditionMet += GameManager_OnWinConditionMet;
        GameManager.Instance.OnOneClientDisconnected += GameManager_OnOneClientDisconnected;
    }

    void Start()
    {
        panel.SetActive(false);
        stopWatchStringFormat = stopWatchText.text;
    }

    void Update()
    {
        if (!startStopWatch) return;

        stopWatchValue += Time.deltaTime;
        int min = Mathf.FloorToInt(stopWatchValue / 60);
        int sec = Mathf.FloorToInt(stopWatchValue) % 60;
        stopWatchText.text = string.Format(stopWatchStringFormat, min, sec);
    }

    private void GameManager_OnGameStarted(object _sender, EventArgs _event)
    {
        panel.SetActive(true);
        startStopWatch = true;
        stopWatchValue = 0;
        stopWatchText.text = string.Format(stopWatchStringFormat, 0, 0);
    }

    private void GameManager_OnWinConditionMet(object _sender, EventArgs _event)
    {
        startStopWatch = false;
    }

    private void GameManager_OnOneClientDisconnected(object _sender, EventArgs _event)
    {
        panel.SetActive(false);
        startStopWatch = false;
    }
}
