using System.Collections.Generic;
using System.Linq;

using UnityEngine;

public class InvaderFormationColumn : MonoBehaviour
{
    private List<Invader> _invaders = null;


    public Invader ClosestInvaderToLunarSurface
    {
        get { return GetClosestInvaderToLunarSurface(); }
    }


    private void Awake()
    {
        _invaders = GetComponentsInChildren<Invader>().ToList<Invader>();

        foreach (Invader invader in _invaders)
        {
            invader.OnDestroyed += InvaderDestroyed;
        }
    }

    private Invader GetClosestInvaderToLunarSurface()
    {
        Invader closestInvader = null;

        foreach (Invader invader in _invaders)
        {
            if (closestInvader)
            {
                if (invader && invader.transform.position.y < closestInvader.transform.position.y)
                {
                    closestInvader = invader;
                }
            }
            else
            {
                closestInvader = invader;
            }
        }

        return closestInvader;
    }

    private void InvaderDestroyed(object sender, InvaderDestroyedEventArgs e)
    {
        e.Invader.OnDestroyed -= InvaderDestroyed;

        _invaders.Remove(e.Invader);

        if (_invaders.Count == 0)
        {
            Destroy(gameObject);
        }
    }
}