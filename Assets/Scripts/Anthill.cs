using System;
using System.Collections;
using System.Collections.Generic;
using GMTK2024;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    [SerializeField] private int scorePerAnt = 10;
    [SerializeField] private float antSpawnCooldown = 1f;

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
            GameManager.Instance.CurrentScore += scorePerAnt;
            yield return new WaitForSeconds(antSpawnCooldown);
        }
        isCoroutineRunning = false;
    }
}
