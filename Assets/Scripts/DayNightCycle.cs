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
    private Vector3 rotation = Vector3.zero;
    private Vector3 totalRotation = Vector3.zero;
    public Daytime daytime;

    [SerializeField] private float DawnSeconds = 10f;
    [SerializeField] private float DawnDegrees = 30f;

    [SerializeField] private float DayTimeSeconds = 60f;
    [SerializeField] private float DayTimeDegrees = 150f;

    [SerializeField] private float DuskSeconds = 20f;
    [SerializeField] private float DuskDegrees = 30f;

    [SerializeField] private float NightSeconds = 120f;
    [SerializeField] private float NightDegrees = 150f;
    void Update()
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

    void CheckTime()
    {
        if (daytime != Daytime.DAWN && totalRotation.x < DawnDegrees)
        {
            daytime = Daytime.DAWN;
            OnDayCycleChange?.Invoke(daytime);
        } else if (daytime != Daytime.DAY && totalRotation.x >= DawnDegrees && totalRotation.x < DawnDegrees + DayTimeDegrees)
        {
            daytime = Daytime.DAY;
            OnDayCycleChange?.Invoke(daytime);
        } else if (daytime != Daytime.DUSK && totalRotation.x >= DawnDegrees + DayTimeDegrees && totalRotation.x < DawnDegrees + DayTimeDegrees + DuskDegrees)
        {
            daytime = Daytime.DUSK;
            OnDayCycleChange?.Invoke(daytime);
        } else if (daytime != Daytime.NIGHT && totalRotation.x >= DawnDegrees + DayTimeDegrees + DuskDegrees)
        {
            daytime = Daytime.NIGHT;
            OnDayCycleChange?.Invoke(daytime);
        }

        if (totalRotation.x > 360)
        {
            totalRotation.x = 0;
        }
    }

    void DawnUpdate()
    {
        float degrees_per_second = DawnDegrees / DawnSeconds;
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
        totalRotation.x += rotation.x;
    }

    void DayUpdate()
    {
        float degrees_per_second = DayTimeDegrees / DayTimeSeconds;
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
        totalRotation.x += rotation.x;
    }

    void DuskUpdate()
    {
        float degrees_per_second = DuskDegrees / DuskSeconds;
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
        totalRotation.x += rotation.x;
    }

    void NightUpdate()
    {
        float degrees_per_second = NightDegrees / NightSeconds;
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
        totalRotation.x += rotation.x;
    }

    public delegate void DaytimeDelegate(Daytime daytime);
    public event DaytimeDelegate OnDayCycleChange;
}
