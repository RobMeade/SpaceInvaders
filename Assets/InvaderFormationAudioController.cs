using System;

using UnityEngine;

public class InvaderFormationAudioController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private AudioClip _movementSlowest = null;

    [SerializeField]
    private AudioClip _movementSlow = null;

    [SerializeField]
    private AudioClip _movementFast = null;

    [SerializeField]
    private AudioClip _movementFastest = null;

    [SerializeField]
    private AudioClip _formationDestroyedSFX = null;

    private AudioSource _audioSource = null;


    public void Stop()
    {
        _audioSource.Stop();
    }


    private void Awake()
    {
        AddAudioSourceComponent();
    }

    private void AddAudioSourceComponent()
    {
        _audioSource = gameObject.AddComponent<AudioSource>();

        _audioSource.clip = _movementSlowest;
        _audioSource.playOnAwake = false;
        _audioSource.loop = true;

        _audioSource.volume = _configuration.InvaderFormationDefaultAudioVolume;
        _audioSource.spatialBlend = 0f;
    }

    private void GameOver(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void InvaderFormationDescent(object sender, EventArgs e)
    {
        _audioSource.volume += _configuration.InvaderFormationDescentAudioVolumeIncrease;
    }

    private void InvaderFormationDestroyed(object sender, EventArgs e)
    {
        AudioSource.PlayClipAtPoint(_formationDestroyedSFX, Camera.main.transform.position);
    }

    private void InvaderFormationHaltAttack(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void InvaderFormationLanded(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void InvaderFormationLastInvaderHit(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void InvaderFormationLaunch(object sender, EventArgs e)
    {
        _audioSource.Play();
    }

    private void InvaderFormationResumeAttack(object sender, EventArgs e)
    {
        _audioSource.Play();
    }

    private void InvaderFormationVelocityIncreased(object sender, InvaderFormationVelocityIncreasedEventArgs e)
    {
        if (e.Velocity == _configuration.InvaderFormationMovementVelocitySlow)
        {
            _audioSource.Stop();
            _audioSource.clip = _movementSlow;
            _audioSource.Play();
        }
        else if (e.Velocity == _configuration.InvaderFormationMovementVelocityFast)
        {
            _audioSource.Stop();
            _audioSource.clip = _movementFast;
            _audioSource.Play();
        }
        else if (e.Velocity == _configuration.InvaderFormationMovementVelocityVeryFast)
        {
            _audioSource.Stop();
            _audioSource.clip = _movementFastest;
            _audioSource.Play();
        }
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= GameOver;
        GameController.OnInvaderFormationLanded -= InvaderFormationLanded;
        InvaderFormation.OnFormationDestroyed -= InvaderFormationDestroyed;
        InvaderFormation.OnHaltAttack -= InvaderFormationHaltAttack;
        InvaderFormation.OnLastInvaderHit -= InvaderFormationLastInvaderHit;
        InvaderFormation.OnResumeAttack -= InvaderFormationResumeAttack;
        InvaderFormationMovementController.OnDescent -= InvaderFormationDescent;
        InvaderFormationMovementController.OnLaunch -= InvaderFormationLaunch;
        InvaderFormationMovementController.OnVelocityIncreased -= InvaderFormationVelocityIncreased;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += GameOver;
        GameController.OnInvaderFormationLanded += InvaderFormationLanded;
        InvaderFormation.OnFormationDestroyed += InvaderFormationDestroyed;
        InvaderFormation.OnHaltAttack += InvaderFormationHaltAttack;
        InvaderFormation.OnLastInvaderHit += InvaderFormationLastInvaderHit;
        InvaderFormation.OnResumeAttack += InvaderFormationResumeAttack;
        InvaderFormationMovementController.OnDescent += InvaderFormationDescent;
        InvaderFormationMovementController.OnLaunch += InvaderFormationLaunch;
        InvaderFormationMovementController.OnVelocityIncreased += InvaderFormationVelocityIncreased;
    }
}