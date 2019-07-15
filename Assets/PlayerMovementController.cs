using System;

using UnityEngine;

[RequireComponent(typeof(Player))]
public class PlayerMovementController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    private float _deltaX = 0f;
    private float _newXPosition = 0f;

    private bool _canMove = false;


    private void DisableMovement(object sender, EventArgs e)
    {
        _canMove = false;
    }

    private void EnableMovement(object sender, EventArgs e)
    {
        _canMove = true;
    }

    private void Move()
    {
        if (_canMove)
        {
            _deltaX = Input.GetAxis("Horizontal") * Time.deltaTime * _configuration.PlayerVelocity.x;

            _newXPosition = Mathf.Clamp(transform.position.x + _deltaX, _configuration.PlayerBoundsMinimumX, _configuration.PlayerBoundsMaximumX);

            transform.position = new Vector2(_newXPosition, transform.position.y);
        }
    }

    private void OnDisable()
    {
        GameController.OnGameOver -= DisableMovement;
        GameController.OnGameStarted -= EnableMovement;
        GameController.OnInvaderFormationLanded -= DisableMovement;
        GameController.OnPlayerAbduction -= DisableMovement;
        GameController.OnPlayerAbductionComplete -= EnableMovement;
    }

    private void OnEnable()
    {
        GameController.OnGameOver += DisableMovement;
        GameController.OnGameStarted += EnableMovement;
        GameController.OnInvaderFormationLanded += DisableMovement;
        GameController.OnPlayerAbduction += DisableMovement;
        GameController.OnPlayerAbductionComplete += EnableMovement;
    }

    private void Update()
    {
        Move();
    }
}