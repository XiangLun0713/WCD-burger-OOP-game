using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonScaleBehaviour : MonoBehaviour
{
    private RectTransform _rectTransform;
    private Button _button;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _button = GetComponent<Button>();
    }

    public void ScaleUpButton()
    {
        if (_rectTransform != null && _button != null && _button.interactable) _rectTransform.localScale = new Vector3(1.1f, 1.1f, 1.1f);
    }

    public void ScaleDownButton()
    {
        if (_rectTransform != null && _button != null && _button.interactable) _rectTransform.localScale = new Vector3(1f, 1f, 1f);
    }
}