using UnityEngine;

[RequireComponent(typeof(CommandShip))]
public class CommandShipMovementController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private CommandShip _commandShip = null;


    private void Awake()
    {
        _commandShip = GetComponent<CommandShip>();
    }

    private void Move()
    {
        float xPosition = gameObject.transform.position.x + _configuration.CommandShipVelocity.x * Time.deltaTime;
        float yPosition = gameObject.transform.position.y;

        gameObject.transform.position = new Vector2(xPosition, yPosition);
    }

    private void Update()
    {
        if (_commandShip.State == CommandShip.CommandShipState.Flying)
        {
            Move();
        }
    }
}