using RobbieWagnerGames.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayCycleMusic : MonoBehaviour
{
    [SerializeField] private float dawnStingerLength = 15f;
    [SerializeField] private float duskStingerLength = 15f;

    private void Awake()
    {
        DayNightCycle.Instance.OnDayCycleChange += TransitionMusic;

        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Day);
        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Music_Day, false, .25f);
    }

    private void TransitionMusic(Daytime daytime)
    {
        if (daytime == Daytime.DUSK)
        {
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Day));
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Music_Day));
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Night, 2, true));
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Stinger_Dusk);
            StartCoroutine(BasicAudioManager.Instance.DelayAudioPlay(AudioSourceName.Music_Night, duskStingerLength));
        }
        else if (daytime == Daytime.DAWN)
        {
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Night));
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Music_Night));
            StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Day, 2, true));
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Stinger_Dawn);
            StartCoroutine(BasicAudioManager.Instance.DelayAudioPlay(AudioSourceName.Music_Day, dawnStingerLength, .25f));
        }    
        else if(daytime == Daytime.DAY)
        {
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Music_Day, true, .25f);
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Day, true);
        }
        else if (daytime == Daytime.NIGHT)
        {
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Music_Night, true);
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Night, true);
        }
    }
}
