using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid System")]
public class GridSystemData : ScriptableObject
{
    public int level = 1;
    public int gridSizeX = 5;
    public int gridSizeY = 5;
    public ColorController colorController;
    public GridDirectionData gridDirectionDatas;
    public string grids;
}
[Serializable]
public class Grids
{
    [SerializeField] public int directionValue = -1;
    [SerializeField] public int gridElementValue = -1;
    [SerializeField] public int colorControllerValue = -1;
    [SerializeField] public int x;
    [SerializeField] public int y;
    [SerializeField] public int otherOneX;
    [SerializeField] public int otherOneY;
    [SerializeField] public int otherTwoX;
    [SerializeField] public int otherTwoY;
    [SerializeField] public int active;
}
