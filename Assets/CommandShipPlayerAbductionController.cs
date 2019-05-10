using UnityEngine;

[RequireComponent(typeof(CommandShip))]
public class CommandShipPlayerAbductionController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private CommandShip _commandShip = null;
    private AbductionSequenceState _abductionSequenceState = AbductionSequenceState.Targeting;

    private GameObject _target = null;

    private float _commandShipPositionX;
    private float _commandShipPositionY;


    private enum AbductionSequenceState { Targeting, Descending, Ascending };


    public void SetTarget(GameObject target)
    {
        _target = target;
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
        // move
        _commandShipPositionY = gameObject.transform.position.y + _configuration.CommandShipVelocity.y * Time.deltaTime;
        gameObject.transform.position = new Vector2(_commandShipPositionX, _commandShipPositionY);
    }

    private void Awake()
    {
        _commandShip = GetComponent<CommandShip>();
    }

    private void Target()
    {
        if (_commandShipPositionX >= _target.transform.position.x)
        {
            // refine positioning
            gameObject.transform.position = new Vector2(_target.transform.position.x, _commandShipPositionY);

            _abductionSequenceState = AbductionSequenceState.Descending;
        }
        else
        {
            // move
            _commandShipPositionX = gameObject.transform.position.x + _configuration.CommandShipVelocity.x * Time.deltaTime;
            gameObject.transform.position = new Vector2(_commandShipPositionX, _commandShipPositionY);
        }
    }

    private void Descend()
    {
        if (_commandShipPositionY <= (_target.transform.position.y + 0.125f))   // TODO: Put this into config
        {
            // refine positioning
            gameObject.transform.position = new Vector2(_target.transform.position.x, _target.transform.position.y + 0.125f);

            _target.transform.parent = gameObject.transform;

            _abductionSequenceState = AbductionSequenceState.Ascending;
        }
        else
        {
            // move
            _commandShipPositionY = gameObject.transform.position.y - _configuration.CommandShipVelocity.y * Time.deltaTime;
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
