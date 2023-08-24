using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleInteractable : MonoBehaviour, IObject
{
    public InteractableType interactableType = InteractableType.Barrier;

    public GameObject GetGameObject()
    {
        return gameObject;
    }
}
