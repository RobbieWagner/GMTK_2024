using RobbieWagnerGames.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleMusic : MonoBehaviour
{
    private void Awake()
    {
        DayNightCycle.Instance.OnDayCycleChange += TransitionMusic;

        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Day);
    }

    private void TransitionMusic(Daytime daytime)
    {
        if (daytime == Daytime.DUSK)
        {
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Night);
            //Stop Day music
            //Play stinger
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Day));
            
        }
        else if (daytime == Daytime.DAWN)
        {
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Day);
            //Stop Night music
            //Play stinger
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Night));
        }    
        else if(daytime == Daytime.DAY)
        {
            //play day music
        }
        else if (daytime == Daytime.NIGHT)
        {
            //play night music
        }
    }
}
