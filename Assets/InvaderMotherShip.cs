using System;
using System.Collections;

using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class InvaderMotherShip : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private InvaderFormation _invaderFormationPrefab = null;

    private InvaderFormation _invaderFormation = null;
    private bool _hasLanded = false;


    public delegate void DescentCompleteEventHandler(object sender, EventArgs e);
    public event DescentCompleteEventHandler OnDescentComplete;

    public delegate void LandedEventHandler(object sender, EventArgs e);
    public event LandedEventHandler OnLanded;


    public void CeaseFire()
    {
        if(_invaderFormation)
        {
            _invaderFormation.GetComponent<InvaderFormationAttackController>().enabled = false;
        }
    }

    public void HaltAttack()
    {
        if (_invaderFormation)
        {
            _invaderFormation.GetComponent<InvaderFormationMovementController>().enabled = false;
            _invaderFormation.GetComponent<InvaderFormationAttackController>().enabled = false;
        }
    }

    public void ResumeAttack()
    {
        if (_invaderFormation)
        {
            _invaderFormation.GetComponent<InvaderFormationMovementController>().enabled = true;
            _invaderFormation.GetComponent<InvaderFormationAttackController>().enabled = true;
        }
    }

    private void DescendAndLaunchInvaderFormation(object sender, EventArgs e)
    {
        if (!_hasLanded)
        {
            StartCoroutine(Descend());
        }
        else
        {

            // the following should only happen AFTER the commandship sequence

            // launches invaders after the mothership has landed
            // how to fit in the commandship sequence?
            PrepareToLaunchInvaderFormation();

            // repositions player - dont want this to happen for the commandship sequence
            if (OnDescentComplete != null)
            {
                OnDescentComplete(this, EventArgs.Empty);
            }
        }
    }

    private IEnumerator Descend()
    {
        bool arrived = false;
        Vector3 targetPosition = new Vector2(transform.position.x, transform.position.y - _configuration.MotherShipDescentStep);

        yield return new WaitForSeconds(_configuration.MotherShipDescentInitialDelay);

        while (!arrived)
        {
            if (transform.position == targetPosition)
            {
                arrived = true;

                if (!_hasLanded)
                {
                    PrepareToLaunchInvaderFormation();

                    if(OnDescentComplete != null)
                    {
                        OnDescentComplete(this, EventArgs.Empty);
                    }
                }

                yield return null;
            }
            else
            {
                transform.position = new Vector3(transform.position.x, transform.position.y - (_configuration.MotherShipDescentStep / _configuration.MotherShipStepsPerDescent));

                yield return new WaitForSeconds(_configuration.MotherShipDescentIterativeDelay);
            }
        }
    }

    private void DestroyExistingInvaderFormation()
    {
        if (_invaderFormation)
        {
            Destroy(_invaderFormation.gameObject);
        }
    }

    private IEnumerator DelayLaunchOfInvaderFormation(float launchDelay)
    {
        yield return new WaitForSeconds(launchDelay);

        LaunchInvaderFormation();
    }

    private void InitiateAttack(object sender, EventArgs e)
    {
        // TOOD: After testing Abduction sequence - uncomment the following line
        // transform.position = _configuration.MotherShipSpawnPosition;

        DestroyExistingInvaderFormation();

        PrepareToLaunchInvaderFormation();
    }

    private void LaunchInvaderFormation()
    {
        Vector2 launchPosition = (Vector2)transform.position + _configuration.InvaderFormationSpawnPositionOffSet;

        _invaderFormation = Instantiate(_invaderFormationPrefab, launchPosition, Quaternion.identity);

        // TODO: Could throw an error due to assumption of component
        _invaderFormation.GetComponent<InvaderFormationMovementController>().Velocity = _configuration.InvaderFormationVelocity;

        // TODO: Could use the Game parameter (overloaded method of same name) to set the invader formations shooting pattern
    }

    private void OnDisable()
    {
        GameController.OnGameStarted -= InitiateAttack;
        GameController.OnPlayerAbductionComplete -= InitiateAttack;
        InvaderFormation.OnFormationDestroyed -= DescendAndLaunchInvaderFormation;
    }

    private void OnEnable()
    {
        GameController.OnGameStarted += InitiateAttack;
        GameController.OnPlayerAbductionComplete += InitiateAttack;
        InvaderFormation.OnFormationDestroyed += DescendAndLaunchInvaderFormation;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PlayspaceBoundary playspaceBoundary = collision.gameObject.GetComponent<PlayspaceBoundary>();

        if (playspaceBoundary && playspaceBoundary.Type == PlayspaceBoundary.BoundaryType.Bottom)
        {
            if (!_hasLanded)
            {
                Land();
            }
        }
    }

    private void Land()
    {
        _hasLanded = true;

        if (OnLanded != null)
        {
            OnLanded(this, EventArgs.Empty);
        }
    }

    private void PrepareToLaunchInvaderFormation()
    {
        StartCoroutine(DelayLaunchOfInvaderFormation(1f));
    }
}