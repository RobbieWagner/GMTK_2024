using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.AI
{
    public class AIStalker : AIAgent
    {
        [SerializeField] private float maxChaseDistance = 15f; //TODO: use raycast visual instead
        [SerializeField] private AudioSource footstepSounds;
        [SerializeField] private AudioSource alertSound;

        protected override void Awake()
        {
            base.Awake();

            OnStateChange += HandleStateChange;
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

            if(alertSound != null)
            {
                if (state == AIState.CHASING && !alertSound.isPlaying)
                    alertSound.Play();
            }
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