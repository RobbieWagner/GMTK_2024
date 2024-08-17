using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Foliage : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            StealthPlayer.Instance.ObscurePlayer();
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            StealthPlayer.Instance.ResetPlayer();
    }
}
