using System;

public class InvaderFormationInvaderHitEventArgs : EventArgs
{
    private int _invadersHit = 0;

    public InvaderFormationInvaderHitEventArgs(int invadersHit)
    {
        _invadersHit = invadersHit;
    }


    public int InvadersHit
    {
        get { return _invadersHit; }
    }
}