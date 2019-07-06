using System;

using UnityEngine;

public class InvaderFormationVelocityIncreasedEventArgs : EventArgs
{
    private Vector2 _velocity = Vector2.zero;


    public InvaderFormationVelocityIncreasedEventArgs(Vector2 velocity)
    {
        _velocity = velocity;
    }


    public Vector2 Velocity 
    {
        get { return _velocity; }
    }
}