﻿using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class InvaderMotherShip : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private InvaderFormation _invaderFormationPrefab = null;

    private InvaderFormation _invaderFormation = null;
    private int _descentsPerformed = 0;
    private bool _hasLanded = false;


    public delegate void CeaseFireEventHandler(object sender, EventArgs e);
    public static event CeaseFireEventHandler OnCeaseFire;

    public delegate void DescentCompleteEventHandler(object sender, EventArgs e);
    public static event DescentCompleteEventHandler OnDescentComplete;

    public delegate void HaltAttackEventHandler(object sender, EventArgs e);
    public static event HaltAttackEventHandler OnHaltAttack;

    public delegate void LandedEventHandler(object sender, EventArgs e);
    public static event LandedEventHandler OnLanded;

    public delegate void InvaderFormationSpawnedEventHandler(object sender, InvaderFormationSpawnedEventArgs e);
    public static event InvaderFormationSpawnedEventHandler OnInvaderFormationSpawned;

    public delegate void ResumeAttackEventHandler(object sender, EventArgs e);
    public static event ResumeAttackEventHandler OnResumeAttack;


    public int DescentsPerformed
    {
        get { return _descentsPerformed; }
    }


    public void CeaseFire()
    {
        if (OnCeaseFire != null)
        {
            OnCeaseFire(this, EventArgs.Empty);
        }
    }

    public void HaltAttack()
    {
        if (OnHaltAttack != null)
        {
            OnHaltAttack(this, EventArgs.Empty);
        }
    }

    public void ResumeAttack()
    {
        if (OnResumeAttack != null)
        {
            OnResumeAttack(this, EventArgs.Empty);
        }
    }

    private void DescendAndLaunchInvaderFormation(object sender, EventArgs e)
    {
        // NOTE:    This code gets interupted as the descent steps do not fully complete before the Landed event is raised.
        //          As such, the DescentComplete event is never raised for the final (8th) descent.
        //          This is useful in one case, as it prevents the player from being repositioned during the player abduction,
        //          however, it also means that the PlayerAbductionComplete event needs to call EnableLaunch for the CommandShip
        //          which is ordinarily taken care of through the DescentComplete event handling.
        if (!_hasLanded)
        {
            StartCoroutine(Descend());
        }
        else
        {
            PrepareToLaunchInvaderFormation();

            if (OnDescentComplete != null)
            {
                OnDescentComplete(this, EventArgs.Empty);
            }
        }
    }

    private IEnumerator Descend()
    {
        bool arrived = false;
        Vector3 targetPosition = new Vector2(transform.position.x, transform.position.y - _configuration.MotherShipDescentStep);

        yield return new WaitForSeconds(_configuration.MotherShipDescentInitialDelay);

        while (!arrived)
        {
            if (transform.position == targetPosition)
            {
                arrived = true;
                _descentsPerformed++;

                if (!_hasLanded)
                {
                    PrepareToLaunchInvaderFormation();

                    if (OnDescentComplete != null)
                    {
                        OnDescentComplete(this, EventArgs.Empty);
                    }
                }

                yield return null;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (_configuration.MotherShipDescentStep / _configuration.MotherShipStepsPerDescent));

                yield return new WaitForSeconds(_configuration.MotherShipDescentIterativeDelay);
            }
        }
    }

    private void DestroyExistingInvaderFormation()
    {
        if (_invaderFormation)
        {
            Destroy(_invaderFormation.gameObject);
        }
    }

    private IEnumerator DelayLaunchOfInvaderFormation(float launchDelay)
    {
        yield return new WaitForSeconds(launchDelay);

        LaunchInvaderFormation();
    }

    private void GameStarted(object sender, EventArgs e)
    {
        transform.position = _configuration.MotherShipSpawnPosition;

        InitiateAttack();
    }

    private void InitiateAttack()
    {
        DestroyExistingInvaderFormation();

        PrepareToLaunchInvaderFormation();
    }

    private void InitiateAttack(object sender, EventArgs e)
    {
        InitiateAttack();
    }

    private void LaunchInvaderFormation()
    {
        Vector2 launchPosition = (Vector2)transform.position + _configuration.InvaderFormationSpawnPositionOffSet;

        _invaderFormation = Instantiate(_invaderFormationPrefab, launchPosition, Quaternion.identity);

        if (OnInvaderFormationSpawned != null)
        {
            InvaderFormationSpawnedEventArgs invaderFormationSpawnedEventArgs = new InvaderFormationSpawnedEventArgs(_descentsPerformed);

            OnInvaderFormationSpawned(this, invaderFormationSpawnedEventArgs);
        }

        // NOTE: Could use the Game parameter (overloaded method of same name) to set the invader formations shooting pattern
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= GameStarted;
        GameController.OnPlayerAbductionComplete -= InitiateAttack;
        InvaderFormation.OnFormationDestroyed -= DescendAndLaunchInvaderFormation;
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += GameStarted;
        GameController.OnPlayerAbductionComplete += InitiateAttack;
        InvaderFormation.OnFormationDestroyed += DescendAndLaunchInvaderFormation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayspaceBoundary playspaceBoundary = collision.gameObject.GetComponent<PlayspaceBoundary>();

        if (playspaceBoundary && playspaceBoundary.Type == PlayspaceBoundary.BoundaryType.Bottom)
        {
            if (!_hasLanded)
            {
                Land();
            }
        }
    }

    private void Land()
    {
        _hasLanded = true;

        if (OnLanded != null)
        {
            OnLanded(this, EventArgs.Empty);
        }
    }

    private void PrepareToLaunchInvaderFormation()
    {
        StartCoroutine(DelayLaunchOfInvaderFormation(1f));
    }
}