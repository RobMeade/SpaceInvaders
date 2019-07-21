using System;
using System.Collections.Generic;

using UnityEngine;

public class GameController : MonoBehaviour
{
    [SerializeField]
    private Configuration _configuration = null;

    [SerializeField]
    private List<Game> _games = null;

    private Game _game = null;

    public enum InvaderProjectileType { Slow, Fast, SlowAndFast };

    private InvaderMotherShip _invaderMotherShip = null;
    private Player _player = null;
    private UIController _uiController = null;

    private int _invaderDestroyedPoints;
    private int _commandShipDestroyedPoints;

    private int _selectedGameIndex = 0;


    public delegate void InvaderFormationFirstDescentEventHandler(object sender, EventArgs e);
    public static event InvaderFormationFirstDescentEventHandler OnInvaderFormationFirstDescent;

    public delegate void InvaderFormationHalfDestroyedEventHandler(object sender, EventArgs e);
    public static event InvaderFormationHalfDestroyedEventHandler OnInvaderFormationHalfDestroyed;

    public delegate void InvaderFormationLandedEventHandler(object sender, EventArgs e);
    public static event InvaderFormationLandedEventHandler OnInvaderFormationLanded;

    public delegate void GameOverEventHandler(object sender, EventArgs e);
    public static event GameOverEventHandler OnGameOver;

    public delegate void GameStartedEventHandler(object sender, EventArgs e);
    public static event GameStartedEventHandler OnGameStarted;

    public delegate void PlayerAbductionEventHandler(object sender, PlayerAbductionEventArgs e);
    public static event PlayerAbductionEventHandler OnPlayerAbduction;

    public delegate void PlayerAbductionCompleteEventHandler(object sender, EventArgs e);
    public static event PlayerAbductionCompleteEventHandler OnPlayerAbductionComplete;

    public delegate void WaveCompleteEventHandler(object sender, EventArgs e);
    public static event WaveCompleteEventHandler OnWaveComplete;


    private void AbductionComplete(object sender, EventArgs e)
    {
        // TODO: Consider some kind of short delay?

        if (OnPlayerAbductionComplete != null)
        {
            OnPlayerAbductionComplete(this, EventArgs.Empty);
        }
    }

    private void ApplyPointsMultiplier()
    {
        _invaderDestroyedPoints = _configuration.InvaderDestroyedPoints * _configuration.InvaderDescentPointsMultiplier;
    }

    private void Awake()
    {
        _invaderMotherShip = FindObjectOfType<InvaderMotherShip>();
        _player = FindObjectOfType<Player>();
        _uiController = FindObjectOfType<UIController>();

        _commandShipDestroyedPoints = _configuration.CommandShipDestroyedPoints;

        SetGame();
        ResetUI();
    }

    private void CommandShipHit(object sender, EventArgs e)
    {
        _player.Score += _commandShipDestroyedPoints;
        _uiController.SetScore(_player.Score);
    }

    private void GameOver()
    {
        if (OnGameOver != null)
        {
            OnGameOver(this, EventArgs.Empty);
        }
    }

    private void HaltInvaderAttack()
    {
        _invaderMotherShip.HaltAttack();
    }

    private void InvaderDestroyed(object sender, InvaderFormationInvaderDestroyedEventArgs e)
    {
        if (e.RemainingInvaders == (e.InvadersPerWave / 2))
        {
            if(OnInvaderFormationHalfDestroyed != null)
            {
                OnInvaderFormationHalfDestroyed(this, EventArgs.Empty);
            }
        }
    }

    private void InvaderFormationFirstDescent(object sender, EventArgs e)
    {
        ApplyPointsMultiplier();

        if (OnInvaderFormationFirstDescent != null)
        {
            OnInvaderFormationFirstDescent(this, EventArgs.Empty);
        }
    }

    private void InvaderFormationLanded(object sender, EventArgs e)
    {
        if (OnInvaderFormationLanded != null)
        {
            OnInvaderFormationLanded(this, e);
        }
    }

    private void InvaderHit(object sender, InvaderFormationInvaderHitEventArgs e)
    {
        _player.Score += _invaderDestroyedPoints;
        _uiController.SetScore(_player.Score);
    }

    private void OnDisable()
    {
        UIController.OnSystemResetButtonClicked -= QuitGame;
        UIController.OnOptionButtonClicked -= SelectGame;
        UIController.OnSelectButtonClicked -= SelectNumberOfPlayers;
        UIController.OnStartButtonClicked -= StartGame;
        CommandShip.OnHit -= CommandShipHit;
        CommandShip.OnAbductionComplete -= AbductionComplete;
        InvaderFormation.OnFormationDestroyed -= RemovePointsMultiplier;
        InvaderFormation.OnInvaderDestroyed -= InvaderDestroyed;
        InvaderFormation.OnInvaderHit -= InvaderHit;
        InvaderFormationMovementController.OnLanded -= InvaderFormationLanded;
        InvaderFormationMovementController.OnFirstDescent -= InvaderFormationFirstDescent;
        InvaderMotherShip.OnDescentComplete -= MotherShipDescentComplete;
        InvaderMotherShip.OnLanded -= MotherShipLanded;
        _player.OnDestroyed -= PlayerDestroyed;
        _player.OnHit -= PlayerHit;
    }

    private void OnEnable()
    {
        UIController.OnSystemResetButtonClicked += QuitGame;
        UIController.OnOptionButtonClicked += SelectGame;
        UIController.OnSelectButtonClicked += SelectNumberOfPlayers;
        UIController.OnStartButtonClicked += StartGame;
        CommandShip.OnHit += CommandShipHit;
        CommandShip.OnAbductionComplete += AbductionComplete;
        InvaderFormation.OnFormationDestroyed += RemovePointsMultiplier;
        InvaderFormation.OnInvaderDestroyed += InvaderDestroyed;
        InvaderFormation.OnInvaderHit += InvaderHit;
        InvaderFormationMovementController.OnLanded += InvaderFormationLanded;
        InvaderFormationMovementController.OnFirstDescent += InvaderFormationFirstDescent;
        InvaderMotherShip.OnDescentComplete += MotherShipDescentComplete;
        InvaderMotherShip.OnLanded += MotherShipLanded;
        _player.OnDestroyed += PlayerDestroyed;
        _player.OnHit += PlayerHit;
    }

    private void MotherShipLanded(object sender, EventArgs e)
    {
        if (OnPlayerAbduction != null)
        {
            PlayerAbductionEventArgs playerAbductionEventArgs = new PlayerAbductionEventArgs(_player);

            OnPlayerAbduction(this, playerAbductionEventArgs);
        }
    }

    private void PlayerDestroyed(object sender, EventArgs e)
    {
        if (_player.Lives <= 0)
        {
            _uiController.SetLives(_player.Lives);

            GameOver();
        }
        else
        {
            _uiController.SetLives(_player.Lives);

            Invoke(nameof(ResumeInvaderAttack), _configuration.PlayerRespawnDelay);
        }
    }

    private void PlayerHit(object sender, EventArgs e)
    {
        if (_player.Lives <= 0)
        {
            _invaderMotherShip.CeaseFire();
        }
        else
        {
            HaltInvaderAttack();
        }
    }

    private void RemovePointsMultiplier()
    {
        RemovePointsMultiplier(this, EventArgs.Empty);
    }

    private void RemovePointsMultiplier(object sender, EventArgs e)
    {
        _invaderDestroyedPoints = _configuration.InvaderDestroyedPoints;
    }

    private void ResetUI()
    {
        _uiController.SetGame(_game.Number);
        _uiController.SetScore(0);
        _uiController.SetLives(_game.PlayerLives);
    }

    private void MotherShipDescentComplete(object sender, EventArgs e)
    {
        RepositionPlayer();

        if(OnWaveComplete != null)
        {
            OnWaveComplete(this, EventArgs.Empty);
        }
    }

    private void RepositionPlayer()
    {
        _player.Reposition();
    }

    private void ResumeInvaderAttack()
    {
        _invaderMotherShip.ResumeAttack();
    }

    private void SelectGame(object sender, EventArgs e)
    {
        _selectedGameIndex++;

        if (_selectedGameIndex >= _games.Count)
        {
            _selectedGameIndex = 0;
        }

        SetGame();
        ResetUI();
    }

    private void SelectNumberOfPlayers(object sender, EventArgs e)
    {
        // TODO: Will be used for two player support
    }

    private void SetGame()
    {
        _game = _games[_selectedGameIndex];

        _player.Score = 0;
        _player.Lives = _game.PlayerLives;
    }

    private void StartGame(object sender, EventArgs e)
    {
        RemovePointsMultiplier();

        SetGame();
        ResetUI();

        if (OnGameStarted != null)
        {
            OnGameStarted(this, EventArgs.Empty);
        }
    }

    private void QuitGame(object sender, EventArgs e)
    {
#if (UNITY_EDITOR)
        UnityEditor.EditorApplication.isPlaying = false;
#elif (UNITY_STANDALONE)
        Application.Quit();
//#elif (UNITY_WEBGL)
//        Application.OpenURL("about:blank");
#endif
    }
}