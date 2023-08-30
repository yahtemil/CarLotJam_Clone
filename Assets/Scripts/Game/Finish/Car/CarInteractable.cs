using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CarInteractable : MonoBehaviour, IAnimation, IMovable, IObject
{
    public Move move;
    public bool moving;
    public Transform LeftDoor;
    public Transform RightDoor;
    public Transform LeftDoorEnterPoint;
    public Transform RightDoorEnterPoint;
    public Transform LeftDoorLookPoint;
    public Transform RightDoorLookPoint;
    public Transform offsetDoorLeft;
    public Transform offsetDoorRight;
    public Transform mainTransform;
    public InteractableType interactableType = InteractableType.Car;
    public ColorType colorType = ColorType.Blue;
    public ColorController colorController;
    public MeshRenderer meshRenderer;
    public CarMoveController carMoveControllerForward;
    public CarMoveController carMoveControllerBack;
    public List<Grid> triggerGrids = new List<Grid>();

    List<Grid> _path;
    Transform _firstRoadTransform;
    public ParticleSystem SmogEffect;
    bool activeMove;
    bool forward = true;
    public bool rightDoorActive;
    public List<MeshRenderer> meshRenderers;
    public bool tutorialLevel;
    bool tutorialFinish;

    private void Start()
    {
        Invoke("FinishCarCounterPlus", 0.15f);
    }

    public void SetColorOption(ColorType _colorType)
    {
        colorType = _colorType;
        ColorOption colorOption = colorController.colorOptions.Find((x) => x.colorType == _colorType);
        foreach (MeshRenderer item in meshRenderers)
        {
            item.material = colorOption.materialNormal;
        }
        meshRenderer.material = colorOption.materialOutline;
    }

    public void SetTutorialLevel()
    {
        tutorialLevel = true;
    }

    private void FinishCarCounterPlus()
    {
        FinishLiner.instance.carCounter++;
    }

    public void AnimationPlay(bool activeAnimation)
    {
        if (tutorialLevel && !tutorialFinish)
        {
            tutorialFinish = true;
            TutorialLevelController.instance.ClosePanels();
        }
        else if (tutorialLevel && tutorialFinish)
        {
            TutorialLevelController.instance.ClosePanels();
        }
        float firstValue = activeAnimation ? 0f : 8f;
        float endValue = activeAnimation ? 8f : 0f;
        DOVirtual.Float(firstValue, endValue, 0.25f, (x) =>
        {
            meshRenderer.material.SetFloat("_Outline", x);
        }).OnComplete(() => 
        {
            DOVirtual.Float(endValue, firstValue, 0.25f, (x) =>
            {
                meshRenderer.material.SetFloat("_Outline", x);
            });
        });
    }

    public void EmojiOpen(bool positive)
    {

    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Move(List<Grid> path,bool enterCar, CarInteractable carInteractable)
    {
        Debug.Log("araba islemleri basladi");

        SmogEffect.Play();
        _path = new List<Grid>();
        _path = path;
        moving = true;
        if(CheckFirstPosition())
            MoveProcess(forward);
    }

    public void SetMoveReady(bool moveReady, Transform firstRoadTransform,bool _forward)
    {
        if (moveReady)
        {
            _firstRoadTransform = firstRoadTransform;
            if (moving && !activeMove)
            {
                MoveProcess(_forward);
            }
        }
    }

    private bool CheckFirstPosition()
    {
        if (carMoveControllerForward.moveReady && carMoveControllerBack.moveReady)
        {
            Debug.Log("forward ve back acik");
            float distance1 = Vector3.Distance(transform.position, carMoveControllerForward.firstRoadTransform.position);
            float distance2 = Vector3.Distance(transform.position, carMoveControllerBack.firstRoadTransform.position);

            if (distance1 <= distance2)
            {
                Debug.Log("forward acik");
                forward = true;
                _firstRoadTransform = carMoveControllerForward.firstRoadTransform;
                return true;
            }
            else
            {
                Debug.Log("back acik");
                forward = false;
                _firstRoadTransform = carMoveControllerBack.firstRoadTransform;
                return true;
            }
        }
        else if (carMoveControllerForward.moveReady)
        {
            Debug.Log("forward acik");
            forward = true;
            _firstRoadTransform = carMoveControllerForward.firstRoadTransform;
            return true;
        }
        else if (carMoveControllerBack.moveReady)
        {
            Debug.Log("back acik");
            forward = false;
            _firstRoadTransform = carMoveControllerBack.firstRoadTransform;
            return true;
        }
        else
        {
            Debug.Log("bos olarak cikti");
            return false;
        }
    }

    public void MoveProcess(bool forward)
    {
        StartCoroutine(MoveTiming(forward));
    }

    IEnumerator MoveTiming(bool forward)
    {
        activeMove = true;
        foreach (var item in triggerGrids)
        {
            item.triggerObject = null;
        }
        meshRenderer.gameObject.transform.DOLocalRotate(new Vector3(forward ? -10f : 10f, 0f, 0f), 0.5f).
            OnComplete(() => meshRenderer.gameObject.transform.DOLocalRotate(Vector3.zero, 0.5f));
        SmogEffect.Play();
        //yield return new WaitForSeconds(1f);
        float firstWaitTime = move.moveSpeed * Vector3.Distance(transform.position, _firstRoadTransform.position);
        transform.DOMove(_firstRoadTransform.position, firstWaitTime);
        AStar.instance.startPoint = _firstRoadTransform;
        AStar.instance.endPoint = FinishLiner.instance.gameObject.transform;
        _path = new List<Grid>();
        _path = AStar.instance.FindRoadPath();
        yield return new WaitForSeconds(firstWaitTime);
        foreach (Grid item in _path)
        {
            transform.DOLookAt(item.transform.position,move.rotateSpeed);
            transform.DOMove(item.transform.position, move.moveSpeed).SetEase(Ease.Linear);
            yield return new WaitForSeconds(move.moveSpeed);
        }
        FinishLiner.instance.carCounter--;
        FinishLiner.instance.CheckCarCounter();
        FinishLiner.instance.BarrierRaise();
        transform.DORotate(Vector3.zero, move.rotateSpeed);
        transform.DOMove(transform.position + Vector3.forward * 10, move.moveSpeed * 10).SetEase(Ease.Linear);
        
    }
}
