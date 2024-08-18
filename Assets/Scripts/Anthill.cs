using System;
using System.Collections;
using System.Collections.Generic;
using GMTK2024;
using RobbieWagnerGames.AI;
using RobbieWagnerGames.Common;
using RobbieWagnerGames.FirstPerson;
using UnityEngine;

// TODO: Anthill only gives out certain amount of points before it is destroyed

public class Anthill : MonoBehaviour
{
    [SerializeField] private int scorePerAnt = 10;
    [SerializeField] private float antSpawnCooldown = .75f;
    [SerializeField] private int maxAnts = 10;

    public float scoreRadius;
    [SerializeField] private CapsuleCollider capsuleCollider;

    private int antsSquashed = 0;

    private float timer = 0f;

    private SimpleFirstPersonPlayerMovement playerMovement;

    [SerializeField] private AnthillAntAgent antPrefab;
    [SerializeField] private Vector2Int antsToSpawn;
    private List<AnthillAntAgent> spawnedAgents = new List<AnthillAntAgent>();
    [SerializeField] private List<Vector3> antSpawnLocations;

    private List<AudioSourceName> splatSounds;

    private void Awake()
    {
        StartCoroutine(Init());
    }

    private IEnumerator Init()
    {
        yield return null;

        splatSounds = new List<AudioSourceName>()
        {
            AudioSourceName.AntSplat1,
            AudioSourceName.AntSplat2,
            AudioSourceName.AntSplat3,
            AudioSourceName.AntSplat4,
            AudioSourceName.AntSplat5,
        };

        capsuleCollider.radius = scoreRadius;

        for (int i = 0; i < UnityEngine.Random.Range(antsToSpawn.x, antsToSpawn.y); i++)
        {
            Vector3 pos = transform.position + antSpawnLocations[i];
            AnthillAntAgent agent = AIManager.Instance.AddAgentToScene(antPrefab, pos, null, transform) as AnthillAntAgent;
            agent.anthill = this;
            agent.transform.position = pos;
            spawnedAgents.Add(agent);
        }

        foreach (AIAgent agent in spawnedAgents)
        {
            agent.GoIdle();
        }
    }

    private void Update()
    {
        if (antsSquashed >= maxAnts)
        {
            Destroy(gameObject);
        }
    }

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
                antsSquashed++;
                GameManager.Instance.CurrentScore += scorePerAnt;
                timer = 0f;
                BasicAudioManager.Instance.PlayRandomSound(splatSounds);
            }
        }
    }
}
