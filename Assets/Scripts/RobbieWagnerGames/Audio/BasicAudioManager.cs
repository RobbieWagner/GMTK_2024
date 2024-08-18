using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections;
using DG.Tweening;
using Ink.Parsed;
using System.Collections.Generic;
using System.Linq;

namespace RobbieWagnerGames.Common
{
    public enum AudioSourceName
    {
        Footstep_Walk,
        Footstep_Run,

        AntSplat1,  
        AntSplat2,
        AntSplat3,
        AntSplat4,
        AntSplat5,

        Flashlight,
        
        Ambience_Day,
        Ambience_Night
    }

    public class BasicAudioManager : MonoBehaviour
    {
        [SerializeField][SerializedDictionary("source type", "source")] private SerializedDictionary<AudioSourceName, AudioSource> audioSources;

        public static BasicAudioManager Instance {get; private set;}

        private void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }

        public AudioSource GetAudioSource(AudioSourceName name)
        {
            return audioSources.ContainsKey(name) ? audioSources[name] : null;
        }

        public void PlayAudioSource(AudioSourceName name, bool checkPlayStatus = false)
        {
            if(audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                audioSources[name].volume = 1;
                if(!checkPlayStatus || !audioSources[name].isPlaying)
                    audioSources[name].Play();
            }
        }

        public void StopAudioSource(AudioSourceName name)
        {
            if(audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                audioSources[name].Stop();
            }
        }

        public IEnumerator FadeSoundCo(AudioSourceName name, float fadeTime = 2f, bool fadeIn = false)
        {
            if (audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                yield return StartCoroutine(FadeSound(audioSources[name], fadeTime, fadeIn));
                AudioSource audioSource = audioSources[name];
                if (fadeIn && !audioSource.isPlaying)
                {
                    yield return audioSource.DOFade(1, fadeTime).WaitForCompletion();
                    audioSource.Play();
                }
            }
        }

        public static IEnumerator FadeSound(AudioSource source, float fadeTime = 2f, bool fadeIn = false)
        {
            if (fadeIn && !source.isPlaying)
            {
                source.volume = 0.0f;
                source.Play();
                yield return source.DOFade(1, fadeTime).WaitForCompletion();
            }

            else if(!fadeIn && source.isPlaying)
            {
                yield return source.DOFade(0, fadeTime).WaitForCompletion();
                source.Stop();
            }
        }

        public void PlayRandomSound(List<AudioSourceName> sources, bool stopPlayingSounds = false)
        {
            List<AudioSource> audioSources = sources.Select(x => GetAudioSource(x)).Where(x => x != null).ToList();
            if (audioSources == null)
            {
                Debug.LogWarning("Could not play random audio source: No Audio Sources found!");
                return;
            }

            List<AudioSource> validSources = stopPlayingSounds ? audioSources : audioSources.Where(x => !x.isPlaying).ToList();

            validSources[UnityEngine.Random.Range(0, validSources.Count)].Play();
        }
    }
}