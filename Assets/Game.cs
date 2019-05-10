using UnityEngine;

[CreateAssetMenu(menuName = "Space Invaders/Game", order = 2)]
public class Game : ScriptableObject
{
    [SerializeField]
    private int _number = 0;

    [SerializeField]
    private int _playerLives = 0;

    [SerializeField]
    private GameController.InvaderProjectileType _invaderProjectileType = GameController.InvaderProjectileType.Slow;

    [SerializeField]
    private bool _invaderHomeInProjectiles = false;


    public int Number
    {
        get { return _number; }
    }

    public int PlayerLives
    {
        get { return _playerLives; }
    }

    public GameController.InvaderProjectileType InvaderProjectileType
    {
        get { return _invaderProjectileType; }
    }

    public bool InvaderHomeInProjectiles
    {
        get { return _invaderHomeInProjectiles; }
    }
}