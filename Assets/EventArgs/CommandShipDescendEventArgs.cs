using System;

public class CommandShipDescendEventArgs : EventArgs
{
    private float _step = 0f;


    public CommandShipDescendEventArgs(float step)
    {
        _step = step;
    }


    public float Step
    {
        get { return _step; }
    }
}