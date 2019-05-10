using System;

public class PlayerAbductionEventArgs : EventArgs
{
    private Player _player = null;


    public PlayerAbductionEventArgs(Player player)
    {
        _player = player;
    }


    public Player Player
    {
        get { return _player; }
    }
}