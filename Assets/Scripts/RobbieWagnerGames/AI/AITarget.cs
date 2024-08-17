using RobbieWagnerGames.Minijam164;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace RobbieWagnerGames.AI
{
    public class AITarget : MonoBehaviour
    {
        public virtual void OnCaught(AIAgent agent)
        {
            Debug.Log($"{agent.gameObject.name} caught {gameObject.name}");
            StartCoroutine(Die());
        }

        public virtual IEnumerator Die()
        {
            yield return StartCoroutine(ScreenCover.Instance.FadeCoverIn(2));
            SceneManager.LoadScene("DeathScene");
        }
    }
}