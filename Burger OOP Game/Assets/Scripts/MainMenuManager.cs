using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private AudioSource popSound;
    [SerializeField] private Button ChinaButton;
    [SerializeField] private TextMeshProUGUI ChinaButtonText;
    [SerializeField] private Button UKButton;
    [SerializeField] private TextMeshProUGUI UKButtonText;
    [SerializeField] private TextMeshProUGUI storyModeText;
    [SerializeField] private TextMeshProUGUI endlessModeText;

    public static GameManager.Language InGameLanguage { get; private set; }
    public static bool IsStoryMode { get; private set; }
    private Image _chinaImage;
    private Image _ukImage;

    private void Start()
    {
        InGameLanguage = GameManager.InGameLanguage;
        _chinaImage = ChinaButton.GetComponent<Image>();
        _ukImage = UKButton.GetComponent<Image>();
        switch (InGameLanguage)
        {
            case GameManager.Language.English:
                ChangeToEnglish();
                break;
            case GameManager.Language.Chinese:
                ChangeToChinese();
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public void PlayStoryMode()
    {
        IsStoryMode = true;
        DialogueManager.DialogueState = DialogueManager.StoryState.Start;
        SceneManager.LoadScene("Scenes/Story Scene");
    }

    public void PlayEndlessMode()
    {
        IsStoryMode = false;
        SceneManager.LoadScene("Scenes/Game Scene");
    }

    public void ChangeToEnglish()
    {
        InGameLanguage = GameManager.Language.English;

        ChinaButtonText.text = "ZH";
        UKButtonText.text = "EN";
        storyModeText.text = "Story Mode";
        endlessModeText.text = "Endless Mode";

        if (_chinaImage != null && _ukImage != null)
        {
            Color tempColor = _chinaImage.color;
            tempColor.a = .4f;
            _chinaImage.color = tempColor;

            tempColor = _ukImage.color;
            tempColor.a = 1f;
            _ukImage.color = tempColor;
        }
    }

    public void ChangeToChinese()
    {
        InGameLanguage = GameManager.Language.Chinese;

        ChinaButtonText.text = "中";
        UKButtonText.text = "英";
        storyModeText.text = "故事模式";
        endlessModeText.text = "无尽模式";

        if (_chinaImage != null && _ukImage != null)
        {
            Color tempColor = _chinaImage.color;
            tempColor.a = 1f;
            _chinaImage.color = tempColor;

            tempColor = _ukImage.color;
            tempColor.a = .4f;
            _ukImage.color = tempColor;
        }
    }

    public void PlayPopSound()
    {
        popSound.Play();
    }
}