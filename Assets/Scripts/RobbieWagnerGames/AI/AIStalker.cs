using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.AI
{
    public class AIStalker : AIAgent
    {
        [SerializeField] private float maxChaseDistance = 15f; //TODO: use raycast visual instead
        protected void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
                ChaseNearestTarget();
        }

        protected override void UpdateChaseState()
        {
            base.UpdateChaseState();

            if(Vector3.Distance(chasingTarget.transform.position, transform.position) > maxChaseDistance)
                GoIdle();
        }
    }
}