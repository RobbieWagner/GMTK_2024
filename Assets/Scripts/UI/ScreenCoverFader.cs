using RobbieWagnerGames.Minijam164;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenCoverFader : MonoBehaviour
{
    [SerializeField] private bool fadeIn = false;

    private void Awake()
    {
        StartCoroutine(Fade());
    }

    private IEnumerator Fade()
    {
        yield return null;
        if(fadeIn)
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn(2));
        else
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverOut(2));
    }
}
