using RobbieWagnerGames.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioTest : MonoBehaviour
{
    private void Awake()
    {
        StartCoroutine(Test());
    }

    private IEnumerator Test()
    {
        yield return null;
        Debug.Log("start test");

        Debug.Log("footsteps test");
        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Footstep_Walk);
        yield return new WaitForSeconds(5);
        BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Footstep_Walk);
        yield return null;
        BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Footstep_Walk);

        Debug.Log("sound test");
        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.AntSplat1);
        yield return new WaitForSeconds(2);
        BasicAudioManager.Instance.StopAudioSource(AudioSourceName.AntSplat1);

        Debug.Log("ambience test");
        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Ambience_Night);
        yield return new WaitForSeconds(5);
        StartCoroutine(BasicAudioManager.Instance.FadeSoundCo(AudioSourceName.Ambience_Night));

        Debug.Log("end test");
    }
}
