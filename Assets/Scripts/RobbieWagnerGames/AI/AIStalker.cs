using RobbieWagnerGames.Common;
using RobbieWagnerGames.FirstPerson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

namespace RobbieWagnerGames.AI
{
    public class AIStalker : AIAgent
    {
        [SerializeField] private float maxChaseDistance = 15f; //TODO: use raycast visual instead
        //[SerializeField] private AudioSource footstepSounds;
        [SerializeField] private List<AudioSource> alertedSounds;
        private static AudioSource playingAlertedSound;
        private Coroutine alertSoundCooldown;

        [SerializeField] private float minStalkDistance = 75f;
        [SerializeField] private float maxStalkDistance = 200f;
        [SerializeField] private float sightCheckCooldown = 0.5f;
        
        private float timer;
        private float moveTimer;

        [HideInInspector] public bool isInPlayerRange;

        [SerializeField] private float runSpeed = 6.7f;
        [SerializeField] private float walkSpeed = 6.7f;
        [SerializeField] private float searchSpeed = 6.7f;

        protected override void Awake()
        {
            base.Awake();

            OnStateChange += HandleStateChange;
            
        }

        protected override void OnReachTarget(AITarget target)
        {
            base.OnReachTarget(target);
            footstepSounds.volume = 0;
        }

        private void HandleStateChange(AIState state)
        {
            if(footstepSounds != null)
            {
                if (state != AIState.IDLE && !footstepSounds.isPlaying)
                    footstepSounds.Play();
                else if(state == AIState.IDLE)
                    footstepSounds.Stop();
            }

            if(alertedSounds != null && alertedSounds.Any())
            {
                if (state == AIState.CHASING && !playingAlertedSound)
                {
                    playingAlertedSound = BasicAudioManager.PlayRandomSound(alertedSounds);
                    if(playingAlertedSound != null)
                        alertSoundCooldown = StartCoroutine(CooldownAlertTimer(playingAlertedSound.clip.length));
                }
                if (state != AIState.CHASING && alertSoundCooldown != null)
                {
                    StartCoroutine(BasicAudioManager.FadeSound(playingAlertedSound));
                    playingAlertedSound = null;
                    alertSoundCooldown = null;
                }
            }

            if (state == AIState.CHASING)
                agent.speed = runSpeed;
            else if (state == AIState.SEARCHING)
                agent.speed = searchSpeed;
            else
                agent.speed = walkSpeed;
        }

        private IEnumerator CooldownAlertTimer(float length)
        {
            yield return new WaitForSeconds(length);
            playingAlertedSound = null;
            alertSoundCooldown = null;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Player"))
                isInPlayerRange = true;
        }

        protected virtual void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player"))
                isInPlayerRange = false;
        }

        protected override void UpdateSearchState()
        {
            base.UpdateSearchState();

            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up,
                    chasingTarget.transform.position - (transform.position + transform.up), out hit, 200f, raycastLayers))
            {
                if (hit.collider.CompareTag("Player") || isInPlayerRange)
                {
                    ChaseNearestTarget();
                }
            }

            if (agent.destination == null || chasingTarget == null || AIManager.GetPathLength(agent.path) < .05f)
            {
                GoIdle();
            }
        }

        protected void OnTriggerStay(Collider other)
        {
            if (other.CompareTag("Player"))
            {
                timer += Time.deltaTime;
                if (timer > sightCheckCooldown)
                {
                    RaycastHit hit;
                    Debug.DrawRay(transform.position + transform.up, (other.transform.position - (transform.position + transform.up)).normalized * 60, Color.blue, 0.5f);
                    if (Physics.Raycast(transform.position + transform.up, other.transform.position - (transform.position + transform.up), out hit, 60, raycastLayers))
                    {
                        if (hit.transform.gameObject.CompareTag("Player") && CurrentState != AIState.CHASING)
                        {
                            timer = 0;
                            //Debug.Log("Found Player: " + hit.transform.gameObject);
                            ChaseNearestTarget();
                        }
                        else
                        {
                            //Debug.Log("Found: " + hit.transform.gameObject);
                        }
                    }
                }
            }
        }

        public override bool ChaseNearestTarget()
        {
            if(base.ChaseNearestTarget())
            {
                chasingTarget.chasers.Add(this);
                
                return true;
            }
            return false;
        }

        protected override void UpdateMovingState()
        {
            if (agent != null)
            {
                moveTimer += Time.deltaTime;
                if (((agent.destination == null || AIManager.GetPathLength(agent.path) < 2.5f) && moveTimer >= 5f) || moveTimer > 60)
                {
                    moveTimer = 0;
                    GoIdle();
                }
            }
        }

        protected override void UpdateChaseState()
        {
            base.UpdateChaseState();

            RaycastHit hit;
            if (Physics.Raycast(transform.position + transform.up,
                    chasingTarget.transform.position - (transform.position + transform.up), out hit, 30.1f, raycastLayers))
            {
                if (hit.transform == null || hit.transform.gameObject != chasingTarget.gameObject)
                {
                    currentState = AIState.SEARCHING;
                }
            }
            else
            {
                CurrentState = AIState.SEARCHING;
            }
        }

        public override void MoveToRandomSpot(float range = 100f)
        {
            StartCoroutine(StalkPlayer(SimpleFirstPersonPlayerMovement.Instance.transform.position, minStalkDistance, maxStalkDistance));
        }

        public virtual IEnumerator StalkPlayer(Vector3 offset, float minRange = 75f, float maxRange = 200f, int tryLimit = 10000, int triesBeforeYield = 25)
        {
            int tries = 0;
            bool success = false;
            while (tries < tryLimit && !success)
            {
                tries++;
                if (tries % triesBeforeYield == 0)
                    yield return null;

                Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * maxRange; // transform.position?
                randomDirection += offset;
                NavMeshHit hit;
                if (NavMesh.SamplePosition(randomDirection, out hit, maxRange, NavMesh.AllAreas))
                {
                    if(Vector3.Distance(SimpleFirstPersonPlayerMovement.Instance.transform.position, hit.position) >= minRange)
                    {
                        success = MoveAgent(hit.position);
                    }
                }
            }

            if (!success)
                Debug.LogWarning($"Could not find a path after trying {tryLimit} times!");
        }

        public override bool MoveAgent(Vector3 destination)
        {
            if (agent != null)
            {
                CurrentState = AIState.MOVING;
                return SetDestination(destination);
            }
            return false;
        }
    }
}