using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum Daytime
{
    NONE = 0,
    DAWN = 1,
    DAY = 2,
    DUSK = 3,
    NIGHT = 4
}

public class DayNightCycle : MonoBehaviour
{
    [SerializeField] private float initialDaytimeValue = 30;
    [HideInInspector] public Vector3 totalRotation;
    private float daytimeValue = 0;
    public float DaytimeValue
    {
        get 
        {
            return daytimeValue;
        }
        set 
        {
            if(daytimeValue == value)
                return;
            daytimeValue = value;
            //Debug.Log($"{daytimeValue}");
            OnUpdateDaytimeValue?.Invoke(daytimeValue);
        }
    }
    public delegate void DaytimeValueDelegate(float time);
    public event DaytimeValueDelegate OnUpdateDaytimeValue;

    private Daytime daytime = Daytime.NONE;
    public Daytime Daytime 
    { 
        get 
        { 
            return daytime; 
        } 
        set 
        {
            if(daytime == value)
                return;
            daytime = value;
            //Debug.Log($"{daytime}");
            OnDayCycleChange?.Invoke(daytime);
        }
    }

    [Header("Dawn")]
    [SerializeField] private float DawnSeconds = 10f;
    [SerializeField] private float DawnDegrees = 30f;

    [Header("Day")]
    [SerializeField] private float DayTimeSeconds = 60f;
    [SerializeField] private float DayTimeDegrees = 150f;

    [Header("Dusk")]
    [SerializeField] private float DuskSeconds = 20f;
    [SerializeField] private float DuskDegrees = 30f;

    [Header("Night")]
    [SerializeField] private float NightSeconds = 120f;
    [SerializeField] private float NightDegrees = 150f;

    private Transform sunlight;
    private Transform moonlight;

    public static DayNightCycle Instance { get; private set; }

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

        DaytimeValue = initialDaytimeValue;
        totalRotation = new Vector3(initialDaytimeValue, 0, 0);
        transform.rotation = Quaternion.Euler(totalRotation);

        sunlight = transform.Find("Sunlight");
        moonlight = transform.Find("Moonlight");
    }

    private void Update()
    {
        switch (daytime)
        {
            case Daytime.DAWN:
                DawnUpdate();
                break;
            case Daytime.DAY:
                DayUpdate();
                break;
            case Daytime.DUSK:
                DuskUpdate();
                break;
            case Daytime.NIGHT:
                NightUpdate();
                break;
        }
        
        CheckTime();

        // if daisy-chain
        // when set, do OnDayCycleChange?.Invoke(daytime);
    }

    private void CheckTime()
    {
        if (daytime != Daytime.DAWN && totalRotation.x < DawnDegrees)
        {
            Daytime = Daytime.DAWN;
        } 
        else if (daytime != Daytime.DAY && totalRotation.x >= DawnDegrees && totalRotation.x < DawnDegrees + DayTimeDegrees)
        {
            Daytime = Daytime.DAY;
        } 
        else if (daytime != Daytime.DUSK && totalRotation.x >= DawnDegrees + DayTimeDegrees && totalRotation.x < DawnDegrees + DayTimeDegrees + DuskDegrees)
        {
            Daytime = Daytime.DUSK;
        } 
        else if (daytime != Daytime.NIGHT && totalRotation.x >= DawnDegrees + DayTimeDegrees + DuskDegrees)
        {
            Daytime = Daytime.NIGHT;
        }

        if (totalRotation.x > 360)
        {
            totalRotation.x = 0;
        }
    }

    void DawnUpdate()
    {
        UpdateDayTimeCycle(DawnDegrees / DawnSeconds);
    }

    void DayUpdate()
    {
        UpdateDayTimeCycle(DayTimeDegrees / DayTimeSeconds);
    }

    void DuskUpdate()
    {
        UpdateDayTimeCycle(DuskDegrees / DuskSeconds);
    }

    void NightUpdate()
    {
        UpdateDayTimeCycle(NightDegrees / NightSeconds);
    }

    private void UpdateDayTimeCycle(float rotationSpeed)
    {
        float delta = rotationSpeed * Time.deltaTime;
        DaytimeValue += delta;

        Vector3 rotationVector = delta * Vector3.right;
        sunlight.Rotate(rotationVector, Space.World);
        moonlight.Rotate(rotationVector, Space.World);
        totalRotation.x += rotationVector.x;
    }

    public delegate void DaytimeDelegate(Daytime daytime);
    public event DaytimeDelegate OnDayCycleChange;
}
