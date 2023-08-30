using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
[CreateAssetMenu(menuName = "Grid Direction Data")]
public class GridDirectionData : ScriptableObject
{
    [SerializeField] public List<GridDirection> gridDirections = new List<GridDirection>();
}

[System.Serializable]
public class GridDirection
{
    [SerializeField] public DirectionOption directionOption;
    [SerializeField] public Texture2D texture;
}
