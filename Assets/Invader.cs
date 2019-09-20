using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
[RequireComponent(typeof(AudioSource))]
[RequireComponent(typeof(AnimationEventController))]
public class Invader : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer = null;
    private BoxCollider2D _boxCollider2D = null;
    private AudioSource _audioSource = null;
    private AnimationEventController _animationEventController = null;

    private Color32 _color = default;
    private bool _hasLaunched = false;


    public delegate void DestroyedEventHandler(object sender, InvaderDestroyedEventArgs e);
    public event DestroyedEventHandler OnDestroyed;

    public delegate void HitEventHandler(object sender, EventArgs e);
    public event HitEventHandler OnHit;


    public bool HasLaunched
    {
        get { return _hasLaunched; }
    }

    public Color32 Color
    {
        get { return _color; }
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
        _boxCollider2D = GetComponent<BoxCollider2D>();
        _audioSource = GetComponent<AudioSource>();
        _animationEventController = GetComponent<AnimationEventController>();

        _color = _spriteRenderer.color;
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
        _boxCollider2D.enabled = false;
        _audioSource.Play();

        if (OnHit != null)
        {
            OnHit(this, System.EventArgs.Empty);
        }
    }

    private void OnDisable()
    {
        _animationEventController.OnAnimationComplete -= AnimationComplete;
    }

    private void OnEnable()
    {
        _animationEventController.OnAnimationComplete += AnimationComplete;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Projectile projectile = collision.gameObject.GetComponent<Projectile>();
        InvaderMotherShip invaderMotherShip = collision.gameObject.GetComponent<InvaderMotherShip>();

        if (projectile)
        {
            Hit();
        }

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
}