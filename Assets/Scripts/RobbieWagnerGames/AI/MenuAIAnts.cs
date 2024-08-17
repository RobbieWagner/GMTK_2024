using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace RobbieWagnerGames.AI
{
    public class MenuAIAnts : MonoBehaviour
    {
        [SerializeField] private AIAgent testAgentPrefab;
        private List<AIAgent> menuAgents = new List<AIAgent>();
        [SerializeField] private List<Vector3> spawnLocations;

        private void Awake()
        {
            foreach (Vector3 spawnLocation in spawnLocations)
            {
                AIAgent agent = AIManager.Instance.AddAgentToScene(testAgentPrefab, spawnLocation, new List<AITarget>());
                agent.idleWaitTime = UnityEngine.Random.Range(1f, 5f);
                menuAgents.Add(agent);

            }

            AIManager.Instance.InitializeAI();
        }
    }
}