using AYellowpaper.SerializedCollections;
using DG.Tweening;
using GMTK2024;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUI : MonoBehaviour
{
    [Header("Score")]
    private int displayedScore = 0;
    [SerializeField] private TextMeshProUGUI scoreText;
    private Coroutine scoreTextUpdateCo;

    [Header("Day Cycle UI")]
    [SerializeField] private Slider dayCycleSlider; // Add a day cycle bar showing time of day? (circular slider)
    [SerializeField] private Image daySliderImage;
    [SerializeField] private Image daySliderFill;
    [SerializeField][SerializedDictionary("Day Cycle", "UI Image")] private SerializedDictionary<Daytime, Sprite> daytimeSprites;
    [SerializeField][SerializedDictionary("Day Cycle", "Color")] private SerializedDictionary<Daytime, Color> daytimeColors;

    private Coroutine transitionCo;
    private bool initialized = false;

    [Header("Stamina")]
    [SerializeField] private Slider staminaSlider;

    public static GameUI Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        GameManager.Instance.OnScoreChange += UpdateScoreText;
        DayNightCycle.Instance.OnUpdateDaytimeValue += UpdateDayCycleSlider;
        DayNightCycle.Instance.OnDayCycleChange += DayCycleTransitionHandler;

        scoreText.text = displayedScore.ToString();
        dayCycleSlider.minValue = 0;
        dayCycleSlider.maxValue = 360;
        dayCycleSlider.value = DayNightCycle.Instance.DaytimeValue;
    }

    private void DayCycleTransitionHandler(Daytime daytime)
    {
        if (transitionCo == null && initialized)
            transitionCo = StartCoroutine(DayCycleTransitionHandlerCo(daytime));
        else
        {
            daySliderFill.color = daytimeColors[daytime];
            daySliderImage.sprite = daytimeSprites[daytime];
            initialized = true;
        }

    }

    private IEnumerator DayCycleTransitionHandlerCo(Daytime daytime, float transitionTime = 2f)
    {
        StartCoroutine(FadeBarColor(daytime, transitionTime));

        yield return daySliderImage.DOColor(Color.black, transitionTime/2).WaitForCompletion();
        daySliderImage.sprite = daytimeSprites[daytime];
        yield return daySliderImage.DOColor(Color.white, transitionTime / 2).WaitForCompletion();

        transitionCo = null;
    }

    private IEnumerator FadeBarColor(Daytime daytime, float transitionTime = 2f)
    {
        yield return daySliderFill.DOColor(daytimeColors[daytime], transitionTime).WaitForCompletion();
    }

    private void UpdateScoreText(int score)
    {
        if (scoreTextUpdateCo == null)
            scoreTextUpdateCo = StartCoroutine(UpdateScoreTextCo());
    }

    private void UpdateDayCycleSlider(float time)
    {
        dayCycleSlider.value = time % dayCycleSlider.maxValue;
    }

    private IEnumerator UpdateScoreTextCo()
    {
        
        while(displayedScore < GameManager.Instance.CurrentScore)
        {
            displayedScore++;
            scoreText.text = displayedScore.ToString();
            yield return new WaitForSeconds(.05f);
        }
        scoreTextUpdateCo = null;
    }
}
