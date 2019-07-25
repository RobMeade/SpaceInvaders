using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D = null;
    private Vector2 _velocity = new Vector2();

    private bool _commandShipCollision = false;


    public delegate void DestroyedEventHandler(object sender, ProjectileDestroyedEventArgs e);
    public event DestroyedEventHandler OnDestroyed;


    public Vector2 Velocity
    {
        set { _velocity = value; }
    }


    private void Awake()
    {
        _rigidBody2D = GetComponent<Rigidbody2D>();
    }

    private void Move()
    {
        _rigidBody2D.velocity = _velocity;
    }

    private void OnDisable()
    {
        GameController.OnPlayerAbduction -= PlayerAbduction;
        GameController.OnPlayerAbductionComplete -= PlayerAbductionComplete;
    }

    private void OnEnable()
    {
        GameController.OnPlayerAbduction += PlayerAbduction;
        GameController.OnPlayerAbductionComplete += PlayerAbductionComplete;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // TODO: Refactor - Projectile shouldn't know about CommandShip
        CommandShip commandShip = collision.gameObject.GetComponent<CommandShip>();

        if(!commandShip || commandShip.State != CommandShip.CommandShipState.Abducting)
        {
            SelfDestruct();
        }
        else
        {
            _commandShipCollision = true;
        }
    }

    private void PlayerAbduction(object sender, EventArgs e)
    {
        _rigidBody2D.velocity = Vector2.zero;
    }

    private void PlayerAbductionComplete(object sender, EventArgs e)
    {
        // TODO: Refactor - Projectile shouldn't know about CommandShip
        if (_commandShipCollision)
        {
            SelfDestruct();
        }
        else
        {
            _rigidBody2D.velocity = _velocity;
        }
    }

    private void SelfDestruct()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(this, new ProjectileDestroyedEventArgs(this));
        }

        Destroy(gameObject);
    }

    private void Start()
    {
        Move();
    }
}