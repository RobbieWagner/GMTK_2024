using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GMTK2024
{
    public enum GameStatistic
    {
        Highscore,
        Total_Ants_Killed,
        Deaths,

        Anthills_Destroyed,
        Flashlight_Clicks,
        Days_Survived
    }

    public class GameStatTracker : MonoBehaviour
    {
        public static GameStatTracker Instance { get; private set; }

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

            //if (GameUI.Instance != null)
            //    GameUI.Instance.highscoreText.text = GetStat(GameStatistic.Highscore).ToString();
        }

        public Dictionary<string, int> GetStats()
        {
            Dictionary<string, int> output = new Dictionary<string, int>();

            foreach (GameStatistic stat in Enum.GetValues(typeof(GameStatistic)))
                output.Add(stat.ToString(), PlayerPrefs.GetInt(stat.ToString(), 0));
            
            return output;
        }

        public void SetStat(GameStatistic stat, int value, bool additive = false)
        {
            if (additive)
                PlayerPrefs.SetInt(stat.ToString(), PlayerPrefs.GetInt(stat.ToString(), 0) + value);
            else
                PlayerPrefs.SetInt(stat.ToString(), value);
        }

        public int GetStat(GameStatistic stat)
        {
            return PlayerPrefs.GetInt(stat.ToString(), 0);
        }
    }
}
