using System;
using System.Collections;
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
    [SerializeField] private AudioSource winSound;
    [SerializeField] private AudioSource gameOverSound;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private TextMeshProUGUI doneText;
    [SerializeField] private TextMeshProUGUI customerOrderTitleText;
    [SerializeField] private TextMeshProUGUI recipeText;
    [SerializeField] private TextMeshProUGUI recipeTitleText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private TextMeshProUGUI targetText;
    [SerializeField] private TextMeshProUGUI summaryTitleText;
    [SerializeField] private TextMeshProUGUI summaryTargetText;
    [SerializeField] private TextMeshProUGUI summaryMoneyText;
    [SerializeField] private TextMeshProUGUI summaryTotalText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI mainMenuText;
    [SerializeField] private TextMeshProUGUI newHighScoreText;
    [SerializeField] private Image targetLabel;
    [SerializeField] private Image moneyLabel;
    [SerializeField] private Image totalLabel;
    [SerializeField] private Image recipeBackgroundImage;
    [SerializeField] private Button continueButton;
    [SerializeField] private Button readyButton;
    [SerializeField] private Button mainMenuButton;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private ParticleSystem confetti;

    public TextMeshProUGUI dayText;
    public TextMeshProUGUI moneyText;

    public bool IsGameOver { get; private set; } = true;
    public static Language InGameLanguage { get; private set; } = Language.English;
    public static bool PlayedOneTime { get; private set; } = false;
    private int MoneyEarned { get; set; }
    private int DayPassed { get; set; }
    private int TargetForTheDay { get; set; }
    private int TotalMoneyEarned { get; set; }

    private const int GameDurationInSec = 150;
    private const int InitialTargetForTheDayVal = 300;
    private const int IncreaseInTargetValPerDay = 250;
    private float _timerTime;

    private void Start()
    {
        transitionAnimation.Play("Transition_Out");
        PlayedOneTime = true;

        // get game language from main menu manager
        InGameLanguage = MainMenuManager.InGameLanguage;

        // set text language accordingly
        doneText.text = (InGameLanguage == Language.Chinese) ? "完成" : "DONE";
        customerOrderTitleText.text = (InGameLanguage == Language.Chinese) ? "顾客订单：" : "Customer's Order:";
        recipeTitleText.text = (InGameLanguage == Language.Chinese) ? "今日份菜谱" : "Recipe Of The Day";
        readyText.text = (InGameLanguage == Language.Chinese) ? "我准备好了" : "I'm Ready";
        continueText.text = (InGameLanguage == Language.Chinese) ? "继续" : "Continue";
        mainMenuText.text = (InGameLanguage == Language.Chinese) ? "返回主页" : "Main Menu";
        newHighScoreText.text = (InGameLanguage == Language.Chinese) ? "新纪录！" : "New Highscore!";

        // show recipe
        customerOrderGenerator.CreateRecipeOfTheDay();
        recipeText.gameObject.SetActive(true);
        recipeTitleText.gameObject.SetActive(true);
        recipeBackgroundImage.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);

        // set initial values
        TargetForTheDay = InitialTargetForTheDayVal;
        DayPassed = 0;
        TotalMoneyEarned = 0;

        // hide target text if is story mode
        if (MainMenuManager.IsStoryMode) targetText.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        readyButton.gameObject.SetActive(false);
        StartCoroutine(StartGameCoroutine());
    }

    private IEnumerator StartGameCoroutine()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide the recipe
        recipeText.gameObject.SetActive(false);
        recipeTitleText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);

        transitionAnimation.Play("Transition_Out");

        // start the game
        _timerTime = GameDurationInSec;
        TargetForTheDay = InitialTargetForTheDayVal + (DayPassed * IncreaseInTargetValPerDay);
        targetText.text = ((InGameLanguage == Language.Chinese) ? "目标: $" : "Target: $") + TargetForTheDay;
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

        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        burgerMaker.RemoveTray();

        if (MainMenuManager.IsStoryMode)
        {
            DialogueManager.DialogueState = DialogueManager.StoryState.End;
            SceneManager.LoadScene("Scenes/Story Scene");
        }
        else
        {
            transitionAnimation.Play("Transition_Out");

            // display day summary
            recipeBackgroundImage.gameObject.SetActive(true);
            summaryTitleText.gameObject.SetActive(true);
            moneyLabel.gameObject.SetActive(true);
            totalLabel.gameObject.SetActive(true);
            targetLabel.gameObject.SetActive(true);

            TotalMoneyEarned += MoneyEarned;

            summaryTargetText.text = (InGameLanguage == Language.English)
                ? $"Today's Target: ${TargetForTheDay}"
                : $"今日の目标：${TargetForTheDay}";
            summaryMoneyText.text = (InGameLanguage == Language.English)
                ? $"Today's Profit: ${MoneyEarned}"
                : $"今日の收入：${MoneyEarned}";
            summaryTotalText.text = (InGameLanguage == Language.English)
                ? $"Total Profit: ${TotalMoneyEarned}"
                : $"总收入：${TotalMoneyEarned}";

            if (TotalMoneyEarned > PlayerPrefs.GetInt("highscore"))
            {
                newHighScoreText.gameObject.SetActive(true);
                PlayerPrefs.SetInt("highscore", TotalMoneyEarned);
            }

            if (MoneyEarned >= TargetForTheDay)
            {
                continueButton.gameObject.SetActive(true);

                summaryTitleText.text = (InGameLanguage == Language.English)
                    ? $"Day {DayPassed} Passed!"
                    : $"第{DayPassed}天·成功！";

                winSound.Play();
                confetti.Play();
            }
            else
            {
                mainMenuButton.gameObject.SetActive(true);

                summaryTitleText.text = (InGameLanguage == Language.English)
                    ? $"Day {DayPassed} Failed!"
                    : $"第{DayPassed}天·失败！";

                gameOverSound.Play();
            }
        }
    }

    public void OnContinueButtonPressed()
    {
        continueButton.gameObject.SetActive(false);
        StartCoroutine(ContinueCoroutine());
    }

    private IEnumerator ContinueCoroutine()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide summary UI
        newHighScoreText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);
        summaryTitleText.gameObject.SetActive(false);
        moneyLabel.gameObject.SetActive(false);
        totalLabel.gameObject.SetActive(false);
        targetLabel.gameObject.SetActive(false);

        // show recipe
        customerOrderGenerator.CreateRecipeOfTheDay();
        recipeText.gameObject.SetActive(true);
        recipeTitleText.gameObject.SetActive(true);
        recipeBackgroundImage.gameObject.SetActive(true);
        readyButton.gameObject.SetActive(true);

        transitionAnimation.Play("Transition_Out");
    }

    public void OnMainMenuButtonPressed()
    {
        mainMenuButton.gameObject.SetActive(false);
        StartCoroutine(GoToMainMenu());
    }

    private IEnumerator GoToMainMenu()
    {
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);

        // hide summary UI
        newHighScoreText.gameObject.SetActive(false);
        recipeBackgroundImage.gameObject.SetActive(false);
        summaryTitleText.gameObject.SetActive(false);
        targetLabel.gameObject.SetActive(false);
        moneyLabel.gameObject.SetActive(false);
        totalLabel.gameObject.SetActive(false);

        SceneManager.LoadScene("Scenes/Main Menu");
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