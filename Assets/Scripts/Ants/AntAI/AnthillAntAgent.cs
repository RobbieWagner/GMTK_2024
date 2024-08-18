using RobbieWagnerGames.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AnthillAntAgent : AIAgent
{
    public Anthill anthill = null;

    public override void MoveToRandomSpot(float range = 100)
    {
        if(anthill != null)
            StartCoroutine(MoveToRandomSpotCo(anthill.transform.position, anthill.scoreRadius, 10000));
        else
            base.MoveToRandomSpot(range);
    }
}
