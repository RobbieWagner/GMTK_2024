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
    public Daytime daytime;

    [SerializeField] private float DayTimeSeconds = 90f;
    [SerializeField] private float DayTimeDegrees = 150f;

    [SerializeField]
    private float degrees_per_second = 6;

    // Update is called once per frame
    void Update()
    {
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);

        // if daisy-chain
        // when set, do OnDayCycleChange?.Invoke(daytime);
    }

    public delegate void DaytimeDelegate(Daytime daytime);
    public event DaytimeDelegate OnDayCycleChange;

    public bool IsDayTime()
    {
        return rotation.x >= 0 && rotation.x < 180;
    }
}
