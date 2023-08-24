using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;
using System;

[CustomEditor(typeof(GridSystemData))]
public class GridSystemDataEditor : Editor
{
    private SerializedProperty level;
    private SerializedProperty gridSizeX;
    private SerializedProperty gridSizeY;
    //private SerializedProperty cellColorOptions;
    private SerializedProperty grids;

    private GridElement selectedElement = GridElement.None;

    private Dictionary<Vector2Int, ColorOption> cellColorOptions = new Dictionary<Vector2Int, ColorOption>();

    private DirectionOption selectedDirection = DirectionOption.Up;

    GridSystemData gridSystemData;

    private void OnEnable()
    {
        level = serializedObject.FindProperty("level");
        gridSizeX = serializedObject.FindProperty("gridSizeX");
        gridSizeY = serializedObject.FindProperty("gridSizeY");
        grids = serializedObject.FindProperty("grids");
        //cellColorOptions = serializedObject.FindProperty("cellColorOptions");
        //elementData = serializedObject.FindProperty("elementData");
    }

    public override void OnInspectorGUI()
    {
        gridSystemData = target as GridSystemData;

        serializedObject.Update();

        EditorGUILayout.PropertyField(level);
        EditorGUILayout.PropertyField(gridSizeX);
        EditorGUILayout.PropertyField(gridSizeY);

        GUILayout.Space(10);

        (target as GridSystemData).colorController = EditorGUILayout.ObjectField("Color Controller", (target as GridSystemData).colorController, typeof(ColorController), false) as ColorController;

        GUILayout.Space(10);

        //if (GUILayout.Button("Create Grid"))
        //{
        //    gridSystemData.InitializeGrid();
        //    cellColorOptions.Clear();
        //}
        var data = GetLevelData(grids.stringValue, gridSizeX.intValue, gridSizeY.intValue);

        GUILayout.Space(10);

        selectedElement = (GridElement)EditorGUILayout.EnumPopup("Selected Element", selectedElement);

        DrawColorSelector(gridSystemData);

        GUILayout.Space(5);

        data = DrawGrid(data);

        grids.stringValue = JsonHelper.ToJson(data.ToArray());

        serializedObject.ApplyModifiedProperties();
    }



    private void DrawColorSelector(GridSystemData gridSystemData)
    {
        if (gridSystemData.colorController != null)
        {
            List<ColorOption> availableColorOptions = gridSystemData.colorController.colorOptions;
            string[] colorNames = availableColorOptions.Select(option => option.colorType.ToString()).ToArray();

            EditorGUI.BeginDisabledGroup(selectedElement == GridElement.Obstacle1x1);
            int selectedIndex = GetSelectedColorIndex(selectedElement);
            int newSelectedIndex = EditorGUILayout.Popup("Select Color", selectedIndex, colorNames);

            if (newSelectedIndex != selectedIndex)
            {
                SetSelectedColorIndex(selectedElement, newSelectedIndex);
            }

            if (selectedElement == GridElement.Car1x2 || selectedElement == GridElement.Car1x3 || selectedElement == GridElement.Obstacle1x2)
            {
                selectedDirection = (DirectionOption)EditorGUILayout.EnumPopup("Select Direction", selectedDirection);
            }

            EditorGUI.EndDisabledGroup();
        }
    }


    private Dictionary<GridElement, int> selectedColorIndices = new Dictionary<GridElement, int>();

    private int GetSelectedColorIndex(GridElement element)
    {
        if (selectedColorIndices.TryGetValue(element, out int index))
        {
            return index;
        }
        return 0; // Varsayýlan renk seçeneði
    }

    private void SetSelectedColorIndex(GridElement element, int index)
    {
        if (selectedColorIndices.ContainsKey(element))
        {
            selectedColorIndices[element] = index;
        }
        else
        {
            selectedColorIndices.Add(element, index);
        }
    }
    private Grids[] DrawGrid(Grids[] data)
    {
        if (gridSizeX.intValue <= 0 || gridSizeY.intValue <= 0)
        {
            // End the foldout group
            EditorGUILayout.EndFoldoutHeaderGroup();
            return data;
        }

        if (GUILayout.Button("Clear"))
        {
            // Show a confirmation dialog
            if (EditorUtility.DisplayDialog($"Clear", "Are you sure you want to clear this layer? This process can not be reverted", "Yes", "No"))
            {
                // Reset the layer data if the button is pressed                    
                data = GetLevelData(string.Empty, gridSizeX.intValue, gridSizeY.intValue);
            }
        }

        for (int y = gridSizeY.intValue-1; y >= 0 ; y--)
        {
            GUILayout.BeginHorizontal();

            for (int x = 0; x < gridSizeX.intValue; x++)
            {
                GUIStyle style = new GUIStyle(GUI.skin.button);
                Color bgColor = Color.white;

                Grids grid = data.FirstOrDefault(fd => fd.x == x && fd.y == y);

                Vector2Int cellPos = new Vector2Int(x, y);
                GridElement currentElement = (GridElement)grid.gridElementValue;

                if (IsOnGridEdge(cellPos)) // Kenar hücre mi kontrol ediyoruz
                {
                    bgColor = Color.gray; // Kenar hücreleri gri yapalým
                }
                else if (currentElement == GridElement.Obstacle1x1 || currentElement == GridElement.Obstacle1x2)
                {
                    bgColor = Color.black; // "Obstacle" için sabit renk olan siyahý kullanalým
                }
                else
                {
                    if(grid.colorControllerValue != -1)
                        bgColor = gridSystemData.colorController.colorOptions[grid.colorControllerValue].color;
                }

                Color textColor = Color.white;
                if (bgColor.r * 0.299 + bgColor.g * 0.587 + bgColor.b * 0.114 > 0.5)
                {
                    textColor = Color.black;
                }

                style.normal.textColor = textColor;
                style.normal.background = MakeTex(2, 2, bgColor);

                EditorGUI.BeginDisabledGroup(IsOnGridEdge(cellPos));
                if (GUILayout.Button(currentElement.ToString(), style, GUILayout.Width(40), GUILayout.Height(40)))
                {
                    Debug.Log("test");
                    Vector2Int selectedCellPos = new Vector2Int(x, y);
                    Vector2Int targetCellPos = selectedCellPos + GetTargetAddPoint();
                    Vector2Int targetCellPos2 = selectedCellPos + GetTargetAddPoint() + GetTargetAddPoint();
                    Grids grid2 = data.FirstOrDefault((fd => fd.x == targetCellPos.x && fd.y == targetCellPos.y));
                    Grids grid3 = data.FirstOrDefault((fd => fd.x == targetCellPos2.x && fd.y == targetCellPos2.y));
                    if (selectedElement == GridElement.Stickman || selectedElement == GridElement.Obstacle1x1)
                    {
                        if (IsInsideGrid(cellPos) && !IsOnGridEdge(cellPos))
                        {
                            if (grid.colorControllerValue == -1)
                            {
                                grid.colorControllerValue = GetSelectedColorIndex(selectedElement);
                                grid.directionValue = ((int)selectedDirection);
                                grid.gridElementValue = ((int)selectedElement);
                            }
                        }
                    }
                    else if (selectedElement == GridElement.Obstacle1x2 || selectedElement == GridElement.Car1x2)
                    {
                        if (IsInsideGrid(targetCellPos) && !IsOnGridEdge(targetCellPos))
                        {
                            if (grid.colorControllerValue == -1 && grid2.colorControllerValue == -1)
                            {
                                AddData(ref grid, grid2, null);
                                AddData(ref grid2,grid, null);
                                grid.colorControllerValue = GetSelectedColorIndex(selectedElement);
                                grid.directionValue = ((int)selectedDirection);
                                grid.gridElementValue = ((int)selectedElement);                               
                                grid2.colorControllerValue = GetSelectedColorIndex(selectedElement);
                            }
                        }

                    }
                    else if (selectedElement == GridElement.Car1x3)
                    {
                    
                        if ((IsInsideGrid(targetCellPos2) && !IsOnGridEdge(targetCellPos2)) && (IsInsideGrid(targetCellPos) && !IsOnGridEdge(targetCellPos)))
                        {
                            if (grid.colorControllerValue == -1 && grid2.colorControllerValue == -1
                                 && grid3.colorControllerValue == -1)
                            {
                                AddData(ref grid, grid2,grid3);
                                AddData(ref grid2, grid, grid3);
                                AddData(ref grid3, grid, grid3);
                                grid.colorControllerValue = GetSelectedColorIndex(selectedElement);
                                grid.directionValue = ((int)selectedDirection);
                                grid.gridElementValue = ((int)selectedElement);                      
                                grid2.colorControllerValue = GetSelectedColorIndex(selectedElement);
                                grid3.colorControllerValue = GetSelectedColorIndex(selectedElement);
                            }
                        }
                    }
                    else if(selectedElement == GridElement.None)
                    {
                        if (IsInsideGrid(cellPos) && !IsOnGridEdge(cellPos))
                        {
                            if (grid.colorControllerValue != -1)
                            {
                                DeleteData(data,grid);
                            }
                        }
                    }
                }
                EditorGUI.EndDisabledGroup();
            }
            GUILayout.EndHorizontal();            
        }
        grids.stringValue = JsonHelper.ToJson(data.ToArray());
        return data;
    }

    private void AddData(ref Grids selectGrid,Grids otherGrid1, Grids otherGrid2)
    {
        if(otherGrid1 != null)
        {
            selectGrid.otherOneX = otherGrid1.x;
            selectGrid.otherOneY = otherGrid1.y;
        }
        if(otherGrid2 != null)
        {
            selectGrid.otherTwoX = otherGrid2.x;
            selectGrid.otherTwoY = otherGrid2.y;
        }
    }

    private void DeleteData(Grids[] data,Grids selectGrid)
    {
        selectGrid.gridElementValue = 0;
        selectGrid.colorControllerValue = -1;
        if(selectGrid.otherOneX != 99)
        {
            Grids firstGrids = data.FirstOrDefault((fd) => fd.x == selectGrid.otherOneX && fd.y == selectGrid.otherOneY);
            if(firstGrids != null)
            {
                firstGrids.gridElementValue = 0;
                firstGrids.colorControllerValue = -1;
            }
        }
        if(selectGrid.otherTwoX != 99)
        {
            Grids secondGrids = data.FirstOrDefault((fd) => fd.x == selectGrid.otherTwoX && fd.y == selectGrid.otherTwoY);
            if (secondGrids != null)
            {
                secondGrids.gridElementValue = 0;
                secondGrids.colorControllerValue = -1;
            }
        }
    }

    private Vector2Int GetTargetAddPoint()
    {
        return selectedDirection == DirectionOption.Up ? new Vector2Int(0, 1) :
            selectedDirection == DirectionOption.Down ? new Vector2Int(0, -1)  :
            selectedDirection == DirectionOption.Right ? new Vector2Int(1, 0) : new Vector2Int(-1, 0)  ;
    }

    private bool IsInsideGrid(Vector2Int cellPos)
    {
        return cellPos.x >= 1 && cellPos.x < gridSizeX.intValue - 1 &&
               cellPos.y >= 1 && cellPos.y < gridSizeY.intValue - 1;
    }


    private bool IsOnGridEdge(Vector2Int cellPos)
    {
        return cellPos.x == 0 || cellPos.x == gridSizeX.intValue - 1 ||
               cellPos.y == 0 || cellPos.y == gridSizeY.intValue - 1;
    }
    private ColorOption GetCellColorOption(Vector2Int cellPos, GridSystemData gridSystemData)
    {
        if (cellColorOptions.TryGetValue(cellPos, out ColorOption option))
        {
            return option;
        }
        return GetSelectedColorOption(selectedElement,gridSystemData);
    }

    private void SetCellColorOption(Vector2Int cellPos, ColorOption colorOption)
    {
        cellColorOptions[cellPos] = colorOption;
    }

    private ColorOption GetSelectedColorOption(GridElement element,GridSystemData gridSystemData)
    {
        int selectedColorIndex = GetSelectedColorIndex(element);
        if(gridSystemData.colorController.colorOptions.Count > selectedColorIndex)
            return gridSystemData.colorController.colorOptions[selectedColorIndex];
        return new ColorOption();
    }

    private void SetCurrentColorOption(GridElement element, ColorOption colorOption)
    {
        Vector2Int cellPos = GetGridPosition();
        cellColorOptions[cellPos] = colorOption;
    }

    private Vector2Int GetGridPosition()
    {
        int x = (int)GUILayoutUtility.GetLastRect().xMin;
        int y = (int)GUILayoutUtility.GetLastRect().yMin;
        return new Vector2Int(x, y);
    }

    private Texture2D MakeTex(int width, int height, Color color)
    {
        Color[] pix = new Color[width * height];
        for (int i = 0; i < pix.Length; i++)
        {
            pix[i] = color;
        }
        Texture2D result = new Texture2D(width, height);
        result.SetPixels(pix);
        result.Apply();
        return result;
    }

    private Grids[] GetLevelData(string json, int sizeX, int sizeY)
    {
        var data = new List<Grids>();
        if (!string.IsNullOrEmpty(json))
        {
            data = JsonHelper.FromJson<Grids>(json).ToList();
        }

        if (data != null && sizeX * sizeY > 0 && data.Count == sizeX * sizeY)
        {
            return data.ToArray();
        }

        // Generate empty level data
        data.Clear();
        for (var x = 0; x < sizeX; x++)
        {
            for (var y = 0; y < sizeY; y++)
            {
                data.Add(new Grids
                {
                    x = x,
                    y = y,
                    directionValue = 0,
                    gridElementValue = 0,
                    colorControllerValue = -1,
                    otherOneX = 99,
                    otherOneY = 99,
                    otherTwoX = 99,
                    otherTwoY = 99

                }) ;
            }
        }

        return data.ToArray();
    }
}

[Serializable]
public class Grids
{
    //public DirectionOption direction;
    //public GridElement gridElement;
    //public ColorOption colorOption;
    //public Vector2Int position;

    [SerializeField] public int directionValue = -1;
    [SerializeField] public int gridElementValue = -1;
    [SerializeField] public int colorControllerValue = -1;
    [SerializeField] public int x;
    [SerializeField] public int y;
    [SerializeField] public int otherOneX;
    [SerializeField] public int otherOneY;
    [SerializeField] public int otherTwoX;
    [SerializeField] public int otherTwoY;
}
