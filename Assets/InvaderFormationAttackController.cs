﻿using System;

using UnityEngine;

[RequireComponent(typeof(InvaderFormation))]
public class InvaderFormationAttackController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private GameObject _projectilePrefab = null;

    private InvaderFormation _invaderFormation = null;

    private bool _canAttack;
    private bool _canReArm;
    private bool _reArmed;

    private float _timeToReArm;
    private float _fireRate;


    private void Attack()
    {
        Invader invader;

        if (_invaderFormation.Invaders.Count > 0)
        {
            int invaderIndex = UnityEngine.Random.Range(0, _invaderFormation.Invaders.Count - 1);

            invader = _invaderFormation.Invaders[invaderIndex];

            if (invader & invader.HasLaunched)
            {
                _canReArm = false;
                _reArmed = false;

                Color invaderColor = invader.GetComponent<SpriteRenderer>().color;

                GameObject projectile = Instantiate(_projectilePrefab, invader.transform.position, Quaternion.identity);

                projectile.GetComponent<Projectile>().OnDestroyed += ProjectileDestroyed;
                projectile.GetComponent<Projectile>().Velocity = _configuration.InvaderProjectileVelocity;
                projectile.GetComponent<SpriteRenderer>().color = new Color(invaderColor.r, invaderColor.g, invaderColor.b, invaderColor.a);
            }
        }
    }

    private void Awake()
    {
        _invaderFormation = GetComponent<InvaderFormation>();
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= StopAttacking;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += StopAttacking;
    }

    private void PrepareToAttack()
    {
        _fireRate = UnityEngine.Random.Range(_configuration.InvaderMinimumReArmDelay, _configuration.InvaderMaximumReArmDelay);

        _timeToReArm = Time.time + _fireRate;
    }

    private void ProjectileDestroyed(object sender, ProjectileDestroyedEventArgs e)
    {
        _canReArm = true;

        e.Projectile.OnDestroyed -= ProjectileDestroyed;

        PrepareToAttack();
    }

    private void Start()
    {
        _canAttack = true;
        _canReArm = true;

        PrepareToAttack();
    }

    private void StopAttacking(object sender, EventArgs e)
    {
        _canAttack = false;
    }

    private void Update()
    {
        if(_canAttack && _canReArm && Time.time > _timeToReArm)
        {
            _reArmed = true;
        }

        if (_canAttack && _reArmed && _invaderFormation.Invaders.Count > 0)
        {
            Attack();
        }
    }
}