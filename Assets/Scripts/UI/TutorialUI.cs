using DG.Tweening;
using GMTK2024;
using RobbieWagnerGames.AI;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class TutorialUI : MonoBehaviour
{
    [SerializeField] private TutorialText tutorialTextPrefab;
    [SerializeField] private Canvas canvas;

    private bool hasDisplayedAntTutorial = false;
    private bool hasDisplayedHideTutorial = false;

    public static TutorialUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
            Destroy(gameObject);
        else
            Instance = this;

        DayNightCycle.Instance.OnDayCycleChange += DisplayHide;
        GameManager.Instance.OnChangeCurrentAnthill += DisplayStomp;
    }

    public void DisplayStomp(Anthill anthill)
    {
        if (!hasDisplayedAntTutorial && anthill != null)
        {
            StartCoroutine(DisplayTutorialText("Stomp On Ants", new Vector2(-550, 85), new Vector2(-320, 85)));
            hasDisplayedAntTutorial = true;
        }
    }

    public void DisplayHide(Daytime daytime)
    {
        if (!hasDisplayedHideTutorial && daytime == Daytime.NIGHT)
        {
            StartCoroutine(DisplayTutorialText("Run Or Hide From Them", new Vector2(550, -100), new Vector2(320, -100)));
            hasDisplayedHideTutorial = true;
        }
    }


    public IEnumerator DisplayTutorialText(string text, Vector2 initialSpot, Vector2 tutorialSpot, float tutorialTime = 10)
    {
        TutorialText tutorialText = Instantiate(tutorialTextPrefab, canvas.transform);

        tutorialText.textUI.text = text;

        tutorialText.thisRectTransform.anchoredPosition = initialSpot;

        yield return tutorialText.thisRectTransform.DOAnchorPos(tutorialSpot, tutorialTime / 6).WaitForCompletion();
        yield return new WaitForSeconds(tutorialTime/2);
        yield return tutorialText.thisRectTransform.DOAnchorPos(initialSpot, tutorialTime / 3).WaitForCompletion();

        Destroy(tutorialText.gameObject);
    }
}
