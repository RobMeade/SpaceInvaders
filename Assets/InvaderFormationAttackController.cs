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
    private bool _reArmed = false;


    private void Attack()
    {
        bool targetWithinRange = false;
        int laserCannonIndex;

        foreach (InvaderFormationColumn column in _invaderFormation.Columns)
        {
            Invader invader = column.ClosestInvaderToLunarSurface;

            if (invader && invader.HasLaunched)
            {
                float lowerColumnBoundsX = invader.transform.position.x - 0.5f;
                float upperColumnBoundsX = invader.transform.position.x + 0.5f;

                if (_target.transform.position.x >= lowerColumnBoundsX && _target.transform.position.x <= upperColumnBoundsX)
                {
                    laserCannonIndex = UnityEngine.Random.Range(0, invader.LaserCannons.Count);
                    LaunchProjectile(invader.LaserCannons[laserCannonIndex].transform.position, _configuration.InvaderProjectileVelocity, invader.Color);

                    targetWithinRange = true;

                    break;
                }
            }
        }

        if (!targetWithinRange)
        {
            Invader invader = GetClosestInvadeToLunarSurfaceFromRandomColumn();

            if (invader && invader.HasLaunched)
            {
                laserCannonIndex = UnityEngine.Random.Range(0, invader.LaserCannons.Count);
                LaunchProjectile(invader.LaserCannons[laserCannonIndex].transform.position, _configuration.InvaderProjectileVelocity, invader.Color);
            }
        }
    }

    private void Awake()
    {
        _invaderFormation = GetComponent<InvaderFormation>();

        // TODO: Temp - get reference to player - don't like this as its knows about the "player" etc
        _target = GameObject.FindObjectOfType<Player>().gameObject;

        _reArmed = true;
    }

    private Invader GetClosestInvadeToLunarSurfaceFromRandomColumn()
    {
        int columnIndex = UnityEngine.Random.Range(0, _invaderFormation.Columns.Count);

        return _invaderFormation.Columns[columnIndex].ClosestInvaderToLunarSurface;
    }

    private void LaunchProjectile(Vector2 launchPosition, Vector2 launchVelocity, Color32 projectileColor)
    {
        _reArmed = false;

        GameObject projectile = Instantiate(_projectilePrefab, launchPosition, Quaternion.identity);

        projectile.GetComponent<Projectile>().OnDestroyed += ProjectileDestroyed;
        projectile.GetComponent<Projectile>().Velocity = launchVelocity;
        projectile.GetComponent<SpriteRenderer>().color = projectileColor;
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

    private void ProjectileDestroyed(object sender, ProjectileDestroyedEventArgs e)
    {
        _reArmed = true;

        e.Projectile.OnDestroyed -= ProjectileDestroyed;
    }

    private void StartAttacking()
    {
        StartAttacking(this, EventArgs.Empty);
    }

    private void StartAttacking(object sender, EventArgs e)
    {
        _canAttack = true;
    }

    private void StopAttacking(object sender, EventArgs e)
    {
        _canAttack = false;
    }

    private void Update()
    {
        if (_canAttack && _reArmed && _invaderFormation.Invaders.Count > 0)
        {
            Attack();
        }
    }
}