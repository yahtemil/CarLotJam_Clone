using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManagers : MonoBehaviour
{
    public static InteractionManagers Instance;
    public IInteractionable _saveInteractionable;
    List<Grid> path;

    public LayerMask layerMask;

    void Awake()
    {
        Instance = this;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit[] hits = Physics.RaycastAll(ray,layerMask);
            foreach (var item in hits)
            {
                GameObject hitObject = item.collider.gameObject;

                if (hitObject.TryGetComponent(out IInteractionable iInteractionable))
                {
                    if (_saveInteractionable == null)
                        iInteractionable.FirstClick();
                    else
                        iInteractionable.SecondClick();

                    break;
                }              
            }
        }
    }

    public void SetGridTrigger(Grid targetGrid)
    {
        targetGrid.triggerObject = _saveInteractionable.GetGrid().triggerObject;
    }

    public void StickmanMove(CarInteractable carInteractable = null)
    {
        AnimationPlay(GetAnimationComponent(_saveInteractionable), false, false, false);
        _saveInteractionable.GetIMovable().Move(path, carInteractable != null, carInteractable);
        _saveInteractionable.GetGrid().triggerObject = null;
        _saveInteractionable = null;
    }

    public void StickmanFailMove()
    {
        AnimationPlay(GetAnimationComponent(_saveInteractionable), false, true, false);
        _saveInteractionable = null;
    }

    public void StickmanAnimationClose()
    {
        AnimationPlay(GetAnimationComponent(_saveInteractionable), false, false, false);
        _saveInteractionable = null;
    }

    IAnimation GetAnimationComponent(IInteractionable iInteractionable)
    {
        GameObject triggerObject = iInteractionable.GetGrid().triggerObject.GetGameObject();

        if (triggerObject.TryGetComponent(out StickmanInteractable stickmanInteractable))
        {
            return stickmanInteractable;
        }
        if (triggerObject.TryGetComponent(out CarInteractable carInteractable))
        {
            return carInteractable;
        }

        return null;
    }

    public void AnimationPlay(IAnimation iAnimation,bool animationActive,bool emojiEffectActive,bool emojiPositiveEffect)
    {
        iAnimation.AnimationPlay(animationActive);
        if(emojiEffectActive)
            iAnimation.EmojiOpen(emojiPositiveEffect);
    }

    public bool CheckPath(CarInteractable carInteractable)
    {
        StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
        if (stickmanInteractable.colorType == carInteractable.colorType)
        {
            int counter1 = 99;
            int counter2 = 99;
            if (AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorLeft))
            {
                counter1 = path.Count;
            }
            if (AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorRight))
            {
                counter2 = path.Count;
            }

            if (counter1 == 99 && counter2 == 99)
                return false;
            else
            {
                if (counter1 < counter2)
                {
                    AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorLeft);
                    carInteractable.rightDoorActive = false;
                }
                else
                {
                    AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorRight);
                    carInteractable.rightDoorActive = true;
                }                

                return true;
            }
        }
        return false;
    }

    public bool CheckPath(Transform targetTransform)
    {
        return AStarProcess(_saveInteractionable.GetObject().transform, targetTransform);
    }

    public bool AStarProcess(Transform startPoint,Transform endPoint)
    {
        path = new List<Grid>();

        AStar.instance.startPoint = startPoint;
        AStar.instance.endPoint = endPoint;

        path = AStar.instance.FindPath();
        if (path == null)
            return false;

        return path.Count != 0;
    }

}
