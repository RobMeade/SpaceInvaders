using System;

using UnityEngine;

[RequireComponent(typeof(InvaderFormation))]
public class InvaderFormationAttackController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private GameObject _projectilePrefab = null;

    private InvaderFormation _invaderFormation = null;

    private GameObject _target = null;

    private bool _canAttack = false;
    private bool _canReArm = false;
    private bool _reArmed = false;

    private float _timeToReArm = 0f;
    private float _fireRate = 0f;


    private void Attack()
    {
        bool targetWithinRange = false;

        foreach (InvaderFormationColumn column in _invaderFormation.Columns)
        {
            Invader invader = column.ClosestInvaderToLunarSurface;

            if (invader)
            {
                float lowerColumnBoundsX = invader.transform.position.x - 0.5f;
                float upperColumnBoundsX = invader.transform.position.x + 0.5f;

                if (_target.transform.position.x >= lowerColumnBoundsX && _target.transform.position.x <= upperColumnBoundsX)
                {
                    LaunchProjectile(invader);

                    targetWithinRange = true;

                    break;
                }
            }
        }

        if (!targetWithinRange)
        {
            Invader invader = GetClosestInvadeToLunarSurfaceFromRandomColumn();

            LaunchProjectile(invader);
        }
    }

    private void Awake()
    {
        _invaderFormation = GetComponent<InvaderFormation>();

        // TODO: Temp - get reference to player - don't like this as its knows about the "player" etc
        _target = GameObject.FindObjectOfType<Player>().gameObject;
    }

    private Invader GetClosestInvadeToLunarSurfaceFromRandomColumn()
    {
        int columnIndex = UnityEngine.Random.Range(0, (_invaderFormation.Columns.Count - 1));

        return _invaderFormation.Columns[columnIndex].ClosestInvaderToLunarSurface;
    }

    private void LaunchProjectile(Invader invader)
    {
        if (invader && invader.HasLaunched)
        {
            _canReArm = false;
            _reArmed = false;

            GameObject projectile = Instantiate(_projectilePrefab, invader.transform.position, Quaternion.identity);

            projectile.GetComponent<Projectile>().OnDestroyed += ProjectileDestroyed;
            projectile.GetComponent<Projectile>().Velocity = _configuration.InvaderProjectileVelocity;
            projectile.GetComponent<SpriteRenderer>().color = invader.Color;
        }
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= StopAttacking;
        GameController.OnInvaderFormationLanded -= StopAttacking;
        InvaderFormation.OnCeaseFire -= StopAttacking;
        InvaderFormation.OnHaltAttack -= StopAttacking;
        InvaderFormation.OnResumeAttack -= StartAttacking;
        InvaderFormationMovementController.OnLaunch -= StartAttacking;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += StopAttacking;
        GameController.OnInvaderFormationLanded += StopAttacking;
        InvaderFormation.OnCeaseFire += StopAttacking;
        InvaderFormation.OnHaltAttack += StopAttacking;
        InvaderFormation.OnResumeAttack += StartAttacking;
        InvaderFormationMovementController.OnLaunch += StartAttacking;
    }

    private void PrepareToAttack()
    {
        _fireRate = UnityEngine.Random.Range(_configuration.InvaderMinimumReArmDelay, _configuration.InvaderMaximumReArmDelay);

        _timeToReArm = Time.time + _fireRate;
    }

    private void ProjectileDestroyed(object sender, ProjectileDestroyedEventArgs e)
    {
        _canReArm = true;

        e.Projectile.OnDestroyed -= ProjectileDestroyed;

        PrepareToAttack();
    }

    private void StartAttacking()
    {
        StartAttacking(this, EventArgs.Empty);
    }

    private void StartAttacking(object sender, EventArgs e)
    {
        _canAttack = true;
        _canReArm = true;

        PrepareToAttack();
    }

    private void StopAttacking(object sender, EventArgs e)
    {
        _canAttack = false;
    }

    private void Update()
    {
        if (_canAttack && _canReArm && Time.time > _timeToReArm)
        {
            _reArmed = true;
        }

        if (_canAttack && _reArmed && _invaderFormation.Invaders.Count > 0)
        {
            Attack();
        }
    }
}