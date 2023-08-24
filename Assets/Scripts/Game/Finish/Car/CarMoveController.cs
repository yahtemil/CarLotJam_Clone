using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarMoveController : MonoBehaviour
{
    public List<GameObject> objectList;
    public bool moveReady;
    public Transform firstRoadTransform;
    CarInteractable carInteractable;
    public bool forward;

    private void Awake()
    {
        objectList = new List<GameObject>();
        carInteractable = GetComponentInParent<CarInteractable>();
        moveReady = true;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out IObject otherObject) || other.CompareTag("Car") || other.CompareTag("Stickman") || other.CompareTag("Obstacle"))
        {
            if (!objectList.Contains(other.gameObject))
            {
                objectList.Add(other.gameObject);
                moveReady = false;
                carInteractable.SetMoveReady(false,firstRoadTransform,forward);
            }                
        }
        else
        {
            if (other.CompareTag("Road"))
            {
                firstRoadTransform = other.transform;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out IObject otherObject) || other.CompareTag("Car") || other.CompareTag("Stickman") || other.CompareTag("Obstacle"))
        {
            if (objectList.Contains(other.gameObject))
            {
                objectList.Remove(other.gameObject);
                if(objectList.Count == 0)
                {
                    Debug.Log("check");
                    moveReady = true;
                    carInteractable.SetMoveReady(true, firstRoadTransform,forward);
                }
            }                
        }
    }    
}
