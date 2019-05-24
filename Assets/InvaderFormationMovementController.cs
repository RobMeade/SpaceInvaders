using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(InvaderFormation))]
public class InvaderFormationMovementController : MonoBehaviour
{
    private BoxCollider2D _boxCollider2D = null;
    private InvaderFormation _invaderFormation = null;

    private Vector2 _velocity = new Vector2();

    private bool _isLaunching = false;
    private bool _hasMadeFirstDescent = false;
    private bool _hasLanded = false;
    private bool _canMove = false;


    public delegate void FirstDescentEventHandler(object sender, EventArgs e);
    public static event FirstDescentEventHandler OnFirstDescent;

    public delegate void LandedEventHandler(object sender, EventArgs e);
    public static event LandedEventHandler OnLanded;


    public Vector2 Velocity
    {
        set { _velocity = value; }
    }


    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _invaderFormation = GetComponent<InvaderFormation>();
    }

    private void Descend()
    {
        if (!_hasMadeFirstDescent)
        {
            _hasMadeFirstDescent = true;

            if (OnFirstDescent != null)
            {
                OnFirstDescent(this, EventArgs.Empty);
            }
        }

        float xPosition = gameObject.transform.position.x;
        float yPosition = gameObject.transform.position.y - _velocity.y;

        gameObject.transform.position = new Vector2(xPosition, yPosition);
    }

    private Bounds GetChildBoxColliderBounds()
    {
        Bounds bounds = default(Bounds);
        BoxCollider2D boxCollider2D = null;

        bool hasBounds = false;

        for (int i = 0, ni = _invaderFormation.Invaders.Count; i < ni; i++)
        {
            boxCollider2D = _invaderFormation.Invaders[i].GetComponent<BoxCollider2D>();

            if (boxCollider2D && boxCollider2D.enabled)
            {
                if (hasBounds)
                {
                    bounds.Encapsulate(boxCollider2D.bounds);
                }
                else
                {
                    bounds = boxCollider2D.bounds;

                    hasBounds = true;
                }
            }
        }

        return bounds;
    }

    private void Launch()
    {
        _isLaunching = true;
    }

    private void Move()
    {
        if (!_hasLanded)
        {
            float xPosition = gameObject.transform.position.x + _velocity.x * Time.deltaTime;
            float yPosition = gameObject.transform.position.y;

            gameObject.transform.position = new Vector2(xPosition, yPosition);
        }
    }

    private void OnDisable()
    {
        InvaderFormation.OnInvaderHit -= ResizeBoxCollider;
        InvaderFormation.OnHaltAttack -= StopMoving;
        InvaderFormation.OnResumeAttack -= ResumeMoving;
    }

    private void OnEnable()
    {
        InvaderFormation.OnInvaderHit += ResizeBoxCollider;
        InvaderFormation.OnHaltAttack += StopMoving;
        InvaderFormation.OnResumeAttack += ResumeMoving;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayspaceBoundary playspaceBoundary = collision.gameObject.GetComponent<PlayspaceBoundary>();

        if (playspaceBoundary != null)
        {
            PlayspaceCollision(playspaceBoundary.Type);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        PlayspaceBoundary playspaceBoundary = collision.gameObject.GetComponent<PlayspaceBoundary>();

        if (playspaceBoundary != null)
        {
            _isLaunching = false;
        }
    }

    private void PlayspaceCollision(PlayspaceBoundary.BoundaryType boundaryType)
    {
        switch (boundaryType)
        {
            case PlayspaceBoundary.BoundaryType.Left:
            case PlayspaceBoundary.BoundaryType.Right:

                if (!_isLaunching)
                {
                    Descend();
                    ReverseHorizontalDirection();
                }

                break;

            case PlayspaceBoundary.BoundaryType.Bottom:

                if (!_hasLanded)
                {
                    _hasLanded = true;

                    if (OnLanded != null)
                    {
                        OnLanded(this, EventArgs.Empty);
                    }
                }

                break;
        }
    }

    private void ResizeBoxCollider()
    {
        ResizeBoxCollider(this, EventArgs.Empty);
    }

    private void ResizeBoxCollider(object sender, EventArgs e)
    {
        Bounds boxColliderBounds = GetChildBoxColliderBounds();

        _boxCollider2D.size = boxColliderBounds.size;
        _boxCollider2D.offset = boxColliderBounds.center - transform.position;
    }

    private void ResumeMoving(object sender, EventArgs e)
    {
        _canMove = true;
    }

    private void ReverseHorizontalDirection()
    {
        _velocity.x = -_velocity.x;
    }

    private void Start()
    {
        _canMove = true;

        ResizeBoxCollider();
        Launch();
    }

    private void StopMoving(object sender, EventArgs e)
    {
        _canMove = false;
    }

    private void Update()
    {
        if (_canMove)
        {
            Move();
        }
    }
}