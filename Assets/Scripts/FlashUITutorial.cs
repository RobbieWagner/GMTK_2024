using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FlashUITutorial : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI flashText;

    private void Awake()
    {
        StartCoroutine(FlashText());


    }

    private IEnumerator FlashText()
    {
        yield return flashText.rectTransform.DOAnchorPos(Vector2.zero, 1).WaitForCompletion();
        yield return new WaitForSeconds(2);
        yield return flashText.rectTransform.DOAnchorPos(new Vector2(0, -500), 1).WaitForCompletion();
    }
}
