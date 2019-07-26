using System;

using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(InvaderFormation))]
public class InvaderFormationMovementController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private BoxCollider2D _boxCollider2D = null;
    private InvaderFormation _invaderFormation = null;

    private Vector2 _velocity = new Vector2();

    private bool _isLaunching = false;
    private bool _hasMadeFirstDescent = false;
    private bool _hasLanded = false;
    private bool _canMove = false;
    private bool _hasInitiatedLaunch = false;


    public delegate void DescentEventHandler(object sender, EventArgs e);
    public static event DescentEventHandler OnDescent;

    public delegate void FirstDescentEventHandler(object sender, EventArgs e);
    public static event FirstDescentEventHandler OnFirstDescent;

    public delegate void LandedEventHandler(object sender, EventArgs e);
    public static event LandedEventHandler OnLanded;

    public delegate void LaunchEventHandler(object sender, EventArgs e);
    public static event LaunchEventHandler OnLaunch;

    public delegate void VelocityIncreasedEventHandler(object sender, InvaderFormationVelocityIncreasedEventArgs e);
    public static event VelocityIncreasedEventHandler OnVelocityIncreased;


    private void Awake()
    {
        _boxCollider2D = GetComponent<BoxCollider2D>();
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

    private void DetermineMovementVelocity(int invadersHit, int invadersRemaining)
    {
        if (invadersHit < 48)
        {
            if (invadersRemaining > 24)
            {
                IncreaseMovementVelocity(_configuration.InvaderFormationMovementVelocityVerySlow);
            }
            else if (invadersRemaining <= 24 && invadersRemaining > 12)
            {
                IncreaseMovementVelocity(_configuration.InvaderFormationMovementVelocitySlow);
            }
            else if (invadersRemaining <= 12 && invadersRemaining > 1)
            {
                IncreaseMovementVelocity(_configuration.InvaderFormationMovementVelocityFast);
            }
            else if (invadersRemaining == 1)
            {
                IncreaseMovementVelocity(_configuration.InvaderFormationMovementVelocityVeryFast);
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

    private void IncreaseMovementVelocity(Vector2 velocity)
    {
        if (Mathf.Abs(_velocity.x) != Mathf.Abs(velocity.x))
        {
            SetMovementVelocity(velocity);

            if (OnVelocityIncreased != null)
            {
                OnVelocityIncreased(this, new InvaderFormationVelocityIncreasedEventArgs(velocity));
            }
        }
    }

    private void InvaderDestroyed(object sender, InvaderFormationInvaderDestroyedEventArgs e)
    {
        DetermineMovementVelocity(e.InvadersHit, e.RemainingInvaders);
    }

    private void InvaderHit(object sender, InvaderFormationInvaderHitEventArgs e)
    {
        ResizeBoxCollider();

        if (e.InvadersHit == 48)
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
        float xPosition = gameObject.transform.position.x + _velocity.x * Time.deltaTime;
        float yPosition = gameObject.transform.position.y;

        gameObject.transform.position = new Vector2(xPosition, yPosition);

    }

    private void OnDisable()
    {
        GameController.OnInvaderFormationLanded -= StopMoving;
        InvaderFormation.OnInvaderHit -= InvaderHit;
        InvaderFormation.OnInvaderDestroyed -= InvaderDestroyed;
        InvaderFormation.OnHaltAttack -= StopMoving;
        InvaderFormation.OnResumeAttack -= ResumeMoving;
    }

    private void OnEnable()
    {
        GameController.OnInvaderFormationLanded += StopMoving;
        InvaderFormation.OnInvaderHit += InvaderHit;
        InvaderFormation.OnInvaderDestroyed += InvaderDestroyed;
        InvaderFormation.OnHaltAttack += StopMoving;
        InvaderFormation.OnResumeAttack += ResumeMoving;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayspaceBoundary playspaceBoundary = collision.gameObject.GetComponent<PlayspaceBoundary>();
        InvaderMotherShip invaderMotherShip = collision.gameObject.GetComponent<InvaderMotherShip>();

        if (playspaceBoundary != null)
        {
            PlayspaceCollision(playspaceBoundary.Type);
        }

        if (invaderMotherShip)
        {
            if (!_hasInitiatedLaunch && OnLaunch != null)
            {
                _hasInitiatedLaunch = true;

                OnLaunch(this, EventArgs.Empty);
            }
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