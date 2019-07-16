using System;

using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioController : MonoBehaviour
{
    // TODO: Consider using this component to AddComponent<AudioSource>
    //       will then need the ability to set the appropriate AudioClip

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
        _audioSource = GetComponent<AudioSource>();
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
        // TODO: It would be better if this wasn't using a reference to the Camera
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
        InvaderFormationMovementController.OnVelocityIncreased += InvaderFormationVelocityIncreased;
    }

    private void Start()
    {
        _audioSource.Play();
    }
}