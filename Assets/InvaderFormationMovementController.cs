using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(InvaderFormation))]
public class InvaderFormationMovementController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private BoxCollider2D _boxCollider2D = null;
    private AudioSource _audioSource = null;
    private InvaderFormation _invaderFormation = null;

    private Vector2 _velocity = new Vector2();

    private bool _isLaunching = false;
    private bool _hasMadeFirstDescent = false;
    private bool _hasLanded = false;
    private bool _canMove = false;


    public delegate void DescentEventHandler(object sender, EventArgs e);
    public static event DescentEventHandler OnDescent;

    public delegate void FirstDescentEventHandler(object sender, EventArgs e);
    public static event FirstDescentEventHandler OnFirstDescent;

    public delegate void LandedEventHandler(object sender, EventArgs e);
    public static event LandedEventHandler OnLanded;

    public delegate void VelocityIncreasedEventHandler(object sender, InvaderFormationVelocityIncreasedEventArgs e);
    public static event VelocityIncreasedEventHandler OnVelocityIncreased;


    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _invaderFormation = GetComponent<InvaderFormation>();

        _velocity = _configuration.InvaderFormationMovementVelocityVerySlow;
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

        if (OnDescent != null)
        {
            OnDescent(this, EventArgs.Empty);
        }

        float xPosition = gameObject.transform.position.x;
        float yPosition = gameObject.transform.position.y - _velocity.y;

        gameObject.transform.position = new Vector2(xPosition, yPosition);
    }

    // TODO: Refactor, too much duplication
    private void DetermineMovementVelocity(int invadersRemaining)
    {
        if (invadersRemaining > 24)
        {
            if(Mathf.Abs(_velocity.x) != Mathf.Abs(_configuration.InvaderFormationMovementVelocityVerySlow.x))
            {
                SetMovementVelocity(_configuration.InvaderFormationMovementVelocityVerySlow);

                if (OnVelocityIncreased != null)
                {
                    InvaderFormationVelocityIncreasedEventArgs invaderFormationVelocityIncreasedEventArgs = new InvaderFormationVelocityIncreasedEventArgs(_configuration.InvaderFormationMovementVelocityVerySlow);
                    OnVelocityIncreased(this, invaderFormationVelocityIncreasedEventArgs);
                }
            }
        }
        else if (invadersRemaining <= 24 && invadersRemaining > 12)
        {
            if(Mathf.Abs(_velocity.x) != Mathf.Abs(_configuration.InvaderFormationMovementVelocitySlow.x))
            {
                SetMovementVelocity(_configuration.InvaderFormationMovementVelocitySlow);

                if(OnVelocityIncreased != null)
                {
                    InvaderFormationVelocityIncreasedEventArgs invaderFormationVelocityIncreasedEventArgs = new InvaderFormationVelocityIncreasedEventArgs(_configuration.InvaderFormationMovementVelocitySlow);
                    OnVelocityIncreased(this, invaderFormationVelocityIncreasedEventArgs);
                }
            }
        }
        else if (invadersRemaining <= 12 && invadersRemaining > 1)
        {
            if(Mathf.Abs(_velocity.x) != Mathf.Abs(_configuration.InvaderFormationMovementVelocityFast.x))
            {
                SetMovementVelocity(_configuration.InvaderFormationMovementVelocityFast);

                if (OnVelocityIncreased != null)
                {
                    InvaderFormationVelocityIncreasedEventArgs invaderFormationVelocityIncreasedEventArgs = new InvaderFormationVelocityIncreasedEventArgs(_configuration.InvaderFormationMovementVelocityFast);
                    OnVelocityIncreased(this, invaderFormationVelocityIncreasedEventArgs);
                }
            }
        }
        else if (invadersRemaining == 1)
        {
            if(Mathf.Abs(_velocity.x) != Mathf.Abs(_configuration.InvaderFormationMovementVelocityVeryFast.x))
            {
                SetMovementVelocity(_configuration.InvaderFormationMovementVelocityVeryFast);

                if (OnVelocityIncreased != null)
                {
                    InvaderFormationVelocityIncreasedEventArgs invaderFormationVelocityIncreasedEventArgs = new InvaderFormationVelocityIncreasedEventArgs(_configuration.InvaderFormationMovementVelocityVeryFast);
                    OnVelocityIncreased(this, invaderFormationVelocityIncreasedEventArgs);
                }
            }
        }
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

    private void InvaderDestroyed(object sender, EventArgs e)
    {
        DetermineMovementVelocity(_invaderFormation.Invaders.Count);
    }

    private void InvaderHit(object sender, EventArgs e)
    {
        ResizeBoxCollider();

        if (_invaderFormation.Invaders.Count == 1)
        {
            SetMovementVelocity(Vector2.zero);
        }
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
        InvaderFormation.OnInvaderHit -= InvaderHit;
        InvaderFormation.OnInvaderDestroyed -= InvaderDestroyed;
        InvaderFormation.OnHaltAttack -= StopMoving;
        InvaderFormation.OnResumeAttack -= ResumeMoving;
    }

    private void OnEnable()
    {
        InvaderFormation.OnInvaderHit += InvaderHit;
        InvaderFormation.OnInvaderDestroyed += InvaderDestroyed;
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

    private void SetMovementVelocity(Vector2 velocity)
    {
        if (velocity != Vector2.zero)
        {
            if (_velocity.x <= 0)
            {
                _velocity.x = -velocity.x;
            }
            else
            {
                _velocity.x = velocity.x;
            }

            _velocity.y = velocity.y;
        }
        else
        {
            _velocity = velocity;
        }
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