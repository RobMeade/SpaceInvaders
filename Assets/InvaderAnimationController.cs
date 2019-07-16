using System;

using UnityEngine;

[RequireComponent(typeof(Animator))]
public class InvaderAnimationController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private Animator _animator = null;
    private Invader _invader = null;

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _invader = GetComponent<Invader>();

        _animator.SetBool("isAlive", true);
    }

    private void InvaderFormationLanded(object sender, EventArgs e)
    {
        StopAnimation();
    }

    private void InvaderFormationHaltAttack(object sender, EventArgs e)
    {
        StopAnimation();
    }

    private void InvaderFormationResumeAttack(object sender, EventArgs e)
    {
        StartAnimation();
    }

    private void InvaderFormationVelocityIncreased(object sender, InvaderFormationVelocityIncreasedEventArgs e)
    {
        if (e.Velocity == _configuration.InvaderFormationMovementVelocitySlow)
        {
            _animator.SetFloat("speedMultiplier", _configuration.InvaderAnimationSpeedMultiplierSlow);
        }
        else if (e.Velocity == _configuration.InvaderFormationMovementVelocityFast)
        {
            _animator.SetFloat("speedMultiplier", _configuration.InvaderAnimationSpeedMultiplierFast);
        }
        else if (e.Velocity == _configuration.InvaderFormationMovementVelocityVeryFast)
        {
            _animator.SetFloat("speedMultiplier", _configuration.InvaderAnimationSpeedMultiplierFastest);
        }
    }

    private void InvaderHit(object sender, EventArgs e)
    {
        _animator.SetBool("isAlive", false);
    }

    private void OnDisable()
    {
        GameController.OnInvaderFormationLanded -= InvaderFormationLanded;
        InvaderFormation.OnHaltAttack -= InvaderFormationHaltAttack;
        InvaderFormation.OnResumeAttack -= InvaderFormationResumeAttack;
        InvaderFormationMovementController.OnVelocityIncreased -= InvaderFormationVelocityIncreased;
        _invader.OnHit -= InvaderHit;
    }

    private void OnEnable()
    {
        GameController.OnInvaderFormationLanded += InvaderFormationLanded;
        InvaderFormation.OnHaltAttack += InvaderFormationHaltAttack;
        InvaderFormation.OnResumeAttack += InvaderFormationResumeAttack;
        InvaderFormationMovementController.OnVelocityIncreased += InvaderFormationVelocityIncreased;
        _invader.OnHit += InvaderHit;
    }

    private void StartAnimation()
    {
        // TODO: Consider using _animator.StopPlayBack() instead of disabling component
        if (_animator.enabled == false)
        {
            _animator.enabled = true;
        }
    }

    private void StopAnimation()
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
        {
            // TODO: Consider using _animator.StopPlayBack() instead of disabling component
            _animator.enabled = false;
        }
    }
}
