using System;

public class CommandShipAscendEventArgs : EventArgs
{
    private float _step = 0f;


    public CommandShipAscendEventArgs(float step)
    {
        _step = step;
    }


    public float Step
    {
        get { return _step; }
    }
}