using System;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Player))]
public class PlayerAttackController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private GameObject _projectilePrefab = null;

    private Color _playerColor;
    private bool _canAttack = false;
    private bool _isReloading = false;


    private void Attack()
    {
        if (_canAttack && Input.GetButton("Fire") && !_isReloading)
        {
            _isReloading = true;

            GameObject projectile = Instantiate(_projectilePrefab, gameObject.transform.position, Quaternion.identity);

            projectile.GetComponent<Projectile>().OnDestroyed += Reloaded;
            projectile.GetComponent<Projectile>().Velocity = _configuration.PlayerProjectileVelocity;
            projectile.GetComponent<SpriteRenderer>().color = new Color(_playerColor.r, _playerColor.g, _playerColor.b, _playerColor.a);
        }
    }

    private void Awake()
    {
        _playerColor = GetComponent<SpriteRenderer>().color;
    }

    private void DisableAttacking(object sender, EventArgs e)
    {
        _canAttack = false;
    }

    private void EnableAttacking(object sender, EventArgs e)
    {
        _canAttack = true;
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= DisableAttacking;
        GameController.OnGameStarted -= EnableAttacking;
        GameController.OnPlayerAbduction -= DisableAttacking;
        GameController.OnPlayerAbductionComplete -= EnableAttacking;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += DisableAttacking;
        GameController.OnGameStarted += EnableAttacking;
        GameController.OnPlayerAbduction += DisableAttacking;
        GameController.OnPlayerAbductionComplete += EnableAttacking;
    }

    private void Reloaded(object sender, ProjectileDestroyedEventArgs e)
    {
        _isReloading = false;

        e.Projectile.OnDestroyed -= Reloaded;
    }

    private void Update()
    {
        Attack();
    }
}