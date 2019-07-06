using UnityEngine;

[RequireComponent(typeof(CommandShip))]
public class CommandShipPlayerAbductionController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private CommandShip _commandShip = null;
    private AudioSource _audioSource = null;
    private AbductionSequenceState _abductionSequenceState = AbductionSequenceState.Targeting;

    private GameObject _target = null;

    private float _commandShipPositionX;
    private float _commandShipPositionY;

    float _distanceToTravel = 0f;
    float _timeToTravel = 0f;
    float _maxPitchDelta = 0f;


    private enum AbductionSequenceState { Targeting, Descending, Ascending };


    public void SetTarget(GameObject target)
    {
        _target = target;

        _distanceToTravel = gameObject.transform.position.y - (_target.transform.position.y + _configuration.CommandShipAbductionContactPointOffset.y);
        _timeToTravel = _distanceToTravel / _configuration.CommandShipVelocity.y;
        _maxPitchDelta = _configuration.CommandShipDefaultAudioPitch - _configuration.CommandShipDescentAudioMinimumPitch;
    }


    private void Abduct()
    {
        _commandShipPositionX = gameObject.transform.position.x;
        _commandShipPositionY = gameObject.transform.position.y;

        switch (_abductionSequenceState)
        {
            case AbductionSequenceState.Targeting:
                {
                    Target();

                    break;
                }
            case AbductionSequenceState.Descending:
                {
                    Descend();

                    break;
                }
            case AbductionSequenceState.Ascending:
                {
                    Ascend();

                    break;
                }
        }
    }

    private void Ascend()
    {
        _commandShipPositionY = gameObject.transform.position.y + _configuration.CommandShipVelocity.y * Time.deltaTime;
        gameObject.transform.position = new Vector2(_commandShipPositionX, _commandShipPositionY);

        IncreaseAudioPitch();
    }

    private void Awake()
    {
        _commandShip = GetComponent<CommandShip>();
        _audioSource = GetComponent<AudioSource>();
    }

    private void DecreaseAudioPitch()
    {
        // TODO: Consider moving the pitch change code in this class to a CommandShipAudioController component
        //       send notification of state change and then call appropriate method.

         _audioSource.pitch -= _maxPitchDelta / _timeToTravel * Time.deltaTime;

        if (_audioSource.pitch <= _configuration.CommandShipDescentAudioMinimumPitch)
        {
            _audioSource.pitch = _configuration.CommandShipDescentAudioMinimumPitch;
        }
    }

    private void IncreaseAudioPitch()
    {
        _audioSource.pitch += _maxPitchDelta / _timeToTravel * Time.deltaTime;

        if (_audioSource.pitch >= _configuration.CommandShipAscentAudioMaximumPitch)
        {
            _audioSource.pitch = _configuration.CommandShipAscentAudioMaximumPitch;
        }
    }

    private void Descend()
    {
        if (_commandShipPositionY <= (_target.transform.position.y + _configuration.CommandShipAbductionContactPointOffset.y))
        {
            // refine positioning due to over step
            gameObject.transform.position = new Vector2(_target.transform.position.x, _target.transform.position.y + _configuration.CommandShipAbductionContactPointOffset.y);

            _target.transform.parent = gameObject.transform;

            _abductionSequenceState = AbductionSequenceState.Ascending;
        }
        else
        {
            _commandShipPositionY = gameObject.transform.position.y - _configuration.CommandShipVelocity.y * Time.deltaTime;
            gameObject.transform.position = new Vector2(_commandShipPositionX, _commandShipPositionY);

            DecreaseAudioPitch();
        }
    }

    private void Target()
    {
        if (_commandShipPositionX >= _target.transform.position.x)
        {
            // refine positioning due to over step
            gameObject.transform.position = new Vector2(_target.transform.position.x, _commandShipPositionY);

            _abductionSequenceState = AbductionSequenceState.Descending;
        }
        else
        {
            _commandShipPositionX = gameObject.transform.position.x + _configuration.CommandShipVelocity.x * Time.deltaTime;
            gameObject.transform.position = new Vector2(_commandShipPositionX, _commandShipPositionY);
        }
    }

    private void Update()
    {
        if (_commandShip.State == CommandShip.CommandShipState.Abducting)
        {
            if (_target != null)
            {
                Abduct();
            }
        }
    }
}