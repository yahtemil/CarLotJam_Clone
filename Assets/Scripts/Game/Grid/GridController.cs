using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridController : MonoBehaviour
{
    CarInteractable carInteractable;
    private void Awake()
    {
        carInteractable = GetComponentInParent<CarInteractable>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Grid"))
        {
            if(other.TryGetComponent(out Grid grid))
            {
                grid.triggerObject = GetComponentInParent<IObject>();
                if(carInteractable != null)
                {
                    carInteractable.triggerGrids.Add(grid);
                }
                Invoke("CloseObject", 1f);
            }
        }
    }

    private void CloseObject()
    {
        gameObject.SetActive(false);
    }
}
