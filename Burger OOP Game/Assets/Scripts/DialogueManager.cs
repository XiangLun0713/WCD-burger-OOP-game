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
        "“我准备好了！我准备好了！我准备好了！”",
        "这是你在 WcD 快餐店兼职的第一天。为了要检验你这位新员工的能力，老板特别派你到厨房去制作汉堡包。",
        "抵达厨房后，你发现它和你想象中的场景……似乎不太一致。",
        "面对着充满 Java 代码的显示屏以及统一穿着格子衬衫的员工们，你不禁感叹：“这……我真的是在厨房吗？？”",
        "话是这么说，但工作还是得继续做。"
    };

    private readonly string[] _cnEndDialogue =
    {
        "恭喜你！你已经是一名优秀的程序员……啊不，优秀的汉堡厨师了！",
        "老板见你迅速地融入了工作氛围中，更是对你赞叹不已：“原本我还担心你之前程序员的身份让你思维转换不过来，现在看来，我是白担心了。”",
        "……你本来想说点什么，但想想还是算了。"
    };

    private readonly string[] _enStartDialogue =
    {
        "\"I'm ready! I'm ready! I'm ready!\"",
        "It's your first day working part-time at the WcD fast food restaurant.",
        "In order to test your abilities, your boss has specially sent you to the kitchen to make hamburgers.",
        "Upon arrival in the kitchen, you find that it does not seem to be quite the scene you imagined.",
        "Faced with displays full of Java code and an entire staff in plaid shirts, you wonder, \"Am I ... really in the kitchen?\"",
        "That being said, the work still needs to be done."
    };

    private readonly string[] _enEndDialogue =
    {
        "Congratulations! You are already an excellent programmer - I mean, an excellent burger chef!",
        "The boss is impressed to see how quickly you've fitted into the working environment.",
        "\"I was concerned that you might face difficulties changing your mindset due to your previous role as a programmer, but now it seems that I was worried about nothing.\"",
        "... You were about to say something, but you thought better of it.",
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