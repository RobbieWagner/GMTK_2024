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
        }

        private IEnumerator CooldownAlertTimer(float length)
        {
            yield return new WaitForSeconds(length);
            playingAlertedSound = null;
            alertSoundCooldown = null;
        }

        protected void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
                ChaseNearestTarget();
        }

        public override bool ChaseNearestTarget()
        {
            if(base.ChaseNearestTarget())
            {
                //if(alertSound != null && !alertSound.isPlaying)
                //    alertSound.Play();
                return true;
            }
            return false;
        }

        protected override void UpdateChaseState()
        {
            base.UpdateChaseState();

            if(Vector3.Distance(chasingTarget.transform.position, transform.position) > maxChaseDistance)
                GoIdle();
        }

        public override void MoveToRandomSpot(float range = 100f)
        {
            StartCoroutine(StalkPlayer(SimpleFirstPersonPlayerMovement.Instance.transform.position, minStalkDistance, maxStalkDistance));
        }

        public virtual IEnumerator StalkPlayer(Vector3 offset, float minRange = 75f, float maxRange = 200f, int tryLimit = 10000, int triesBeforeYield = 25)
        {
            int tries = 0;
            bool success = false;
            while (tries < tryLimit)
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
                        MoveAgent(hit.position);
                        success = true;
                        yield break;
                    }
                }
            }

            if (!success)
                Debug.LogWarning($"Could not find a path after trying {tryLimit} times!");
        }
    }
}