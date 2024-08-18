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
    public class PauseMenu : Menu
    {

        [SerializeField] private PlayerInput playerInput;
        [SerializeField] List<string> actionMapsToDisable;

        [SerializeField] private Button resumeButton;
        [SerializeField] private Button settingsButton;
        [SerializeField] private Button controlsButton;
        [SerializeField] private Button saveButton;
        [SerializeField] private Button mainMenuButton;
        [SerializeField] private Button quitButton;

        [SerializeField] private Canvas settings;
        [SerializeField] private Canvas controls;

        [HideInInspector] public bool canPause;
        [HideInInspector] public bool paused;

        [SerializeField] private AudioMixer audioMixer;

        [SerializeField] private List<VolumeSlider> volumeSliders;

        public static PauseMenu Instance {get; private set;}

        protected override void Awake()
        {
            if (Instance != null && Instance != this) 
            { 
                Destroy(gameObject); 
            } 
            else 
            { 
                Instance = this; 
            } 

            canPause = true;
            paused = false;
        } 

        protected override void OnEnable()
    {
            base.OnEnable();

            paused = true;
            Time.timeScale = 0;

            foreach(string actionMapName in actionMapsToDisable)
            {
                if(!string.IsNullOrEmpty(actionMapName))
                {
                    playerInput.actions.FindActionMap(actionMapName).Disable();
                } 
            }

            resumeButton.onClick.AddListener(ResumeGame);
            settingsButton.onClick.AddListener(OpenSettings);
            //controlsButton.onClick.AddListener(OpenControls);
            //saveButton.onClick.AddListener(SaveGame);
            mainMenuButton.onClick.AddListener(QuitToMainMenu);
            quitButton.onClick.AddListener(QuitToDesktop);

            thisCanvas.enabled = true;

            OnGamePaused?.Invoke();
            audioMixer.SetFloat("Main", -80f);
        }

        public delegate void OnGamePausedDelegate();
        public event OnGamePausedDelegate OnGamePaused;

        protected override void OnDisable()
        {
            base.OnDisable();

            if(!paused) 
            {
                Time.timeScale = 1;
                foreach(string actionMapName in actionMapsToDisable)
                {
                    if(!string.IsNullOrEmpty(actionMapName))
                    {
                        playerInput.actions.FindActionMap(actionMapName).Enable();
                    } 
                }

                foreach (VolumeSlider slider in volumeSliders)
                    slider.LoadVolume(true);

                OnGameUnpaused?.Invoke();
            }

            resumeButton.onClick.RemoveListener(ResumeGame);
            settingsButton.onClick.RemoveListener(OpenSettings);
            //controlsButton.onClick.RemoveListener(OpenControls);
            //saveButton.onClick.RemoveListener(SaveGame);
            mainMenuButton.onClick.RemoveListener(QuitToMainMenu);
            quitButton.onClick.RemoveListener(QuitToDesktop);

            thisCanvas.enabled = false;
        }

        public delegate void OnGameUnpausedDelegate();
        public event OnGameUnpausedDelegate OnGameUnpaused;

        public void ResumeGame()
        {
            paused = false;
            this.enabled = false;
        }

        private void OpenSettings()
        {
            StartCoroutine(SwapCanvases(thisCanvas, settings));
        }

        private void OpenControls()
        {
            StartCoroutine(SwapCanvases(thisCanvas, controls));
        }

        protected virtual void SaveGame()
        {

        }

        protected virtual void OnSaveButtonComplete()
        {

        }

        private void QuitToMainMenu()
        {
            ToggleButtonInteractibility(false);

            StartCoroutine(QuitToMainMenuCo());
        }

        private void QuitToDesktop()
        {
            Application.Quit();
        }

        protected override void ToggleButtonInteractibility(bool toggleOn)
        {
            base.ToggleButtonInteractibility(toggleOn);

            resumeButton.interactable = toggleOn;
            settingsButton.interactable = toggleOn;
            //controlsButton.interactable = toggleOn;
            //saveButton.interactable = toggleOn;
            mainMenuButton.interactable = toggleOn;
            quitButton.interactable = toggleOn;
        }

        private IEnumerator QuitToMainMenuCo()
        {
            yield return new WaitForSecondsRealtime(.1f);
            Time.timeScale = 1;
            SceneManager.LoadScene("MainMenu");

            StopCoroutine(QuitToMainMenuCo());
        }

        protected override IEnumerator SwapCanvases(Canvas active, Canvas next)
        {
            yield return StartCoroutine(base.SwapCanvases(active, next));

            StopCoroutine(SwapCanvases(active, next));
        }
    }
}