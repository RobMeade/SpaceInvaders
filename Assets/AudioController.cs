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

    private AudioSource _audioSource = null;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void InvaderFormationDescent(object sender, EventArgs e)
    {
        _audioSource.volume += _configuration.InvaderFormationDescentAudioVolumeIncrease;
    }

    private void InvaderFormationLanded(object sender, EventArgs e)
    {
        _audioSource.Stop();
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
        InvaderFormationMovementController.OnDescent -= InvaderFormationDescent;
        InvaderFormationMovementController.OnLanded -= InvaderFormationLanded;
        InvaderFormationMovementController.OnVelocityIncreased -= InvaderFormationVelocityIncreased;
    }

    private void OnEnable()
    {
        InvaderFormationMovementController.OnDescent += InvaderFormationDescent;
        InvaderFormationMovementController.OnLanded += InvaderFormationLanded;
        InvaderFormationMovementController.OnVelocityIncreased += InvaderFormationVelocityIncreased;
    }

    private void Start()
    {
        _audioSource.Play();
    }
}
