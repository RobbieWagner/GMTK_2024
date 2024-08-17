using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Vector3 rotation = Vector3.zero;

    [SerializeField]
    private float degrees_per_second = 6;

    // Update is called once per frame
    void Update()
    {
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
    }

    public bool IsDayTime()
    {
        return rotation.x >= 0 && rotation.x < 180;
    }
}
