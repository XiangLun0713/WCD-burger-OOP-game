using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public enum Language
    {
        English,
        Chinese
    }

    [SerializeField] private MakeBurger burgerMaker;
    [SerializeField] private CustomerOrderGenerator customerOrderGenerator;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private AudioSource chachingSound;
    [SerializeField] private AudioSource countDownSound;
    [SerializeField] private AudioSource whistleSound;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI doneText;
    [SerializeField] private TextMeshProUGUI customerOrderTitleText;
    [SerializeField] private TextMeshProUGUI recipeText;
    [SerializeField] private TextMeshProUGUI recipeTitleText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Image recipeBackgroundImage;
    [SerializeField] private Button readyButton;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;

    public bool IsGameOver { get; private set; } = true;
    public static Language InGameLanguage { get; private set; } = Language.English;
    private int MoneyEarned { get; set; }
    private int DayPassed { get; set; }
    private int TargetForTheDay { get; set; }

    private const int GameDurationInSec = 15;
    private const int InitialTargetForTheDayVal = 300;
    private const int IncreaseInTargetValPerDay = 250;
    private float _timerTime;

    private void Start()
    {
        // get game language from main menu manager
        InGameLanguage = MainMenuManager.InGameLanguage;

        // set text language accordingly
        doneText.text = (InGameLanguage == Language.Chinese) ? "完成" : "DONE";
        customerOrderTitleText.text = (InGameLanguage == Language.Chinese) ? "顾客订单：" : "Customer's Order:";
        recipeTitleText.text = (InGameLanguage == Language.Chinese) ? "今日份菜谱" : "Recipe Of The Day";
        readyText.text = (InGameLanguage == Language.Chinese) ? "我准备好了" : "I'm Ready";

        // show recipe
        customerOrderGenerator.CreateRecipeOfTheDay();
        recipeText.gameObject.SetActive(true);
        recipeTitleText.gameObject.SetActive(true);
        recipeBackgroundImage.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);

        // set initial values
        TargetForTheDay = InitialTargetForTheDayVal;
        DayPassed = 0;
    }

    public void StartGame()
    {
        // hide the recipe
        recipeText.gameObject.SetActive(false);
        recipeTitleText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);
        readyButton.gameObject.SetActive(false);

        // start the game
        _timerTime = GameDurationInSec;
        TargetForTheDay = InitialTargetForTheDayVal + (DayPassed * IncreaseInTargetValPerDay);
        Debug.Log("Target: " + TargetForTheDay);
        IsGameOver = false;
        MoneyEarned = 0;
        moneyText.text = $"${MoneyEarned}";
        DayPassed++;
        dayText.text = (InGameLanguage == Language.English) ? $"Day {DayPassed}" : $"第{DayPassed}天";
        customerOrderGenerator.GenerateCustomerOrder();
        burgerMaker.SetAllButtonsInteractable(true);
        burgerMaker.CreateTray();
    }

    private void Update()
    {
        if (!IsGameOver)
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
                // if game over,
                GameOver();
            }
        }

        UpdateTimer();
    }

    private void GameOver()
    {
        IsGameOver = true;
        if (countDownSound.isPlaying) countDownSound.Stop();
        StartCoroutine(GameOverCoroutine());
    }

    private IEnumerator GameOverCoroutine()
    {
        whistleSound.Play();
        yield return new WaitWhile(() => whistleSound.isPlaying);

        burgerMaker.RemoveTray();

        if (MainMenuManager.IsStoryMode)
        {
            DialogueManager.DialogueState = DialogueManager.StoryState.End;
            SceneManager.LoadScene("Scenes/Story Scene");
        }
        else
        {
            // display result scene
            // todo show summary ui & proceed to show recipe after button pressed
            Debug.Log($"Day {DayPassed} passed!");
            Debug.Log($"Money earned: ${MoneyEarned}");

            // show recipe
            customerOrderGenerator.CreateRecipeOfTheDay();
            recipeText.gameObject.SetActive(true);
            recipeTitleText.gameObject.SetActive(true);
            recipeBackgroundImage.gameObject.SetActive(true);
            readyButton.gameObject.SetActive(true);
        }
    }

    private void UpdateTimer()
    {
        timerText.text = DisplayTimeAsString((int) Math.Ceiling(_timerTime));
    }

    private string DisplayTimeAsString(int sec)
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

    public void IncreaseMoneyEarned(int amount)
    {
        int originalVal = MoneyEarned;
        MoneyEarned = MoneyEarned + amount < 0 ? 0 : MoneyEarned + amount;
        StartCoroutine(MoneyTextChangeAnim(originalVal, MoneyEarned));
    }

    private IEnumerator MoneyTextChangeAnim(int originalVal, int newVal)
    {
        if (newVal >= originalVal)
        {
            for (int i = originalVal; i <= newVal; i++)
            {
                moneyText.text = $"${i}";
                yield return new WaitForSeconds(0.005f);
            }
        }
        else
        {
            for (int i = originalVal; i >= newVal; i--)
            {
                moneyText.text = $"${i}";
                yield return new WaitForSeconds(0.005f);
            }
        }
    }
}