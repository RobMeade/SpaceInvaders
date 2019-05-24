using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(PolygonCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AnimationEventController))]
[RequireComponent(typeof(PlayerAttackController))]
[RequireComponent(typeof(PlayerMovementController))]
public class Player : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private int _lives = 0;
    private int _score = 0;

    private SpriteRenderer _spriteRenderer = null;
    private Animator _animator = null;
    private PolygonCollider2D _polygonCollider2D = null;
    private AudioSource _audioSource = null;
    private AnimationEventController _animationEventController = null;
    private PlayerAttackController _playerShooting = null;
    private PlayerMovementController _playerMovement = null;

    private Color32 _color;


    public delegate void DestroyedEventHandler(object sender, EventArgs e);
    public event DestroyedEventHandler OnDestroyed;

    public delegate void HitEventHandler(object sender, EventArgs e);
    public event HitEventHandler OnHit;


    public int Lives
    {
        get { return _lives; }
        set { _lives = value; }
    }

    public int Score
    {
        get { return _score; }
        set { _score = value; }
    }


    public void Reposition()
    {
        Reposition(this, EventArgs.Empty);
    }

    private void AnimationComplete(object sender, AnimationCompleteEventArgs e)
    {
        if (e.AnimationClipType == AnimationEventController.AnimationClipType.PlayerDestroyed)
        {
            Die();
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _polygonCollider2D = GetComponent<PolygonCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _animationEventController = GetComponent<AnimationEventController>();
        _playerShooting = GetComponent<PlayerAttackController>();
        _playerMovement = GetComponent<PlayerMovementController>();

        _animator.SetBool("isAlive", true);

        _color = _spriteRenderer.color;
    }

    private void Die()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(this, EventArgs.Empty);
        }

        Invoke(nameof(Respawn), _configuration.PlayerRespawnDelay);
    }

    private void Hit(Color32 projectileColor)
    {
        _lives--;

        _spriteRenderer.color = projectileColor;
        _animator.SetBool("isAlive", false);
        _polygonCollider2D.enabled = false;
        _playerShooting.enabled = false;
        _playerMovement.enabled = false;

        _audioSource.Play();

        if (OnHit != null)
        {
            OnHit(this, EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= Respawn;
        GameController.OnPlayerAbductionComplete -= Reposition;
        _animationEventController.OnAnimationComplete -= AnimationComplete;
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += Respawn;
        GameController.OnPlayerAbductionComplete += Reposition;
        _animationEventController.OnAnimationComplete += AnimationComplete;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        if (projectile)
        {
            Color32 projectileColor = collision.gameObject.GetComponent<SpriteRenderer>().color;

            Hit(projectileColor);
        }
    }

    private void Reposition(object sender, EventArgs e)
    {
        gameObject.transform.parent = null;
        gameObject.transform.position = _configuration.PlayerSpawnPosition;
    }

    private void Respawn()
    {
        Respawn(this, EventArgs.Empty);
    }

    private void Respawn(object sender, EventArgs e)
    {
        _animator.SetBool("isAlive", true);
        _spriteRenderer.color = _color;

        if (_lives > 0)
        {
            _polygonCollider2D.enabled = true;
            _playerShooting.enabled = true;
            _playerMovement.enabled = true;
        }

        Reposition();
    }
}