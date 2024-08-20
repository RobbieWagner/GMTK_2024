using GMTK2024;
using RobbieWagnerGames.Common;
using RobbieWagnerGames.Minijam164;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.AI
{
    public class AITarget : MonoBehaviour
    {
        private Coroutine deathCo;

        //[HideInInspector] 
        public HashSet<AIAgent> chasers = new HashSet<AIAgent>();

        public virtual void OnCaught(AIAgent agent)
        {
            Debug.Log($"{agent.gameObject.name} caught {gameObject.name}");
            if(deathCo == null)
                deathCo = StartCoroutine(Die());
        }

        public virtual IEnumerator Die()
        {
            GameStatTracker.Instance?.SetStat(GameStatistic.Deaths, 1, true);
            GameStatTracker.Instance?.SetStat(GameStatistic.Total_Ants_Killed, GameManager.Instance.CurrentScore, true);
            if(GameManager.Instance.CurrentScore > GameStatTracker.Instance.GetStat(GameStatistic.Highscore))
                GameStatTracker.Instance?.SetStat(GameStatistic.Highscore, GameManager.Instance.CurrentScore);

            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Death);
            
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Ambience_Night);
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Ambience_Day);
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Music_Night);
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Music_Day);
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Stinger_Dawn);
            BasicAudioManager.Instance.StopAudioSource(AudioSourceName.Stinger_Dusk);

            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn(0.1f));
            yield return new WaitForSeconds(3.5f);
            SceneManager.LoadScene("DeathScene");
        }
    }
}