using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AnimationEventController))]
public class Invader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer = null;
    private Animator _animator = null;
    private BoxCollider2D _boxCollider2D = null;
    private AudioSource _audioSource = null;
    private AnimationEventController _animationEventController = null;

    private bool _hasLaunched = false;


    public delegate void DestroyedEventHandler(object sender, InvaderDestroyedEventArgs e);
    public event DestroyedEventHandler OnDestroyed;

    public delegate void HitEventHandler(object sender, System.EventArgs e);
    public event HitEventHandler OnHit;


    public bool HasLaunched
    {
        get { return _hasLaunched; }
    }


    private void AnimationComplete(object sender, AnimationCompleteEventArgs e)
    {
        if (e.AnimationClipType == AnimationEventController.AnimationClipType.InvaderDestroyed)
        {
            Die();
        }
    }

    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _animator = GetComponent<Animator>();
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _animationEventController = GetComponent<AnimationEventController>();

        _animator.SetBool("isAlive", true);
    }

    private void Die()
    {
        if (OnDestroyed != null)
        {
            OnDestroyed(this, new InvaderDestroyedEventArgs(this));
        }

        Destroy(gameObject);
    }

    private void Hit()
    {
        _animator.SetBool("isAlive", false);
        _boxCollider2D.enabled = false;
        _audioSource.Play();

        if (OnHit != null)
        {
            OnHit(this, System.EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= StopAnimation;
        _animationEventController.OnAnimationComplete -= AnimationComplete;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += StopAnimation;
        _animationEventController.OnAnimationComplete += AnimationComplete;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();

        if (projectile)
        {
            Hit();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        InvaderMotherShip invaderMotherShip = collision.gameObject.GetComponent<InvaderMotherShip>();

        if (invaderMotherShip)
        {
            _hasLaunched = true;
            _spriteRenderer.enabled = true;
        }
    }

    private void Start()
    {
        _spriteRenderer.enabled = false;
    }

    private void StopAnimation(object sender, EventArgs e)
    {
        if (_animator.GetCurrentAnimatorStateInfo(0).IsName("Moving"))
        {
            _animator.enabled = false;
        }
    }
}