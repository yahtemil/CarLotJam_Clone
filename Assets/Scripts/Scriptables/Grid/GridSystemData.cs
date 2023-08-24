using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Grid System")]
public class GridSystemData : ScriptableObject
{
    public int level = 1;
    public int gridSizeX = 5;
    public int gridSizeY = 5;
    public Vector2 cellSize = new Vector2(1f, 1f);
    public GridElementData[] elementData;
    public GridElement[,] grid;
    public ColorController colorController;
    public string grids;
    //public List<GridSaveSystem> gridSaveSystems = new List<GridSaveSystem>();

    public void InitializeGrid()
    {
        grid = new GridElement[gridSizeX, gridSizeY];
        //grids = new Grids[gridSizeX, gridSizeY];
        Debug.Log("sifirlandi");
    }

    public void AddElement(Vector2Int position, GridElement elementType)
    {
        if (position.x < 0 || position.x >= gridSizeX ||
            position.y < 0 || position.y >= gridSizeY)
        {
            Debug.LogWarning("Invalid position.");
            return;
        }

        grid[position.x, position.y] = elementType;
        //grids[position.x, position.y].gridElement = elementType;
    }

    public void RemoveElement(Vector2Int position)
    {
        grid[position.x, position.y] = GridElement.None;
        //grids[position.x, position.y].gridElement = GridElement.None;
    }

    [System.Serializable]
    public class GridCellColorOption
    {
        public ColorOption colorOption;
    }

    private GridCellColorOption[,] cellColorOptions;

    public ColorOption GetColorOptionForCell(int x, int y)
    {
        if (cellColorOptions != null && x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            return cellColorOptions[x, y]?.colorOption;
        }
        return null;
    }

    public void SetColorOptionForCell(int x, int y, ColorOption colorOption)
    {
        if (cellColorOptions == null)
        {
            cellColorOptions = new GridCellColorOption[gridSizeX, gridSizeY];
        }

        if (x >= 0 && x < gridSizeX && y >= 0 && y < gridSizeY)
        {
            cellColorOptions[x, y] = new GridCellColorOption { colorOption = colorOption };
        }
    }
}
