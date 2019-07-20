using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AnimationEventController))]
[RequireComponent(typeof(CommandShipMovementController))]
[RequireComponent(typeof(CommandShipPlayerAbductionController))]
[RequireComponent(typeof(CommandShipAudioController))]
public class CommandShip : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private Animator _animator = null;
    private PolygonCollider2D _polygonCollider2D = null;
    private AudioSource _audioSource = null;
    private AnimationEventController _animationEventController = null;
    private CommandShipMovementController _commandShipMovementController = null;
    private CommandShipPlayerAbductionController _commandShipPlayerAbductionController = null;

    private CommandShipState _commandShipState = CommandShipState.Idle;
    private bool _canLaunch = false;


    public delegate void HitEventHandler(object sender, EventArgs e);
    public static event HitEventHandler OnHit;

    public delegate void AbuctionCompleteEventHandler(object sender, EventArgs e);
    public static event AbuctionCompleteEventHandler OnAbductionComplete;


    public enum CommandShipState { Idle, PreparingToLaunch, Flying, Hit, Destroyed, Abducting };


    public CommandShipState State
    {
        get { return _commandShipState; }
    }


    private void Abduct(object sender, PlayerAbductionEventArgs e)
    {
        _commandShipState = CommandShipState.Abducting;

        _animator.SetBool("isAbductingPlayer", true);

        _commandShipPlayerAbductionController.SetTarget(e.Player.gameObject);

        _audioSource.Play();
    }

    private void AbductionComplete()
    {
        Respawn();

        if (OnAbductionComplete != null)
        {
            OnAbductionComplete(this, EventArgs.Empty);
        }
    }

    private void AnimationComplete(object sender, AnimationCompleteEventArgs e)
    {
        if (e.AnimationClipType == AnimationEventController.AnimationClipType.CommandShipDestroyed)
        {
            Die();
        }
    }

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _animationEventController = GetComponent<AnimationEventController>();
        _commandShipMovementController = GetComponent<CommandShipMovementController>();
        _commandShipPlayerAbductionController = GetComponent<CommandShipPlayerAbductionController>();

        Respawn();
    }

    private void Die()
    {
        _commandShipState = CommandShipState.Destroyed;

        Respawn();
    }

    private void DisableLaunch()
    {
        _canLaunch = false;
    }

    private void DisableLaunch(object sender, EventArgs e)
    {
        DisableLaunch();
    }

    private void EnableLaunch(object sender, EventArgs e)
    {
        _canLaunch = true;
    }

    private void Hit()
    {
        _commandShipState = CommandShipState.Hit;

        _animator.SetBool("isAlive", false);

        _polygonCollider2D.enabled = false;

        if (OnHit != null)
        {
            OnHit(this, EventArgs.Empty);
        }
    }

    private void Launch(object sender, EventArgs e)
    {
        if (_canLaunch)
        {
            _commandShipState = CommandShipState.Flying;

            _audioSource.Play();
        }
    }

    private void OnDisable()
    {
        _animationEventController.OnAnimationComplete -= AnimationComplete;
        GameController.OnGameOver -= DisableLaunch;
        GameController.OnGameStarted -= EnableLaunch;
        GameController.OnInvaderFormationFirstDescent -= DisableLaunch;
        GameController.OnInvaderFormationHalfDestroyed -= Launch;
        GameController.OnInvaderFormationLanded -= DisableLaunch;
        GameController.OnPlayerAbduction -= Abduct;
        GameController.OnPlayerAbductionComplete -= EnableLaunch;
        GameController.OnWaveComplete -= EnableLaunch;
    }

    private void OnEnable()
    {
        _animationEventController.OnAnimationComplete += AnimationComplete;
        GameController.OnGameOver += DisableLaunch;
        GameController.OnGameStarted += EnableLaunch;
        GameController.OnInvaderFormationFirstDescent += DisableLaunch;
        GameController.OnInvaderFormationHalfDestroyed += Launch;
        GameController.OnInvaderFormationLanded += DisableLaunch;
        GameController.OnPlayerAbduction += Abduct;
        GameController.OnPlayerAbductionComplete += EnableLaunch;
        GameController.OnWaveComplete += EnableLaunch;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        switch (_commandShipState)
        {
            case CommandShipState.Flying:
                {
                    if (projectile)
                    {
                        DisableLaunch();
                        Hit();
                    }
                    else if (collision.gameObject.layer == LayerMask.NameToLayer("Shredder"))
                    {
                        DisableLaunch();
                        Die();
                    }

                    break;
                }
            case CommandShipState.Abducting:
                {
                    if (collision.gameObject.layer == LayerMask.NameToLayer("Shredder"))
                    {
                        AbductionComplete();
                    }

                    break;
                }
        }
    }

    private void Respawn()
    {
        _commandShipState = CommandShipState.Idle;

        gameObject.transform.position = _configuration.CommandShipSpawnPosition;

        _audioSource.pitch = _configuration.CommandShipDefaultAudioPitch;
        _audioSource.Stop();

        _animator.SetBool("isAlive", true);
        _animator.SetBool("isAbductingPlayer", false);

        _polygonCollider2D.enabled = true;
    }
}