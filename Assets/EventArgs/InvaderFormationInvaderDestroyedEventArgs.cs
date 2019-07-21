using System;

public class InvaderFormationInvaderDestroyedEventArgs : EventArgs
{
    private int _invadersPerWave = 0;
    private int _invadersHit = 0;
    private int _remainingInvaders = 0;


    public InvaderFormationInvaderDestroyedEventArgs(int invadersPerWave, int invadersHit, int remainingInvaders)
    {
        _invadersPerWave = invadersPerWave;
        _invadersHit = invadersHit;
        _remainingInvaders = remainingInvaders;
    }


    public int InvadersPerWave
    {
        get { return _invadersPerWave; }
    }

    public int InvadersHit
    {
        get { return _invadersHit; }
    }

    public int RemainingInvaders
    {
        get { return _remainingInvaders; }
    }
}