using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    public enum StoryState
    {
        Start,
        End
    }

    private readonly string[] _cnStartDialogue =
    {
        "紧急情况！听见警报声响起，你火速赶往事发现场。",
        "“发生什么事了，老板？？”",
        "原来是 WcD 总部刚更新了它们的食谱系统，之前做汉堡的方法已经不再管用了。",
        "也就是说，现在厨房里没人会用这个新系统啊！！",
        "老板把滑鼠交到你的手上：“如今，就全靠你来拯救我们的未来了！”"
    };

    private readonly string[] _cnEndDialogue =
    {
        "“我在餐饮业混了十余年，招聘过无数厨师，但像你这么有天赋的，我还是头一次见！”老板感动得热泪盈眶。",
        "“其实……老板，你有没有想过，你可能是招错地方了。”",
        "当然，为了保住饭碗，你没有将这句话说出来。"
    };

    private readonly string[] _enStartDialogue =
    {
        "EMERGENCY! Hearing the alarm sound, you rush to the scene of the incident.",
        "\"What happened, boss?\"",
        "It turns out that the WcD headquarter has just updated their recipe system, so the previous method of making burgers no longer works.",
        "This means that no one in the kitchen knows how to use this new system now!!",
        "The boss hands the mouse over to you, \"Now it's all up to you to save our future!\"",
    };

    private readonly string[] _enEndDialogue =
    {
        "\"I have been in the food industry for over ten years and have recruited countless chefs; still, this is the first time I've seen someone as talented as you!\" The boss was moved to tears.",
        "\"Actually ... boss, have you ever thought that you might be recruiting in the wrong field?\"",
        "Of course, you didn't say this out loud to keep your job.",
    };

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private AudioSource typingSound;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private Animator transitionAnimation;
    [SerializeField] private Button skipButton;

    private const float EnglishTypingSpeed = .03f;
    private const float ChineseTypingSpeed = .08f;
    private readonly Queue<string> _dialogueQueue = new Queue<string>();

    private GameManager.Language InGameLanguage { get; set; } = GameManager.Language.English;
    public static StoryState DialogueState { get; set; } = StoryState.Start;

    private void Start()
    {
        transitionAnimation.Play("Transition_Out");

        InGameLanguage = MainMenuManager.InGameLanguage;

        dialogueBox.gameObject.SetActive(false);
        continueButton.gameObject.SetActive(false);
        dialogueText.gameObject.SetActive(false);

        switch (DialogueState)
        {
            case StoryState.Start:
                ShowStartDialogue();
                break;
            case StoryState.End:
                ShowEndDialogue();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void LoadStartDialogue()
    {
        string[] lines = { };
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                lines = _enStartDialogue;
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = _cnStartDialogue;
                continueText.text = "继续 》";
                skipText.text = "跳过";
                break;
        }

        foreach (var line in lines)
        {
            _dialogueQueue.Enqueue(line);
        }
    }

    private void LoadEndDialogue()
    {
        string[] lines = { };
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                lines = _enEndDialogue;
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = _cnEndDialogue;
                continueText.text = "继续 》";
                skipText.text = "跳过";
                break;
        }

        foreach (var line in lines)
        {
            _dialogueQueue.Enqueue(line);
        }
    }

    private IEnumerator TypeOneDialogue()
    {
        dialogueText.text = string.Empty;
        dialogueText.gameObject.SetActive(true);

        if (typingSound.time != 0)
        {
            typingSound.UnPause();
        }
        else
        {
            typingSound.Play();
        }

        if (_dialogueQueue.Count > 0)
        {
            string line = _dialogueQueue.Dequeue();
            foreach (var letter in line)
            {
                dialogueText.text += letter;
                yield return (InGameLanguage == GameManager.Language.English)
                    ? new WaitForSeconds(EnglishTypingSpeed)
                    : new WaitForSeconds(ChineseTypingSpeed);
            }

            continueButton.SetActive(true);
        }

        typingSound.Pause();
    }

    private void ShowStartDialogue()
    {
        LoadStartDialogue();
        dialogueBox.gameObject.SetActive(true);
        skipButton.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    private void ShowEndDialogue()
    {
        LoadEndDialogue();
        skipButton.gameObject.SetActive(true);
        dialogueBox.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    private IEnumerator ProceedNextLine()
    {
        continueButton.SetActive(false);

        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);

        dialogueText.gameObject.SetActive(false);

        if (_dialogueQueue.Count <= 0)
        {
            // all story displayed
            StartCoroutine(SwitchScene());
        }
        else
        {
            StartCoroutine(TypeOneDialogue());
        }
    }

    private IEnumerator SwitchScene()
    {
        dialogueBox.gameObject.SetActive(false);
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
    }

    public void OnContinueButtonPressed()
    {
        StartCoroutine(ProceedNextLine());
    }

    public void OnSkipButtonPressed()
    {
        skipButton.gameObject.SetActive(false);
        StartCoroutine(SkipStory());
    }

    private IEnumerator SkipStory()
    {
        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
    }
}