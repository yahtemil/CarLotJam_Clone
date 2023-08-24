using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionManagers : MonoBehaviour
{
    AStar _aStar;

    IInteractionable _saveInteractionable;
    List<Grid> path;

    public LayerMask layerMask;

    void Awake()
    {
        _aStar = GetComponent<AStar>();
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
                    {
                        Debug.Log("ilk tiklama");
                        FirstClickProcess(iInteractionable);
                    }
                    else
                    {
                        Debug.Log("ikinci tiklama");
                        SecondClickProcess(iInteractionable);
                    }
                    break;
                }              
            }
        }
    }

    public void FirstClickProcess(IInteractionable iInteractionable)
    {
        InteractableType interactableType = iInteractionable.GetInteractableType();

        if(interactableType == InteractableType.Stickman)
        {
            if(iInteractionable.GetWalkable() == false)
            {
                _saveInteractionable = iInteractionable;
                StickmanInteractable stickmanInteractable = iInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), true, false, false);
            }

        }
    }

    public void SecondClickProcess(IInteractionable iInteractionable)
    {
        if(_saveInteractionable == iInteractionable)
        {
            //AnimationPlay(_saveInteractionable.GetIAnimation(), false,false,false);
            StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
            AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, false, false);
            _saveInteractionable = null;
        }
        else
        {
            if(iInteractionable.GetInteractableType() == InteractableType.Stickman)
            {
                StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, false, false);
                //AnimationPlay(_saveInteractionable.GetIAnimation(), false,false,false);
                _saveInteractionable = iInteractionable;
                stickmanInteractable = iInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), true, false, false);
                //AnimationPlay(_saveInteractionable.GetIAnimation(), true,false,false);
            }
            else if (iInteractionable.GetInteractableType() == InteractableType.Car)
            {
                CarInteractable carInteractable = iInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<CarInteractable>();
                StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                if (AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorLeft) && stickmanInteractable.colorType == carInteractable.colorType)
                {
                    stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, true);
                    AnimationPlay(carInteractable.GetComponent<IAnimation>(), true, false, false);
                    carInteractable.rightDoorActive = false;
                    _saveInteractionable.GetIMovable().Move(path,true,carInteractable);
                    _saveInteractionable.GetGrid().triggerObject = null;
                    _saveInteractionable = null;
                }
                else if (AStarProcess(_saveInteractionable.GetObject().transform, carInteractable.offsetDoorRight) && stickmanInteractable.colorType == carInteractable.colorType)
                {
                    stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, true);
                    AnimationPlay(carInteractable.GetComponent<IAnimation>(), true, false, false);
                    carInteractable.rightDoorActive = true;
                    _saveInteractionable.GetIMovable().Move(path,true, carInteractable);
                    _saveInteractionable.GetGrid().triggerObject = null;
                    _saveInteractionable = null;
                }
                else if(_saveInteractionable.GetObject().transform.position == carInteractable.offsetDoorRight.position && stickmanInteractable.colorType == carInteractable.colorType)
                {
                    stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, true);
                    AnimationPlay(carInteractable.GetComponent<IAnimation>(), true, false, false);
                    carInteractable.rightDoorActive = true;
                    path = new List<Grid>();
                    path.Add(_saveInteractionable.GetGrid());
                    _saveInteractionable.GetIMovable().Move(path, true, carInteractable);
                    _saveInteractionable.GetGrid().triggerObject = null;
                    _saveInteractionable = null;
                }
                else if (_saveInteractionable.GetObject().transform.position == carInteractable.offsetDoorLeft.position && stickmanInteractable.colorType == carInteractable.colorType)
                {
                    stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, true);
                    AnimationPlay(carInteractable.GetComponent<IAnimation>(), true, false, false);
                    carInteractable.rightDoorActive = false;
                    path = new List<Grid>();
                    path.Add(_saveInteractionable.GetGrid());
                    _saveInteractionable.GetIMovable().Move(path, true, carInteractable);
                    _saveInteractionable.GetGrid().triggerObject = null;
                    _saveInteractionable = null;
                }
                else
                {
                    stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, false);
                    AnimationPlay(iInteractionable.GetIAnimation(), false, false, false);
                    _saveInteractionable = null;
                }
            }
            else if (iInteractionable.GetInteractableType() == InteractableType.Grid)
            {
                if (AStarProcess(_saveInteractionable.GetObject().transform, iInteractionable.GetObject().transform))
                {
                    AnimationPlay(iInteractionable.GetIAnimation(), true, false, false);
                    StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, false, true);
                    _saveInteractionable.GetIMovable().Move(path,false,new CarInteractable());
                    iInteractionable.GetGrid().triggerObject = _saveInteractionable.GetGrid().triggerObject;
                    _saveInteractionable.GetGrid().triggerObject = null;
                    _saveInteractionable = null;
                }
                else
                {
                    StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                    AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, false);
                    AnimationPlay(iInteractionable.GetIAnimation(), false, false, false);
                    _saveInteractionable = null;
                }
            }
            else if (iInteractionable.GetInteractableType() == InteractableType.Barrier)
            {
                StickmanInteractable stickmanInteractable = _saveInteractionable.GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                AnimationPlay(stickmanInteractable.GetComponent<IAnimation>(), false, true, false);
                _saveInteractionable = null;
                AnimationPlay(iInteractionable.GetIAnimation(), false, false, false);
            }
        }
    }

    public void AnimationPlay(IAnimation iAnimation,bool animationActive,bool emojiEffectActive,bool emojiPositiveEffect)
    {
        iAnimation.AnimationPlay(animationActive);
        if(emojiEffectActive)
            iAnimation.EmojiOpen(emojiPositiveEffect);
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
