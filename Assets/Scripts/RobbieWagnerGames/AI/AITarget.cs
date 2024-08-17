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
            yield return new WaitForSeconds(2f);
            SceneManager.LoadScene("FPS");
        }
    }
}