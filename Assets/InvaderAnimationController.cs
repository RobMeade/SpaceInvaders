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
        GameController.OnGameOver -= StopAnimation;
        InvaderFormationMovementController.OnVelocityIncreased -= InvaderFormationVelocityIncreased;
        _invader.OnHit -= InvaderHit;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += StopAnimation;
        InvaderFormationMovementController.OnVelocityIncreased += InvaderFormationVelocityIncreased;
        _invader.OnHit += InvaderHit;
    }

    private void StopAnimation(object sender, EventArgs e)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
        {
            _animator.enabled = false;
        }
    }
}
