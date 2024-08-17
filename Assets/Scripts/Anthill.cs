using System;
using System.Collections;
using System.Collections.Generic;
using GMTK2024;
using RobbieWagnerGames.FirstPerson;
using UnityEngine;

public class Anthill : MonoBehaviour
{
    [SerializeField] private int scorePerAnt = 10;
    [SerializeField] private float antSpawnCooldown = 1f;

    private float timer = 0f;
    
    private bool playerInRange = false;
    private bool isCoroutineRunning = false;

    private SimpleFirstPersonPlayerMovement playerMovement;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerMovement = other.GetComponent<SimpleFirstPersonPlayerMovement>();
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Player") && playerMovement.IsMoving)
        {
            timer += Time.deltaTime;
            if (timer > antSpawnCooldown)
            {
                GameManager.Instance.CurrentScore += scorePerAnt;
                timer = 0f;
            }
        }
    }
}
