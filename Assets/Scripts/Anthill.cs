using System;
using System.Collections;
using System.Collections.Generic;
using GMTK2024;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    [SerializeField] private float scorePerAnt = 100f;
    [SerializeField] private float antSpawnCooldown = 0.5f;

    private bool playerInRange = false;
    private bool isCoroutineRunning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !isCoroutineRunning)
        {
            playerInRange = true;
            StartCoroutine(GiveScore());
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
        }
    }

    IEnumerator GiveScore()
    {
        isCoroutineRunning = true;
        while (playerInRange)
        {
            GameManager.Instance.currentScore += scorePerAnt;
            yield return new WaitForSeconds(antSpawnCooldown);
        }
        isCoroutineRunning = false;
    }
}
