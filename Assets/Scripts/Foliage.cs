using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foliage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("hi");
        if (other.CompareTag("Player"))
            StealthPlayer.Instance.ObscurePlayer();
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("hello");
        if (other.CompareTag("Player"))
            StealthPlayer.Instance.ResetPlayer();
    }
}
