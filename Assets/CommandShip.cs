using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(CommandShipMovementController))]
[RequireComponent(typeof(CommandShipPlayerAbductionController))]
[RequireComponent(typeof(AnimationEventController))]
public class CommandShip : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private Animator _animator = null;
    private PolygonCollider2D _polygonCollider2D = null;
    private CommandShipMovementController _commandShipMovementController = null;
    private CommandShipPlayerAbductionController _commandShipPlayerAbductionController = null;
    private AnimationEventController _animationEventController = null;

    private CommandShipState _commandShipState = CommandShipState.Idle;


    public delegate void HitEventHandler(object sender, EventArgs e);
    public static event HitEventHandler OnHit;

    public delegate void AbuctionCompleteEventHandler(object sender, EventArgs e);
    public static event AbuctionCompleteEventHandler OnAbductionComplete;


    public enum CommandShipState { Idle, PreparingToLaunch, Flying, Hit, Abducting };


    public CommandShipState State
    {
        get { return _commandShipState; }
    }


    private void Abduct(object sender, PlayerAbductionEventArgs e)
    {
        // TODO: Consider that a command ship may already be "flying" and we should wait until it isn't

        _commandShipState = CommandShipState.Abducting;

        gameObject.transform.position = _configuration.CommandShipSpawnPosition;

        _animator.SetBool("isAbductingPlayer", true);

        _commandShipPlayerAbductionController.SetTarget(e.Player.gameObject);
    }

    private void AbductionComplete()
    {
        _commandShipState = CommandShipState.Idle;

        gameObject.transform.position = _configuration.CommandShipSpawnPosition;

        _animator.SetBool("isAbductingPlayer", false);

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
        _commandShipMovementController = GetComponent<CommandShipMovementController>();
        _commandShipPlayerAbductionController = GetComponent<CommandShipPlayerAbductionController>();
        _animationEventController = GetComponent<AnimationEventController>();

        _animator.SetBool("isAlive", true);
    }

    private void Die()
    {
        StartCoroutine(PrepareForLaunch());
    }

    private void DisableLaunch(object sender, EventArgs e)
    {
        _commandShipState = CommandShipState.Idle;

        StopAllCoroutines();
    }

    private void EnableLaunch(object sender, EventArgs e)
    {
        StartCoroutine(PrepareForLaunch());
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

    private void Launch()
    {
        _commandShipState = CommandShipState.Flying;
    }

    private void OnDisable()
    {
        _animationEventController.OnAnimationComplete -= AnimationComplete;
        GameController.OnGameOver -= DisableLaunch;
        GameController.OnGameStarted -= EnableLaunch;
        GameController.OnPlayerAbduction -= Abduct;
        GameController.OnPlayerAbductionComplete -= EnableLaunch;
    }

    private void OnEnable()
    {
        _animationEventController.OnAnimationComplete += AnimationComplete;
        GameController.OnGameOver += DisableLaunch;
        GameController.OnGameStarted += EnableLaunch;
        GameController.OnPlayerAbduction += Abduct;
        GameController.OnPlayerAbductionComplete += EnableLaunch;
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
                        Hit();
                    }
                    else if (collision.gameObject.layer == LayerMask.NameToLayer("Shredder"))
                    {
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

    private IEnumerator PrepareForLaunch()
    {
        _commandShipState = CommandShipState.PreparingToLaunch;

        gameObject.transform.position = _configuration.CommandShipSpawnPosition;

        _animator.SetBool("isAlive", true);

        _polygonCollider2D.enabled = true;

        float launchTimer;

        while (_commandShipState == CommandShipState.PreparingToLaunch)
        {
            launchTimer = UnityEngine.Random.Range(_configuration.CommandShipMinimumSpawnDelay, _configuration.CommandShipMaximumSpawnDelay);

            yield return new WaitForSeconds(launchTimer);

            if (_commandShipState == CommandShipState.PreparingToLaunch)
            {
                Launch();
                StopAllCoroutines();
            }
        }
    }
}