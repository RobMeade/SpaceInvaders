using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class Projectile : MonoBehaviour
{
    private Rigidbody2D _rigidBody2D = null;
    private Vector2 _velocity = new Vector2();


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

    private void Start()
    {
        Move();
    }

    private void Move()
    {
        _rigidBody2D.velocity = _velocity;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(this, new ProjectileDestroyedEventArgs(this));
        }

        Destroy(gameObject);
    }
}