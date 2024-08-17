using RobbieWagnerGames.FirstPerson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StealthPlayer : MonoBehaviour
{
    [SerializeField] private SphereCollider radiusOfDetection;
    [SerializeField] private float obscuredRadius;
    [SerializeField] private float defaultRadius;
    [SerializeField] private float revealedRadius;

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

        OnToggleHideState += ToggleRadius;
        ResetPlayer();
    }

    public void ToggleRadius(bool on)
    {
        radiusOfDetection.enabled = on;
    }

    public void ObscurePlayer()
    {
        radiusOfDetection.radius = obscuredRadius;
    }

    public void ResetPlayer()
    {
        radiusOfDetection.radius = defaultRadius;
    }

    public void RevealPlayer()
    {
        radiusOfDetection.radius = revealedRadius;
    }

}
