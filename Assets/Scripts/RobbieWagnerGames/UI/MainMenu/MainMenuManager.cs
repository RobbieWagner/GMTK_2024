using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Audio;

namespace RobbieWagnerGames.UI
{
    public class MainMenuManager : Menu
    {
        [SerializeField] private Button startButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button creditsButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private string sceneToGoTo;

        [SerializeField] private MenuWithTabs settingsMenu;
        [SerializeField] private Canvas controls;
        [SerializeField] private Canvas credits;

        [SerializeField] private AudioMixer audioMixer;

        protected override void Awake()
        {
            base.Awake();
            Cursor.lockState = CursorLockMode.None;

            StartCoroutine(SetVolumes());
        }

        private IEnumerator SetVolumes()
        {
            yield return null;

            audioMixer.SetFloat("Main", PlayerPrefs.GetFloat("Main", -5f));
            audioMixer.SetFloat("Player", PlayerPrefs.GetFloat("Player", -5f));
            audioMixer.SetFloat("UI", PlayerPrefs.GetFloat("UI", -5f));
            audioMixer.SetFloat("Music", PlayerPrefs.GetFloat("Music", -5f));
        }

        protected override void OnEnable()
        {
            startButton.onClick.AddListener(StartGame);
            settingsButton?.onClick.AddListener(OpenSettings);
            //controlsButton.onClick.AddListener(OpenControls);
            //creditsButton.onClick.AddListener(OpenCredits);
            quitButton?.onClick.AddListener(QuitGame);

            base.OnEnable();
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            startButton.onClick.RemoveListener(StartGame);
            settingsButton?.onClick.RemoveListener(OpenSettings);
            //controlsButton.onClick.RemoveListener(OpenControls);
            //creditsButton.onClick.RemoveListener(OpenCredits);
            quitButton?.onClick.RemoveListener(QuitGame);
        }

        public void StartGame()
        {
            ToggleButtonInteractibility(false);

            SceneManager.LoadScene(sceneToGoTo);
        }

        private void OpenSettings()
        {
            thisCanvas.enabled = false;
            settingsMenu.enabled = true;
        }

        private void OpenControls()
        {
            StartCoroutine(SwapCanvases(thisCanvas, controls));
        }

        private void OpenCredits()
        {
            StartCoroutine(SwapCanvases(thisCanvas, credits));
        }

        private void QuitGame()
        {
            ToggleButtonInteractibility(false);

            //save any new save data
            Application.Quit();
        }

        protected override void ToggleButtonInteractibility(bool toggleOn)
        {
            base.ToggleButtonInteractibility(toggleOn);

            startButton.interactable = toggleOn;
            if(settingsButton != null) 
                settingsButton.interactable = toggleOn;
            //controlsButton.interactable = toggleOn;
            //creditsButton.interactable = toggleOn;
            if (quitButton != null)
                quitButton.interactable = toggleOn;
        }

        protected override IEnumerator SwapCanvases(Canvas active, Canvas next)
        {
            
            yield return StartCoroutine(base.SwapCanvases(active, next));

            StopCoroutine(SwapCanvases(active, next));
        }
    }
}