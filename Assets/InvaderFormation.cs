﻿using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class InvaderFormation : MonoBehaviour
{
    private List<Invader> _invaders = new List<Invader>();
    private int _invadersPerWave = 0;


    public delegate void CeaseFireEventHandler(object sender, EventArgs e);
    public static event CeaseFireEventHandler OnCeaseFire;

    public delegate void HaltAttackEventHandler(object sender, EventArgs e);
    public static event HaltAttackEventHandler OnHaltAttack;

    public delegate void FormationDestroyedEventHandler(object sender, EventArgs e);
    public static event FormationDestroyedEventHandler OnFormationDestroyed;

    public delegate void InvaderDestroyedEventHandler(object sender, InvaderFormationInvaderDestroyedEventArgs e);
    public static event InvaderDestroyedEventHandler OnInvaderDestroyed;

    public delegate void InvaderHitEventHandler(object sender, EventArgs e);
    public static event InvaderHitEventHandler OnInvaderHit;

    public delegate void LastInvaderHitEventHandler(object sender, EventArgs e);
    public static event LastInvaderHitEventHandler OnLastInvaderHit;

    public delegate void ResumeAttackEventHandler(object sender, EventArgs e);
    public static event ResumeAttackEventHandler OnResumeAttack;


    public List<Invader> Invaders
    {
        get { return _invaders; }
    }


    private void Awake()
    {
        _invaders = GetComponentsInChildren<Invader>().ToList<Invader>();
        _invadersPerWave = _invaders.Count();
    }

    private void CeaseFire(object sender, EventArgs e)
    {
        if (OnCeaseFire != null)
        {
            OnCeaseFire(this, EventArgs.Empty);
        }
    }

    private void HaltAttack(object sender, EventArgs e)
    {
        if (OnHaltAttack != null)
        {
            OnHaltAttack(this, EventArgs.Empty);
        }
    }

    private void InvaderDestroyed(object sender, InvaderDestroyedEventArgs e)
    {
        e.Invader.OnDestroyed -= InvaderDestroyed;
        e.Invader.OnHit -= InvaderHit;

        _invaders.Remove(e.Invader);

        if (OnInvaderDestroyed != null)
        {
            InvaderFormationInvaderDestroyedEventArgs invaderFormationInvaderDestroyedEventArgs = new InvaderFormationInvaderDestroyedEventArgs(_invadersPerWave, _invaders.Count);

            OnInvaderDestroyed(this, invaderFormationInvaderDestroyedEventArgs);
        }

        if (_invaders.Count == 0)
        {
            if (OnFormationDestroyed != null)
            {
                OnFormationDestroyed(this, EventArgs.Empty);
            }

            Destroy(gameObject);
        }
    }

    private void InvaderHit(object sender, EventArgs e)
    {
        if (_invaders.Count == 1)
        {
            if (OnLastInvaderHit != null)
            {
                OnLastInvaderHit(this, EventArgs.Empty);
            }
        }

        if (OnInvaderHit != null)
        {
            OnInvaderHit(this, EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        InvaderMotherShip.OnCeaseFire -= CeaseFire;
        InvaderMotherShip.OnHaltAttack -= HaltAttack;
        InvaderMotherShip.OnResumeAttack -= ResumeAttack;
    }

    private void OnEnable()
    {
        InvaderMotherShip.OnCeaseFire += CeaseFire;
        InvaderMotherShip.OnHaltAttack += HaltAttack;
        InvaderMotherShip.OnResumeAttack += ResumeAttack;
    }

    private void ResumeAttack(object sender, EventArgs e)
    {
        if (OnResumeAttack != null)
        {
            OnResumeAttack(this, EventArgs.Empty);
        }
    }

    private void Start()
    {
        foreach (Invader invader in _invaders)
        {
            invader.OnDestroyed += InvaderDestroyed;
            invader.OnHit += InvaderHit;
        }
    }
}