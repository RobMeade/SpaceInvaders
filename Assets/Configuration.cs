﻿using UnityEngine;

[CreateAssetMenu(menuName = "Space Invaders/Configuration", order = 1)]
public class Configuration : ScriptableObject
{
    [Header("Player")]
    [SerializeField]
    private Vector2 _playerSpawnPosition = new Vector2(2.5f, 1.375f);

    [SerializeField]
    private float _playerRespawnDelay = 0.5f;

    [SerializeField]
    private Vector2 _playerVelocity = new Vector2(5f, 0f);

    [SerializeField]
    private float _playerBoundsMinimumX = 1.5f;

    [SerializeField]
    private float _playerBoundsMaximumX = 9.5f;

    [SerializeField]
    private Vector2 _playerProjectileVelocity = new Vector2(0f, 1f);

    
    [Header("Command Ship")]
    [SerializeField]
    private Vector2 _commandShipSpawnPosition = new Vector2(-1f, 6.75f);

    [SerializeField]
    private float _commandShipMinimumSpawnDelay = 12f;

    [SerializeField]
    private float _commandShipMaximumSpawnDelay = 24f;

    [SerializeField]
    private Vector2 _commandShipVelocity = new Vector2(1.75f, 1.75f);

    [SerializeField]
    private int _commandShipDestroyedPoints = 18;


    [Header("Invader")]
    [SerializeField]
    private float _invaderMinimumReArmDelay = 1f;

    [SerializeField]
    private float _invaderMaximumReArmDelay = 1.5f;

    [SerializeField]
    private Vector2 _invaderProjectileVelocity = new Vector2(0f, -1f);

    [SerializeField]
    private int _invaderDestroyedPoints = 2;

    [SerializeField]
    private int _invaderDescentPointsMultiplier = 2;


    [Header("Invader Formation")]
    [SerializeField]
    private Vector2 _invaderFormationSpawnPositionOffSet = new Vector2(-4f, 0f);

    [SerializeField]
    private Vector2 _invaderFormationVelocity = new Vector2(1f, 0.25f);


    [Header("Mother Ship")]
    [SerializeField]
    private Vector2 _motherShipSpawnPosition = new Vector2(0.75f, 5f);

    [SerializeField]
    private float _motherShipDescentInitialDelay = 2f;

    [SerializeField]
    private float _motherShipDescentIterativeDelay = 1f;

    [SerializeField]
    private float _motherShipDescentStep = 0.25f;

    [SerializeField]
    private int _motherShipStepsPerDescent = 4;


    [Header("Miscellaneous")]
    [SerializeField]
    private float _copyrightTextScrollSpeed = 2f;


    // player
    public Vector2 PlayerSpawnPosition
    {
        get { return _playerSpawnPosition; }
    }

    public float PlayerRespawnDelay
    {
        get { return _playerRespawnDelay; }
    }

    public Vector2 PlayerVelocity
    {
        get { return _playerVelocity; }
    }

    public float PlayerBoundsMinimumX
    {
        get { return _playerBoundsMinimumX; }
    }

    public float PlayerBoundsMaximumX
    {
        get { return _playerBoundsMaximumX; }
    }

    public Vector2 PlayerProjectileVelocity
    {
        get { return _playerProjectileVelocity; }
    }


    // command ship
    public Vector2 CommandShipSpawnPosition
    {
        get { return _commandShipSpawnPosition; }
    }

    public Vector2 CommandShipVelocity
    {
        get { return _commandShipVelocity; }
    }

    public float CommandShipMinimumSpawnDelay
    {
        get { return _commandShipMinimumSpawnDelay; }
    }

    public float CommandShipMaximumSpawnDelay
    {
        get { return _commandShipMaximumSpawnDelay; }
    }

    public int CommandShipDestroyedPoints
    {
        get { return _commandShipDestroyedPoints; }
    }
       

    // invader
    public float InvaderMinimumReArmDelay
    {
        get { return _invaderMinimumReArmDelay; }
    }

    public float InvaderMaximumReArmDelay
    {
        get { return _invaderMaximumReArmDelay; }
    }

    public Vector2 InvaderProjectileVelocity
    {
        get { return _invaderProjectileVelocity; }
    }

    public int InvaderDestroyedPoints
    {
        get { return _invaderDestroyedPoints; }
    }

    public int InvaderDescentPointsMultiplier
    {
        get { return _invaderDescentPointsMultiplier; }
    }


    // invader formation
    public Vector2 InvaderFormationSpawnPositionOffSet
    {
        get { return _invaderFormationSpawnPositionOffSet; }
    }

    public Vector2 InvaderFormationVelocity
    {
        get { return _invaderFormationVelocity; }
    }


    // mother ship
    public Vector2 MotherShipSpawnPosition
    {
        get { return _motherShipSpawnPosition; }
    }

    public float MotherShipDescentInitialDelay
    {
        get { return _motherShipDescentInitialDelay; }
    }

    public float MotherShipDescentIterativeDelay
    {
        get { return _motherShipDescentIterativeDelay; }
    }

    public float MotherShipDescentStep
    {
        get { return _motherShipDescentStep; }
    }

    public int MotherShipStepsPerDescent
    {
        get { return _motherShipStepsPerDescent; }
    }


    // miscellaneous
    public float CopyrightTextScrollSpeed
    {
        get { return _copyrightTextScrollSpeed; }
    }
}