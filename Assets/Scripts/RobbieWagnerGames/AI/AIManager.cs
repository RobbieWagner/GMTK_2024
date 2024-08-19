using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.AI
{
    public class AIManager : MonoBehaviour
    {
        public static AIManager Instance { get; private set; }

        public List<AIAgent> activeAgents = new List<AIAgent>();

        private void Awake()
        {
            if (Instance != null && Instance != this)
                Destroy(gameObject);
            else
                Instance = this;
        }

        public void InitializeAI()
        {
            if(activeAgents.Any())
            {
                foreach(AIAgent agent in activeAgents) 
                    agent?.GoIdle();
            }
        }

        public AIAgent AddAgentToScene(AIAgent agentPrefab, Vector3 startingPos, List<AITarget> initialTargets, Transform parent = null)
        {
            AIAgent agent;
            if (parent != null)
                agent = Instantiate(agentPrefab, parent);
            else
                agent = Instantiate(agentPrefab, startingPos, Quaternion.identity);

            agent.SetTargets(initialTargets);
            activeAgents.Add(agent);
            //Debug.Log("Passed Position " + agent.transform.position);
            return agent;
        }

        public void DestroyAgent(AIAgent agent)
        {
            activeAgents.Remove(agent);
            Destroy(agent);
        }

        public void DestroyAllAgents()
        {
            foreach (AIAgent agent in activeAgents)
            {
                if(agent != null)
                    Destroy(agent.gameObject);
            }
            activeAgents.Clear();
        }

        public static float GetPathLength(NavMeshPath path)
        {
            float length = 0.0f;

            if (path.corners.Length < 2)
                return length;

            for (int i = 0; i < path.corners.Length - 1; i++)
            {
                length += Vector3.Distance(path.corners[i], path.corners[i + 1]);
            }

            return length;
        }
    }
}