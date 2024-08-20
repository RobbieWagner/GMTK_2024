using UnityEngine;
using AYellowpaper.SerializedCollections;
using System.Collections;
using DG.Tweening;
using Ink.Parsed;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.ProBuilder.MeshOperations;
using Unity.VisualScripting;

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
        Ambience_Night,
        Music_Day,
        Music_Night,
        Stinger_Dawn,
        Stinger_Dusk,
        
        Death,
        AnthillDestroy,

        UIButton,
        UICancel
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

        public void PlayAudioSource(AudioSourceName name, bool checkPlayStatus = false, float volume = 1)
        {
            if(audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                audioSources[name].volume = volume;
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

        public IEnumerator FadeSoundCo(AudioSourceName name, float fadeTime = 2f, bool fadeIn = false, float vol = 1)
        {
            if (audioSources.ContainsKey(name) && audioSources[name] != null)
            {
                yield return StartCoroutine(FadeSound(audioSources[name], fadeTime, fadeIn, vol));
            }
        }

        public static IEnumerator FadeSound(AudioSource source, float fadeTime = 2f, bool fadeIn = false, float volume = 1)
        {
            if (fadeIn && !source.isPlaying)
            {
                source.volume = 0.0f;
                source.Play();
                yield return source.DOFade(volume, fadeTime).WaitForCompletion();
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

        public static AudioSource PlayRandomSound(List<AudioSource> audioSources, bool stopPlayingSounds = false)
        {
            if (audioSources == null)
            {
                Debug.LogWarning("Could not play random audio source: No Audio Sources found!");
                return null;
            }

            List<AudioSource> validSources = stopPlayingSounds ? audioSources : audioSources.Where(x => !x.isPlaying).ToList();

            if (validSources.Any())
            {
                AudioSource playingSource = validSources[UnityEngine.Random.Range(0, validSources.Count)];

                playingSource.Play();

                return playingSource;

            }

            return null;
        }

        public IEnumerator DelayAudioPlay(AudioSourceName audio, float delay, float vol = 1f)
        {
            yield return new WaitForSeconds(delay);
            PlayAudioSource(audio, false, vol);
        }
    }
}