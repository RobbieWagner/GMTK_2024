using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Rendering.CameraUI;

namespace GMTK2024
{
    public enum GameStatistic
    {
        Highscore,
        Total_Ants_Killed,
        Deaths,

        Anthills_Destroyed,
        Flashlight_Clicks,
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


        }

        public Dictionary<string, int> GetStats()
        {
            Dictionary<string, int> output = new Dictionary<string, int>();

            output.Add(GameStatistic.Highscore.ToString(), PlayerPrefs.GetInt(GameStatistic.Highscore.ToString(), 0));
            output.Add(GameStatistic.Total_Ants_Killed.ToString(), PlayerPrefs.GetInt(GameStatistic.Total_Ants_Killed.ToString(), 0));
            output.Add(GameStatistic.Deaths.ToString(), PlayerPrefs.GetInt(GameStatistic.Deaths.ToString(), 0));
            output.Add(GameStatistic.Anthills_Destroyed.ToString(), PlayerPrefs.GetInt(GameStatistic.Anthills_Destroyed.ToString(), 0));
            output.Add(GameStatistic.Flashlight_Clicks.ToString(), PlayerPrefs.GetInt(GameStatistic.Flashlight_Clicks.ToString(), 0));

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
