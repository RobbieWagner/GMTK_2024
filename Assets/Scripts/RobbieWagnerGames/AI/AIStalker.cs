using RobbieWagnerGames.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace RobbieWagnerGames.AI
{
    public class AIStalker : AIAgent
    {
        [SerializeField] private float maxChaseDistance = 15f; //TODO: use raycast visual instead
        //[SerializeField] private AudioSource footstepSounds;
        [SerializeField] private List<AudioSource> alertedSounds;
        private static AudioSource playingAlertedSound;
        private Coroutine alertSoundCooldown;

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
    }
}