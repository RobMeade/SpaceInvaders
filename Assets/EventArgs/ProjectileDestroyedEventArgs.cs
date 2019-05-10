using System;

public class ProjectileDestroyedEventArgs : EventArgs
{
    private Projectile _projectile;


    public ProjectileDestroyedEventArgs(Projectile projectile)
    {
        _projectile = projectile;
    }


    public Projectile Projectile
    {
        get { return _projectile; }
    }
}