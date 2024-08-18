using GMTK2024;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatMenu : MonoBehaviour
{
    [SerializeField] private StatText statTextPrefab;
    [SerializeField] private LayoutGroup layout;

    private void Awake()
    {
        foreach(KeyValuePair<string, int> stat in GameStatTracker.Instance.GetStats())
        {
            StatText newStatText = Instantiate(statTextPrefab, layout.transform);
            newStatText.stat.text = stat.Key.Replace("_", " ");
            newStatText.value.text = stat.Value.ToString();
        }
    }
}
