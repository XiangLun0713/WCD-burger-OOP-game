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
    [SerializeField] private AudioSource backgroundMusic;
    [SerializeField] private Button ChinaButton;
    [SerializeField] private TextMeshProUGUI ChinaButtonText;
    [SerializeField] private Button UKButton;
    [SerializeField] private TextMeshProUGUI UKButtonText;
    [SerializeField] private TextMeshProUGUI storyModeText;
    [SerializeField] private TextMeshProUGUI endlessModeText;
    [SerializeField] private Animator transitionAnimation;

    public static GameManager.Language InGameLanguage { get; private set; }
    public static bool IsStoryMode { get; private set; }
    private Image _chinaImage;
    private Image _ukImage;

    private void Start()
    {
        if (GameManager.PlayedOneTime) transitionAnimation.Play("Transition_Out");

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
        StartCoroutine(DecreaseBackgroundMusicVolume());
        StartCoroutine(PlayStoryModeCoroutine());
    }

    private IEnumerator PlayStoryModeCoroutine()
    {
        IsStoryMode = true;
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        DialogueManager.DialogueState = DialogueManager.StoryState.Start;
        SceneManager.LoadScene("Scenes/Story Scene");
    }

    public void PlayEndlessMode()
    {
        StartCoroutine(DecreaseBackgroundMusicVolume());
        StartCoroutine(PlayEndlessModeCoroutine());
    }

    private IEnumerator PlayEndlessModeCoroutine()
    {
        IsStoryMode = false;
        transitionAnimation.Play("Transition_In");
        yield return new WaitForSeconds(40.0f / 60);
        SceneManager.LoadScene("Scenes/Game Scene");
    }

    private IEnumerator DecreaseBackgroundMusicVolume()
    {
        float initialVolume = backgroundMusic.volume;
        for (float i = initialVolume; i >= 0; i -= initialVolume / 20)
        {
            backgroundMusic.volume = i;
            yield return new WaitForSeconds(.02f);
        }

        backgroundMusic.volume = 0;
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