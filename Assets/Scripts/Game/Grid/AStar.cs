using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class AStar : MonoBehaviour
{
    public GameObject GridObject;
    public GameObject RoadDefault;
    public GameObject RoadOther;
    public GameObject RoadFinish;
    public GameObject RoadLine;

    public GameObject Obstacle1;
    public GameObject Obstacle2;
    public GameObject Car1;
    public GameObject Car2;
    public GameObject Stickman;

    public GameObject Camera1;
    public GameObject Camera2;

    [HideInInspector]
    public Transform startPoint;
    [HideInInspector]
    public Transform endPoint;
    public Grid[,] allGrid;

    public static AStar instance;

    public GridSystemData gridData;
    private void Awake()
    {
        instance = this;
        StartCoroutine(CreateTiming());
    }

    IEnumerator CreateTiming()
    {
        yield return new WaitForSeconds(0.1f);

        InitializeGrid();
    }
    void InitializeGrid()
    {
        var data = JsonHelper.FromJson<Grids>(gridData.grids);

        Debug.Log("initialize");
        int gridSizeX = gridData.gridSizeX;
        int gridSizeY = gridData.gridSizeY;



        allGrid = new Grid[gridData.gridSizeX, gridData.gridSizeY];
        if (gridSizeY - 2 >= 6)
        {
            int maxValue = gridSizeY >= gridSizeX ? gridSizeY : gridSizeX;
            Camera2.transform.position = new Vector3(3.55f - ((8 - maxValue) * 0.45f), 14.37f, 6.15f);
            Camera2.GetComponent<Camera>().orthographicSize = 8.75f - ((8 - maxValue) * 1.25f);
            Camera1.SetActive(false);
            Camera2.SetActive(true);
        }
        else
        {
            Camera1.transform.position = new Vector3(4.42f - ((7 - gridSizeY) * 0.88f), 10.38f - ((7 - gridSizeY) * 1.75f), -2.54f + ((7 - gridSizeY) * 0.8f));
            Camera1.SetActive(true);
            Camera2.SetActive(false);
        }
        for (int r = 0; r < data.Length; r++)
        {
            Grids grid = data[r];
            GameObject obj;
            Grid _grid = new Grid();
            int x = grid.x;
            int y = grid.y;
            if (x == 0)
            {
                if (y == gridSizeY - 1)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = Vector3.zero;
                    obj = Instantiate(RoadFinish, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    obj = Instantiate(RoadLine, new Vector3(x, 0f, y), quaternion, transform);
                    for (int i = 1; i < 10; i++)
                    {
                        obj = Instantiate(RoadDefault, new Vector3(x, 0f, y + i), quaternion, transform);
                    }
                }
                else if (y == 0)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = new Vector3(0f, -90f, 0f);
                    obj = Instantiate(RoadOther, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
                else
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = Vector3.zero;
                    obj = Instantiate(RoadDefault, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
            }
            else if (x == gridSizeX - 1)
            {
                if (y == gridSizeY - 1)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = new Vector3(0f, 90f, 0f);
                    obj = Instantiate(RoadOther, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
                else if (y == 0)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = new Vector3(0f, 180f, 0f);
                    obj = Instantiate(RoadOther, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
                else
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = Vector3.zero;
                    obj = Instantiate(RoadDefault, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
            }
            else if (y == 0)
            {
                if (x != 0 || x != gridSizeX - 1)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = new Vector3(0f, -90f, 0f);
                    obj = Instantiate(RoadDefault, new Vector3(grid.x, 0f, grid.y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
            }
            else if (y == gridSizeY - 1)
            {
                if (x != 0 || x != gridSizeX - 1)
                {
                    Quaternion quaternion = new Quaternion();
                    quaternion.eulerAngles = new Vector3(0f, -90f, 0f);
                    obj = Instantiate(RoadDefault, new Vector3(x, 0f, y), quaternion, transform);
                    _grid = obj.GetComponent<Grid>();
                    _grid.road = true;
                }
            }
            else
            {
                Quaternion quaternion = new Quaternion();
                quaternion.eulerAngles = Vector3.zero;
                obj = Instantiate(GridObject, new Vector3(x, 0f, y), quaternion, transform);
                _grid = obj.GetComponent<Grid>();
                if (grid.active == 1) 
                {
                    quaternion = new Quaternion();
                    quaternion.eulerAngles = grid.directionValue == 0 ? new Vector3(0f, 180f, 0f) : grid.directionValue == 1 ? new Vector3(0f, 0, 0f) : grid.directionValue == 2 ? new Vector3(0f, -90f, 0f) : new Vector3(0f, 90f, 0f);
                    if (grid.gridElementValue == 1)
                    {
                        obj = Instantiate(Obstacle1, new Vector3(x, 5f, y), quaternion, transform);
                    }
                    else if (grid.gridElementValue == 2)
                    {
                        obj = Instantiate(Obstacle2, new Vector3(x, 5f, y), quaternion, transform);
                    }
                    else if (grid.gridElementValue == 3)
                    {
                        obj = Instantiate(Car1, new Vector3(x, 5f, y), quaternion, transform);
                        CarInteractable carInteractable = obj.GetComponent<CarInteractable>();
                        carInteractable.SetColorOption((ColorType)grid.colorControllerValue);
                        if (grid.directionValue == 2 || grid.directionValue == 3)
                            obj.transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 1.5f);

                        if (gridData.level == 1)
                            carInteractable.SetTutorialLevel();
                    }
                    else if (grid.gridElementValue == 4)
                    {
                        obj = Instantiate(Car2, new Vector3(x, 5f, y), quaternion, transform);
                        CarInteractable carInteractable = obj.GetComponent<CarInteractable>();
                        carInteractable.SetColorOption((ColorType)grid.colorControllerValue);
                        if (grid.directionValue == 2 || grid.directionValue == 3)
                            obj.transform.GetChild(0).transform.localPosition = new Vector3(0f, 0f, 4.5f);

                        if (gridData.level == 1)
                            carInteractable.SetTutorialLevel();
                    }
                    else if (grid.gridElementValue == 5)
                    {
                        obj = Instantiate(Stickman, new Vector3(x, 5f, y), quaternion, transform);
                        StickmanInteractable stickmanInteractable = obj.GetComponent<StickmanInteractable>();
                        stickmanInteractable.SetColorOption((ColorType)grid.colorControllerValue);
                        if (gridData.level == 1)
                            stickmanInteractable.SetTutorialLevel();
                    }

                    if (grid.gridElementValue != 0)
                        StartCoroutine(AnimationTiming(obj));
                }


            }

            _grid.positionX = x;
            _grid.positionZ = y;

            allGrid[x, y] = _grid;
        }
    }

    IEnumerator AnimationTiming(GameObject g)
    {
        g.SetActive(false);
        yield return new WaitForSeconds(Random.Range(0.25f,0.75f));
        g.SetActive(true);
        g.transform.DOMoveY(0f, 0.5f).SetEase(Ease.OutBack);
    }

    public List<Grid> FindRoadPath()
    {
        Grid startNode = NodeFromWorldPoint(startPoint.position);
        Grid endNode = NodeFromWorldPoint(endPoint.position);

        List<Grid> openSet = new List<Grid>();
        HashSet<Grid> closedSet = new HashSet<Grid>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Grid currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
                return RetracePath(startNode, endNode);

            foreach (Grid neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.road || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        return null;
    }

    public List<Grid> FindPath()
    {
        Grid startNode = NodeFromWorldPoint(startPoint.position);
        Grid endNode = NodeFromWorldPoint(endPoint.position);

        if (startNode == endNode)
            return RetracePath(startNode, endNode);

        List<Grid> openSet = new List<Grid>();
        HashSet<Grid> closedSet = new HashSet<Grid>();
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            Grid currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if (openSet[i].fCost < currentNode.fCost || (openSet[i].fCost == currentNode.fCost && openSet[i].hCost < currentNode.hCost))
                    currentNode = openSet[i];
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode);

            if (currentNode == endNode)
                return RetracePath(startNode, endNode);

            foreach (Grid neighbor in GetNeighbors(currentNode))
            {
                if (!neighbor.walkable || closedSet.Contains(neighbor))
                    continue;

                int newCostToNeighbor = currentNode.gCost + GetDistance(currentNode, neighbor);
                if (newCostToNeighbor < neighbor.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, endNode);
                    neighbor.parent = currentNode;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }
        return null;
    }

    List<Grid> RetracePath(Grid startNode, Grid endNode)
    {
        List<Grid> path = new List<Grid>();
        Grid currentNode = endNode;

        if(startNode == endNode)
        {
            path.Add(startNode);
            return path;
        }

        while (currentNode != startNode)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        path.Reverse();

        return path;
    }

    public Grid NodeFromWorldPoint(Vector3 worldPosition)
    {
        return allGrid[Mathf.RoundToInt(worldPosition.x), (Mathf.RoundToInt(worldPosition.z))];
    }

    List<Grid> GetNeighbors(Grid node)
    {
        List<Grid> neighbors = new List<Grid>();

        int[] xOffset = { 1, 0, -1, 0 };
        int[] yOffset = { 0, 1, 0, -1 };

        for (int i = 0; i < xOffset.Length; i++)
        {
            int checkX = node.positionX + xOffset[i];
            int checkY = node.positionZ + yOffset[i];

            if (checkX >= 0 && checkX < allGrid.GetLength(0) && checkY >= 0 && checkY < allGrid.GetLength(1))
                neighbors.Add(allGrid[checkX, checkY]);
        }

        return neighbors;
    }

    int GetDistance(Grid nodeA, Grid nodeB)
    {
        int dstX = Mathf.Abs(nodeA.positionX - nodeB.positionX);
        int dstY = Mathf.Abs(nodeA.positionZ - nodeB.positionZ);
        return dstX + dstY;
    }
}
