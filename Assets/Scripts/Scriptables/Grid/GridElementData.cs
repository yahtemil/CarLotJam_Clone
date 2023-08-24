using UnityEngine;

[CreateAssetMenu(menuName = "Grid Element Data")]
public class GridElementData : ScriptableObject
{
    public GridElement elementType;
    public bool ClearArea;
    public DirectionOption selectedDirection = DirectionOption.Up;
    public ColorOption cellColorOptions;
    public GameObject prefab;
}
public enum GridElement
{
    None,
    Obstacle1x1,
    Obstacle1x2,
    Car1x2,
    Car1x3,
    Stickman
}

public enum DirectionOption
{
    Up,
    Down,
    Left,
    Right
}