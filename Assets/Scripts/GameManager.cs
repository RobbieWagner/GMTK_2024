using RobbieWagnerGames.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2024
{
    public class GameManager : MonoBehaviour
    {
        [Header("AI Chasers")]
        [SerializeField] private AIStalker stalkerPrefab;
        [SerializeField] private List<Vector3> chaserSpawnLocations;

        [Header("Points")]
        [SerializeField] private List<Vector3> anthillSpawnLocations;
        [SerializeField] private Anthill anthillPrefab;
        [SerializeField] private Transform anthillParent;

        private int currentScore = 0;
        public int CurrentScore
        {
            get 
            {
                return currentScore;
            }
            set 
            {
                if(currentScore == value || value < 0 || value > 99999999)
                    return;
                currentScore = value;
                OnScoreChange?.Invoke(currentScore);
            }
        }
        public delegate void ScoreDelegate(int score);
        public event ScoreDelegate OnScoreChange;

        [Header("Player")]
        [SerializeField] private List<AITarget> player;

        public static GameManager Instance { get; private set; }
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

            DayNightCycle.Instance.OnDayCycleChange += TriggerNextDayTimeCycle;
        }

        private void TriggerNextDayTimeCycle(Daytime daytime)
        {
            switch (daytime) 
            {
                case Daytime.DAWN:
                    TriggerDawn();
                    break;
                case Daytime.DAY:
                    TriggerDay();
                    break;
                case Daytime.DUSK:
                    TriggerDusk();
                    break;
                case Daytime.NIGHT:
                    TriggerNight();
                    break;
                default:
                    break;
            }
        }

        private void TriggerDawn()
        {
            AIManager.Instance.DestroyAllAgents();
        }

        private void TriggerDay()
        {
            // Place new anthills
            // Clear Anthills here (foreach Destroy(anthill))? , then place new ones in new locations
            foreach (Vector3 spawnLocation in anthillSpawnLocations)
            {
                Anthill anthill = Instantiate(anthillPrefab, anthillParent);
                anthill.transform.position = spawnLocation;
            }
        }

        private void TriggerDusk()
        {
            //Do something
        }

        private void TriggerNight()
        {
            foreach (Vector3 spawnLocation in chaserSpawnLocations)
                AIManager.Instance.AddAgentToScene(stalkerPrefab, spawnLocation, player);

            AIManager.Instance.InitializeAI();
        }
    }
}