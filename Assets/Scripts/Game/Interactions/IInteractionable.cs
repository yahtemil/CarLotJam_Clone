using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractionable
{
    GameObject GetObject();
    InteractableType GetInteractableType();
    IAnimation GetIAnimation();
    IMovable GetIMovable();

    Grid GetGrid();
    bool FirstClick();
    bool SecondClick();

    bool GetWalkable();
}

public enum InteractableType
{
    Null,
    Stickman,
    Car,
    Barrier,
    Grid
}

public enum ColorType
{
    Black,
    Purple,
    Orange,
    Blue,
    Yellow,
    Red,
    Green,
    Pink
}