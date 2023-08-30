using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GridInteractable : MonoBehaviour, IInteractionable, IAnimation
{
    Grid _grid;
    public MeshRenderer meshRenderer;
    Color _firstColor;

    private void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        _firstColor = meshRenderer.materials[0].color;
        _grid = GetComponent<Grid>();
    }

    public bool FirstClick()
    {
        InteractableType interactableType = _grid.GetInteractable();

        if (interactableType == InteractableType.Stickman)
        {
            StickmanInteractable stickmanInteractable = GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
            InteractionManagers.Instance._saveInteractionable = this;
            return stickmanInteractable.FirstClick();
        }

        return false;
    }

    public bool SecondClick()
    {
        InteractableType interactableType = _grid.GetInteractable();

        if (interactableType == InteractableType.Stickman)
        {
            InteractionManagers.Instance.StickmanAnimationClose();
            StickmanInteractable stickmanInteractable = GetGrid().triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
            InteractionManagers.Instance._saveInteractionable = this;
            return stickmanInteractable.FirstClick();
        }
        else if(interactableType == InteractableType.Car)
        {
            CarInteractable carInteractable = GetGrid().triggerObject.GetGameObject().GetComponent<CarInteractable>();
            if (InteractionManagers.Instance.CheckPath(carInteractable))
            {
                AnimationPlay(true);
                InteractionManagers.Instance.StickmanMove(carInteractable);
                return true;
            }

            InteractionManagers.Instance.StickmanFailMove();
            AnimationPlay(false);
            return false;

        }
        else if(interactableType == InteractableType.Barrier)
        {
            InteractionManagers.Instance.StickmanFailMove();
            AnimationPlay(false);
            return false;
        }
        else if(interactableType == InteractableType.Grid)
        {
            if (InteractionManagers.Instance.CheckPath(transform))
            {
                AnimationPlay(true);
                InteractionManagers.Instance.SetGridTrigger(GetGrid());
                InteractionManagers.Instance.StickmanMove();
                return true;
            }
            else
            {
                InteractionManagers.Instance.StickmanFailMove();
                AnimationPlay(false);
                return false;
            }
        }

        return false;
    }

    public Grid GetGrid()
    {
        return _grid;
    }

    public InteractableType GetInteractableType()
    {
        return _grid.GetInteractable();
    }

    public void OnClickInteraction()
    {
        if(_grid.triggerObject != null)
        {

        }
    }

    public bool GetWalkable()
    {
        if(_grid.triggerObject != null)
        {
            InteractableType interactableType = _grid.GetInteractable();
            if(interactableType == InteractableType.Stickman)
            {
                StickmanInteractable stickman = _grid.triggerObject.GetGameObject().GetComponent<StickmanInteractable>();
                return stickman.moving;
            }
            else if(interactableType == InteractableType.Car)
            {
                CarInteractable carInteractable = _grid.triggerObject.GetGameObject().GetComponent<CarInteractable>();
                return carInteractable.moving;
            }
            else
            {
                return false;
            }
        }

        return false;
    }

    public GameObject GetObject()
    {
        return gameObject;
    }

    public IAnimation GetIAnimation()
    {
        return this;
    }

    public void AnimationPlay(bool activeAnimation)
    {
        meshRenderer.materials[0].DOColor(activeAnimation ? Color.green : Color.red,0.25f).SetEase(Ease.OutBack)
            .OnComplete(() => meshRenderer.materials[0].DOColor(_firstColor,0.25f).SetEase(Ease.InBack));
    }

    public void EmojiOpen(bool positive)
    {

    }

    public IMovable GetIMovable()
    {
        return _grid.triggerObject.GetGameObject().GetComponent<IMovable>();
    }


}
