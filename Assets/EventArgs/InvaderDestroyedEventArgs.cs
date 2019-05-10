using System;

public class InvaderDestroyedEventArgs : EventArgs
{
    private Invader _invader = null;


    public InvaderDestroyedEventArgs(Invader invader)
    {
        _invader = invader;
    }


    public Invader Invader
    {
        get { return _invader; }
    }
}