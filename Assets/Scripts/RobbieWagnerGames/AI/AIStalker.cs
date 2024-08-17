using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RobbieWagnerGames.AI
{
    public class AIStalker : AIAgent
    {
        protected void OnTriggerEnter(Collider other)
        {
            if(other.CompareTag("Player"))
            {
                ChaseNearestTarget();
            }
        }
    }
}