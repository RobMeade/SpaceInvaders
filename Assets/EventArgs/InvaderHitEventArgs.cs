using System;

public class InvaderHitEventArgs : EventArgs
{
    private Invader _invader = null;


    public InvaderHitEventArgs(Invader invader)
    {
        _invader = invader;
    }


    public Invader Invader
    {
        get { return _invader; }
    }
}