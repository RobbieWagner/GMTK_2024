using RobbieWagnerGames.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2024
{
    public class TestAIAnts : MonoBehaviour
    {
        [SerializeField] private AIStalker stalkerPrefab;
        [SerializeField] private List<Vector3> spawnLocations;
        [SerializeField] private List<AITarget> player;

        private void Awake()
        {
            foreach (Vector3 spawnLocation in spawnLocations)
                AIManager.Instance.AddAgentToScene(stalkerPrefab, spawnLocation, player);

            AIManager.Instance.InitializeAI();
        }
    }
}