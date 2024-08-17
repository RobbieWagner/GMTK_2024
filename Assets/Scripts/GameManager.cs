using RobbieWagnerGames.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2024
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField] private AIStalker stalkerPrefab;
        [SerializeField] private List<Vector3> chaserSpawnLocations;

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
            //Place new anthills
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