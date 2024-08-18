using AYellowpaper.SerializedCollections;
using DG.Tweening;
using GMTK2024;
using RobbieWagnerGames.FirstPerson;
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
    [SerializeField] private Slider scoreGainSlider;
    private Anthill currentTrackedAnthill = null;

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
    [SerializeField] private Image staminaSliderImage;
    [SerializeField] private Image staminaFillImage;
    [SerializeField] private Color normalStaminaColor;
    [SerializeField] private Color badStaminaColor;

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

        staminaSlider.minValue = 0;
        staminaSlider.maxValue = SimpleFirstPersonPlayerMovement.Instance.maxStamina;
        staminaSlider.value = SimpleFirstPersonPlayerMovement.Instance.CurStamina;
        SimpleFirstPersonPlayerMovement.Instance.OnStaminaChanged += UpdateStaminaSlider;
        staminaFillImage.color = normalStaminaColor;

        scoreGainSlider.gameObject.SetActive(false);

        GameManager.Instance.OnChangeCurrentAnthill += UpdateCurrentAnthill;
    }

    private void UpdateCurrentAnthill(Anthill ah)
    {
        if(currentTrackedAnthill != null)
        {
            UnsubscribeAnthill();
        }
        currentTrackedAnthill = ah;
        if(currentTrackedAnthill != null)
        {
            SubscribeAnthill();
        }
    }

    private void SubscribeAnthill()
    {
        scoreGainSlider?.gameObject.SetActive(true);
        currentTrackedAnthill.OnStompTimeUpdated += UpdateCurrentAnthill;

        scoreGainSlider.maxValue = currentTrackedAnthill.antSpawnCooldown;
    }

    private void UnsubscribeAnthill()
    {
        scoreGainSlider?.gameObject.SetActive(false);
        currentTrackedAnthill.OnStompTimeUpdated -= UpdateCurrentAnthill;
    }

    private void UpdateCurrentAnthill(float time)
    {
        if(currentTrackedAnthill != null)
            scoreGainSlider.value = currentTrackedAnthill.timer;
    }

    private void Update()
    {
        if(staminaSlider.value == staminaSlider.maxValue)
            staminaSlider.gameObject.SetActive(false);
        else
            staminaSlider.gameObject.SetActive(true);
    }

    private void UpdateStaminaSlider(float floatVal)
    {
        staminaSlider.value = floatVal;

        if (floatVal < staminaSlider.maxValue / 5)
            staminaFillImage.color = badStaminaColor;
        else
            staminaFillImage.color = normalStaminaColor;

        if (floatVal <= 0)
            StartCoroutine(FlashImageColor(staminaSliderImage, Color.white));
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

        yield return daySliderImage.DOColor(Color.black, transitionTime / 2).WaitForCompletion();
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

        while (displayedScore < GameManager.Instance.CurrentScore)
        {
            displayedScore++;
            scoreText.text = displayedScore.ToString();
            yield return new WaitForSeconds(.05f);
        }
        scoreTextUpdateCo = null;
    }

    private IEnumerator FlashImageColor(Image image, Color flashColor, float flashTime = .5f, int flashes = 3)
    {
        Color initialColor = image.color;
        for(int i = 0; i < flashes; i++)
        {
            yield return image.DOColor(flashColor, flashTime).WaitForCompletion();
            yield return image.DOColor(initialColor, flashTime).WaitForCompletion();
        }
    }
}
