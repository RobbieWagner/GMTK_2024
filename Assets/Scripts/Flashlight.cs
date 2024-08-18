using RobbieWagnerGames.Common;
using RobbieWagnerGames.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Flashlight : MonoBehaviour
{
    private PlayerMovementActions controls;
    [SerializeField] private Light flashlight;

    private void Awake()
    {
        controls = new PlayerMovementActions();

        controls.Movement.Flashlight.performed += ToggleFlashlight;
        controls.Enable();

        PauseMenu.Instance.OnGamePaused += DisableControls;
        PauseMenu.Instance.OnGameUnpaused += EnableControls;
    }

    private void DisableControls()
    {
        controls.Disable();
    }

    private void EnableControls()
    {
        controls.Enable();
    }

    private void ToggleFlashlight(InputAction.CallbackContext context)
    {
        BasicAudioManager.Instance.PlayAudioSource(AudioSourceName.Flashlight);
        flashlight.enabled = !flashlight.enabled;
    }

    private void OnDestroy()
    {
        controls.Disable();
    }
}
