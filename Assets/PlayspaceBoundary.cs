using UnityEngine;

public class PlayspaceBoundary : MonoBehaviour
{
    public enum BoundaryType : int { None, Left, Right, Bottom };


    [SerializeField]
    private BoundaryType _type = BoundaryType.None;


    public BoundaryType Type
    {
        get { return _type; }
    }
}