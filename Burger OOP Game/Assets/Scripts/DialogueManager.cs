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

    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private TextMeshProUGUI skipText;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private AudioSource typingSound;
    [SerializeField] private AudioSource popSound;

    private const float EnglishTypingSpeed = .03f;
    private const float ChineseTypingSpeed = .08f;
    private readonly Queue<string> _dialogueQueue = new Queue<string>();

    private GameManager.Language InGameLanguage { get; set; } = GameManager.Language.English;
    public static StoryState DialogueState { get; set; } = StoryState.Start;

    private void Start()
    {
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
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/EN_StartDialogue.txt");
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/CN_StartDialogue.txt");
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
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/EN_EndDialogue.txt");
                continueText.text = "Continue >>";
                skipText.text = "Skip";
                break;
            case GameManager.Language.Chinese:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/CN_EndDialogue.txt");
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
        StartCoroutine(TypeOneDialogue());
    }

    private void ShowEndDialogue()
    {
        LoadEndDialogue();
        dialogueBox.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    private IEnumerator ProceedNextLine()
    {
        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);

        dialogueText.gameObject.SetActive(false);
        continueButton.SetActive(false);

        if (_dialogueQueue.Count <= 0)
        {
            // all story displayed
            dialogueBox.gameObject.SetActive(false);
            SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
        }
        else
        {
            StartCoroutine(TypeOneDialogue());
        }
    }

    public void OnContinueButtonPressed()
    {
        StartCoroutine(ProceedNextLine());
    }

    public void OnSkipButtonPressed()
    {
        StartCoroutine(SkipStory());
    }

    private IEnumerator SkipStory()
    {
        popSound.Play();
        yield return new WaitWhile(() => popSound.isPlaying);

        SceneManager.LoadScene(DialogueState == StoryState.Start ? "Scenes/Game Scene" : "Scenes/Main Menu");
    }
}