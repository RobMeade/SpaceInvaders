using System.Collections;

using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CommandShipAudioController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private AudioSource _audioSource = null;
    private Coroutine _increasePitch = null;


    private void Awake()
    {
        _audioSource = GetComponent<AudioSource>();
    }

    private void BeginDeathSequence()
    {
        _increasePitch = StartCoroutine(IncreaseAudioPitch());
    }

    private void EndDeathSequence()
    {
        StopCoroutine(_increasePitch);

        _audioSource.Stop();
        _audioSource.pitch = _configuration.CommandShipDefaultAudioPitch;
    }

    private IEnumerator IncreaseAudioPitch()
    {
        while (true)
        {
            _audioSource.pitch += _configuration.CommandShipDestroyedAudioMaximumPitch * Time.deltaTime;

            yield return null;
        }
    }
}