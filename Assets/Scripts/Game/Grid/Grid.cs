using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Grid : MonoBehaviour
{
    public int positionX;
    public int positionZ;
    public Grid parent;
    public int gCost;
    public int hCost;
    public int fCost;
    public IObject triggerObject;
    public bool walkable => triggerObject == null && road == false ? true : false;

    public bool road;

    public InteractableType GetInteractable()
    {
        if (triggerObject != null)
        {
            if (triggerObject.GetGameObject().TryGetComponent(out StickmanInteractable stickman))
            {
                return InteractableType.Stickman;
            }
            else if (triggerObject.GetGameObject().TryGetComponent(out CarInteractable car))
            {
                return InteractableType.Car;
            }
            else if (triggerObject.GetGameObject().TryGetComponent(out ObstacleInteractable obstacle))
            {
                return InteractableType.Barrier;
            }
            return InteractableType.Grid;
        }
        else
            return InteractableType.Grid;
    }
}
