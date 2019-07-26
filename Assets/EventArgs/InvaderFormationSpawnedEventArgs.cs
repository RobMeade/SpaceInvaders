using System;

public class InvaderFormationSpawnedEventArgs : EventArgs
{
    private int _descentsPerformed = 0;


    public InvaderFormationSpawnedEventArgs(int descentsPerformed)
    {
        _descentsPerformed = descentsPerformed;
    }


    public int DescentsPerformed
    {
        get { return _descentsPerformed; }
    }
}