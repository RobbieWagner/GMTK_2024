using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    private Vector3 rotation = Vector3.zero;

    [SerializeField]
    private float degrees_per_second = 6;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        rotation.x = degrees_per_second * Time.deltaTime;
        transform.Rotate(rotation, Space.World);
    }
}
