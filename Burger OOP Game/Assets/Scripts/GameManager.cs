using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Chinese
    }

    [SerializeField] private DialogueManager dialogueManager;
    [SerializeField] private MakeBurger burgerMaker;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private AudioSource chachingSound;
    [SerializeField] private AudioSource countDownSound;
    [SerializeField] private AudioSource whistleSound;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI doneText;
    [SerializeField] private TextMeshProUGUI customerOrderTitleText;


    private Language _language = Language.English;
    public Language InGameLanguage => _language;
    private const int GameDurationInSec = 150;
    private float _timerTime;
    public bool IsGameOver { get; private set; }

    private void Start()
    {
        _language = MainMenuManager.InGameLanguage;
        doneText.text = (_language == Language.Chinese) ? "完成" : "DONE";
        customerOrderTitleText.text = (_language == Language.Chinese) ? "顾客订单：" : "Customer's Order:";
        // dialogueManager.ShowStartDialogue();
        StartGame(); // after this need to call this in dialogue manager
    }

    private void StartGame()
    {
        IsGameOver = false;
        _timerTime = GameDurationInSec;
        burgerMaker.CreateTray();
    }

    private void Update()
    {
        if (_timerTime > 0)
        {
            _timerTime -= Time.deltaTime;
            if (_timerTime <= 10)
            {
                if (!countDownSound.isPlaying) countDownSound.Play();
            }
        }
        else
        {
            // game over
            if (!IsGameOver)
            {
                IsGameOver = true;
                whistleSound.Play();
                if (countDownSound.isPlaying) countDownSound.Stop();
            }
        }

        UpdateTimer();
    }

    private void UpdateTimer()
    {
        timerText.text = displayTimeAsString((int) Math.Ceiling(_timerTime));
    }

    private String displayTimeAsString(int sec)
    {
        return $"{sec / 60}:{sec % 60:D2}";
    }

    public void PlayChachingSound()
    {
        chachingSound.Play();
    }

    public void PlayPopSound()
    {
        popSound.Play();
    }
}