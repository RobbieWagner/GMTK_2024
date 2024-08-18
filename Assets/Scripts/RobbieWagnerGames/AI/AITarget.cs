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

        public virtual void OnCaught(AIAgent agent)
        {
            Debug.Log($"{agent.gameObject.name} caught {gameObject.name}");
            if(deathCo == null)
                deathCo = StartCoroutine(Die());
        }

        public virtual IEnumerator Die()
        {
            BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Death);
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn(0.1f));
            yield return new WaitForSeconds(3.5f);
            SceneManager.LoadScene("DeathScene");
        }
    }
}