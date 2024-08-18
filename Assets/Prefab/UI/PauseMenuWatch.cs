using RobbieWagnerGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PauseMenuWatch : MonoBehaviour
{
    [SerializeField] private PauseMenu pauseMenu;
    private UIControls controls;
    
    private void Awake()
    {
        controls = new UIControls();
        controls.Enable();
        controls.UI.PauseGame.performed += TogglePauseMenu;
    }

    private void TogglePauseMenu(InputAction.CallbackContext context)
    {
        if (pauseMenu.enabled)
            pauseMenu.paused = false;
        pauseMenu.enabled = !pauseMenu.enabled;
    }

    private void OnDestroy()
    {
        controls.Disable();
    }
}
