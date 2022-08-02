using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI dialogueText;
    [SerializeField] private TextMeshProUGUI continueText;
    [SerializeField] private GameObject continueButton;
    [SerializeField] private Image dialogueBox;
    [SerializeField] private AudioSource typingSound;
    [SerializeField] private AudioSource popSound;
    [SerializeField] private GameManager gameManager;

    private const float EnglishTypingSpeed = .03f;
    private const float ChineseTypingSpeed = .08f;
    private readonly Queue<string> _dialogueQueue = new Queue<string>();

    private void LoadStartDialogue()
    {
        string[] lines = { };
        switch (gameManager.InGameLanguage)
        {
            case GameManager.Language.English:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/EN_StartDialogue.txt");
                continueText.text = "Continue >>";
                break;
            case GameManager.Language.Chinese:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/CN_StartDialogue.txt");
                continueText.text = "继续 》";
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
        switch (gameManager.InGameLanguage)
        {
            case GameManager.Language.English:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/EN_EndDialogue.txt");
                continueText.text = "Continue >>";
                break;
            case GameManager.Language.Chinese:
                lines = System.IO.File.ReadAllLines("Assets/Dialogues/CN_EndDialogue.txt");
                continueText.text = "继续 》";
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
                yield return (gameManager.InGameLanguage == GameManager.Language.English)
                    ? new WaitForSeconds(EnglishTypingSpeed)
                    : new WaitForSeconds(ChineseTypingSpeed);
            }

            continueButton.SetActive(true);
        }

        typingSound.Pause();
    }

    public void ShowStartDialogue()
    {
        LoadStartDialogue();
        dialogueBox.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    public void ShowEndDialogue()
    {
        LoadEndDialogue();
        dialogueBox.gameObject.SetActive(true);
        StartCoroutine(TypeOneDialogue());
    }

    public void ProceedNextLine()
    {
        popSound.Play();
        dialogueText.gameObject.SetActive(false);
        continueButton.SetActive(false);

        if (_dialogueQueue.Count <= 0)
        {
            dialogueBox.gameObject.SetActive(false);
        }
        else
        {
            StartCoroutine(TypeOneDialogue());
        }
    }
}