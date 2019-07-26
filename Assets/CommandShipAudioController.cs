using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CommandShipAudioController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private AudioSource _audioSource = null;
    private Coroutine _increasePitch = null;

    float _playerAbductionMaximumPitchDelta = 0f;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();

        _playerAbductionMaximumPitchDelta = _configuration.CommandShipDefaultAudioPitch - _configuration.CommandShipDescentAudioMinimumPitch;
    }

    private void BeginDeathSequence()
    {
        _increasePitch = StartCoroutine(DeathSequence());
    }

    private void CommandShipAscend(object sender, CommandShipAscendEventArgs e)
    {
        float pitchDelta = _playerAbductionMaximumPitchDelta / e.Step * Time.deltaTime;

        IncreasePitch(pitchDelta, _configuration.CommandShipAscentAudioMaximumPitch);
    }

    private void CommandShipDescend(object sender, CommandShipDescendEventArgs e)
    {
        float pitchDelta = _playerAbductionMaximumPitchDelta / e.Step * Time.deltaTime;

        DecreasePitch(pitchDelta, _configuration.CommandShipDescentAudioMinimumPitch);
    }

    private void DecreasePitch(float decrease, float minPitch)
    {
        _audioSource.pitch -= decrease;

        if (_audioSource.pitch <= minPitch)
        {
            _audioSource.pitch = minPitch;
        }
    }

    private void EndDeathSequence()
    {
        StopCoroutine(_increasePitch);

        _audioSource.Stop();
        _audioSource.pitch = _configuration.CommandShipDefaultAudioPitch;
    }

    private void GameOver(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void IncreasePitch(float increase, float maxPitch)
    {
        _audioSource.pitch += increase;

        if (_audioSource.pitch >= maxPitch)
        {
            _audioSource.pitch = maxPitch;
        }
    }

    private IEnumerator DeathSequence()
    {
        while (true)
        {
            _audioSource.pitch += _configuration.CommandShipDestroyedAudioMaximumPitch * Time.deltaTime;

            yield return null;
        }
    }

    private void InvaderFormationLanded(object sender, EventArgs e)
    {
        _audioSource.Stop();
    }

    private void OnDisable()
    {
        CommandShipPlayerAbductionController.OnCommandShipAscend -= CommandShipAscend;
        CommandShipPlayerAbductionController.OnCommandShipDescend -= CommandShipDescend;
        GameController.OnGameOver -= GameOver;
        GameController.OnInvaderFormationLanded -= InvaderFormationLanded;
    }

    private void OnEnable()
    {
        CommandShipPlayerAbductionController.OnCommandShipAscend += CommandShipAscend;
        CommandShipPlayerAbductionController.OnCommandShipDescend += CommandShipDescend;
        GameController.OnGameOver += GameOver;
        GameController.OnInvaderFormationLanded += InvaderFormationLanded;
    }
}