using GMTK2024;
using RobbieWagnerGames.AI;
using RobbieWagnerGames.FirstPerson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class StealthPlayer : MonoBehaviour
{
    [SerializeField] private SphereCollider radiusOfDetection;
    [SerializeField] private float obscuredRadius;
    [SerializeField] private float defaultRadius;
    [SerializeField] private float revealedRadius;

    private AITarget aiTarget;

    private bool isHiding = false;
    public bool IsHiding
    {
        get 
        {
            return isHiding;
        }
        set 
        {
            if (isHiding == value)
                return;
            isHiding = value;
            OnToggleHideState?.Invoke(isHiding);
        }
    }
    public delegate void HideDelegate(bool hiding);
    public event HideDelegate OnToggleHideState;

    private bool isRevealed = false;
    public bool IsRevealed
    {
        get
        {
            return isRevealed;
        }
        set
        {
            if (isRevealed == value)
                return;
            isRevealed = value;
            OnToggleRevealState?.Invoke(isRevealed);
        }
    }
    public event HideDelegate OnToggleRevealState;
    public static StealthPlayer Instance { get; private set; }

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

        ResetPlayer();
        aiTarget = GetComponent<AITarget>();

        SimpleFirstPersonPlayerMovement.Instance.ToggleRun += ToggleReveal;
        GameManager.Instance.OnChangeCurrentAnthill += CheckAnthill;
    }

    private void Update()
    {
        UpdateRadius();
        if(IsRevealed)
            UpdateRevealedState();
    }

    private void UpdateRevealedState()
    {
        if ((GameManager.Instance.CurrentStompingAnthill != null && SimpleFirstPersonPlayerMovement.Instance.IsMoving) || SimpleFirstPersonPlayerMovement.Instance.IsRunning || (aiTarget.chasers != null && aiTarget.chasers.Any()))
            return;

        IsRevealed = false;
    }

    public void UpdateRadius()
    {
        if (DayNightCycle.Instance.Daytime != Daytime.NIGHT)
            radiusOfDetection.radius = obscuredRadius;
        else if(aiTarget != null)
        {
            if (aiTarget.chasers != null && aiTarget.chasers.Any())
                radiusOfDetection.radius = revealedRadius;
            else if (IsRevealed)
                radiusOfDetection.radius = revealedRadius;
            else if (!IsRevealed && !IsHiding)
                radiusOfDetection.radius = defaultRadius;
            else if (IsHiding)
                radiusOfDetection.radius = obscuredRadius;
        }
    }

    public void ObscurePlayer()
    {
        IsHiding = true;
    }

    public void ResetPlayer()
    {
        IsHiding = false;
        IsRevealed = false;
    }

    public void RevealPlayer()
    {
        IsRevealed = true;
    }

    private void ToggleReveal(bool on)
    {
        IsRevealed = on;
    }
    private void CheckAnthill(Anthill anthill)
    {
        if (anthill == null && !SimpleFirstPersonPlayerMovement.Instance.IsRunning)
            IsRevealed = false;
        else if (anthill != null && SimpleFirstPersonPlayerMovement.Instance.IsMoving)
            IsRevealed = true;
    }
}
