using RobbieWagnerGames.AI;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

namespace GMTK2024
{
    
    // TODO: Destroy antills and shuffle them on daytime
    // Anthill spawn location restrictions:
    // Cannot be within certain range of player
    // Cannot be too many in range of each other
    
    public class GameManager : MonoBehaviour
    {
        [Header("AI Chasers")]
        [SerializeField] private AIStalker stalkerPrefab;
        [SerializeField] private List<Vector3> chaserSpawnLocations;

        [Header("Points")]
        [SerializeField] private List<Vector3> anthillSpawnLocations;
        [SerializeField] private Anthill anthillPrefab;
        [SerializeField] private Transform anthillParent;

        [Header("Spawning")] 
        [SerializeField] private GameObject spawnPointsParent;
        [SerializeField] private float playerExclusionRange;
        [SerializeField] private float otherSpawnsExclusionRange;
        private List<Vector3> allSpawnLocations = new List<Vector3>();
        private List<Anthill> activeAnthills = new List<Anthill>();

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

            foreach (Transform child in spawnPointsParent.transform)
            {
                allSpawnLocations.Add(child.position);
            }
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
            DespawnAnthills();
            SpawnAnthills();
        }

        private void TriggerDusk()
        {
            //Do something
        }

        private void TriggerNight()
        {
            SpawnAnts();
        }

        private void SpawnAnthills()
        {
            // get valid positions relative to player
            List<Vector3> validSpawns = allSpawnLocations.Where(x =>
                Vector3.Distance(player[0].transform.position, x) > playerExclusionRange).ToList();
            
            // get valid positions relative to other anthills - ?
            
            // pick 5 random from remaining list
            validSpawns = validSpawns.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            // spawn anthills at each point and store references in array to be destroyed later
            foreach (Vector3 spawnPos in validSpawns)
            {
                Anthill anthill = Instantiate(anthillPrefab, anthillParent);
                anthill.transform.position = spawnPos;
                activeAnthills.Add(anthill);
            }
        }

        private void DespawnAnthills()
        {
            foreach (Anthill anthill in activeAnthills)
            {
                Destroy(anthill);
            }
            activeAnthills.Clear();
        }

        private void SpawnAnts()
        {
            // get valid positions relative to player
            List<Vector3> validSpawns = allSpawnLocations.Where(x =>
                Vector3.Distance(player[0].transform.position, x) > playerExclusionRange).ToList();
            
            // get valid positions relative to other ants - ?
            
            // pick 5 random from remaining list
            validSpawns = validSpawns.OrderBy(x => Guid.NewGuid()).Take(5).ToList();

            // spawn ants at each point
            foreach (Vector3 spawnPos in validSpawns)
            {
                AIManager.Instance.AddAgentToScene(stalkerPrefab, spawnPos, player);
            }
            AIManager.Instance.InitializeAI();
        }
    }
}