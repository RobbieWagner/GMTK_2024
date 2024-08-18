using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

namespace RobbieWagnerGames.UI
{
    public class VolumeSlider : MonoBehaviour
    {
        public Slider volumeSlider;
        private float volume;
        [SerializeField] private float defaultValue = -5f;
        [SerializeField] private string exposedParameterName;
        [SerializeField] private TextMeshProUGUI volumeText;
        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private PauseMenu pauseMenu;

        private void Awake()
        {
            volumeSlider.onValueChanged.AddListener(AdjustVolume);
            volumeSlider.minValue = -41;
            volumeSlider.maxValue = 20;
            LoadVolume();
            volumeText.text = exposedParameterName;
        }

        public void AdjustVolume(float value)
        {
            if (value < -40) value = -80;
                volume = value;
            
            SaveVolume(volume);
            LoadVolume();
        }

        public void SaveVolume()
        {
            PlayerPrefs.SetFloat(exposedParameterName, volume);
        }

        public void SaveVolume(float value)
        {
            PlayerPrefs.SetFloat(exposedParameterName, value);
        }

        public void LoadVolume(bool ignorePauseMenu = false)
        {
            volume = PlayerPrefs.GetFloat(exposedParameterName, defaultValue);
            if (volume < -40) 
                volume = -80;
            
            if(pauseMenu == null || ignorePauseMenu)
                audioMixer.SetFloat(exposedParameterName, volume);
            volumeSlider.value = volume;
        }
    }
}