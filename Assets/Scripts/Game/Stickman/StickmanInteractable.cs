using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class StickmanInteractable : MonoBehaviour, IMovable, IAnimation, IObject
{
    public Move move;
    public bool moving;
    public ParticleSystem emojiPositiveEffect;
    public ParticleSystem emojiNegativeEffect;
    public InteractableType interactableType = InteractableType.Stickman;
    public ColorType colorType = ColorType.Blue;
    public ColorController colorController;
    public SkinnedMeshRenderer skinnedMeshRenderer;

    List<Grid> _path = new List<Grid>();
    public Animator animator;
    public bool tutorialLevel;
    int tutorialFinishCounter;

    private void Awake()
    {
        skinnedMeshRenderer.material.SetFloat("_Outline", 0f);
    }

    public void SetColorOption(ColorType _colorType)
    {
        colorType = _colorType;
        ColorOption colorOption = colorController.colorOptions.Find((x) => x.colorType == _colorType);
        skinnedMeshRenderer.material = colorOption.materialOutline;
        skinnedMeshRenderer.material.color = colorOption.color;
        skinnedMeshRenderer.material.SetFloat("_Outline", 0f);
    }

    public void SetTutorialLevel()
    {
        tutorialLevel = true;
        if(colorType == ColorType.Red)
        {
            if (tutorialLevel && tutorialFinishCounter == 0)
            {
                tutorialFinishCounter++;
                TutorialLevelController.instance.stickmanPosition = transform;
            }
            skinnedMeshRenderer.material.SetFloat("_Outline", 10f);
        }
    }

    public void AnimationPlay(bool activeAnimation)
    {
        if (tutorialLevel && tutorialFinishCounter == 0 && colorType == ColorType.Red)
        {
            tutorialFinishCounter++;
            TutorialLevelController.instance.stickmanPosition = transform;
        }
        else if (tutorialLevel && tutorialFinishCounter == 1 && colorType == ColorType.Red)
        {
            tutorialFinishCounter++;
            TutorialLevelController.instance.WherePanelOpen();
        }
        else if (tutorialLevel && tutorialFinishCounter >= 2 && colorType == ColorType.Red)
        {
            TutorialLevelController.instance.ClosePanels();
        }
        float firstValue = activeAnimation ? 0f : 10f;
        float endValue = activeAnimation ? 10f : 0f;
        DOVirtual.Float(firstValue, endValue, 0.25f, (x) =>
        {
            skinnedMeshRenderer.material.SetFloat("_Outline", x);
        });
    }

    public void EmojiOpen(bool positive)
    {
        if (positive)
        {
            animator.Play("Running");
            emojiPositiveEffect.Play();
        }
        else
            emojiNegativeEffect.Play();
    }

    public GameObject GetGameObject()
    {
        return gameObject;
    }

    public void Move(List<Grid> path,bool enterCar, CarInteractable carInteractable)
    {
        moving = true;
        _path = new List<Grid>();
        _path = path;

        MoveProcess(enterCar, carInteractable);
    }

    public void MoveProcess(bool enterCar, CarInteractable carInteractable)
    {
        StartCoroutine(MoveTiming(enterCar, carInteractable));
    }

    IEnumerator MoveTiming(bool enterCar, CarInteractable carInteractable)
    {
        foreach (Grid item in _path)
        {
            transform.DOLookAt(item.transform.position, move.rotateSpeed);
            transform.DOMove(item.transform.position, move.moveSpeed).SetEase(Ease.Linear);
            yield return new WaitForSeconds(move.moveSpeed);
        }
        animator.Play("Idle");
        if (enterCar)
        {
            animator.Play("EnterCar");
            gameObject.transform.DOScale(Vector3.one * 0.2f, 1f).SetEase(Ease.Linear).OnComplete(() => 
            {
                gameObject.SetActive(false);
            });
            if (carInteractable.rightDoorActive)
            {
                gameObject.transform.DOMove(carInteractable.RightDoorEnterPoint.position, 1f);
                gameObject.transform.DOLookAt(carInteractable.RightDoorLookPoint.position , 0.1f);
                carInteractable.RightDoor.DOLocalRotate(new Vector3(0f, 70f, 0f), 0.2f);
                carInteractable.RightDoor.DOLocalRotate(Vector3.zero, 0.2f).SetDelay(0.5f);
            }
            else
            {
                gameObject.transform.DOMove(carInteractable.LeftDoorEnterPoint.position, 1f);
                gameObject.transform.DOLookAt(carInteractable.LeftDoorLookPoint.position, 0.1f);
                carInteractable.LeftDoor.DOLocalRotate(new Vector3(0f, 70f, 0f), 0.2f);
                carInteractable.LeftDoor.DOLocalRotate(Vector3.zero, 0.2f).SetDelay(0.5f);
            }
            carInteractable.meshRenderer.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, -1.7f), 0.25f).OnComplete(() => 
            {
                carInteractable.meshRenderer.gameObject.gameObject.transform.DOLocalRotate(new Vector3(0f, 0f, 2f), 0.25f).OnComplete(() =>
                   {
                       carInteractable.meshRenderer.gameObject.gameObject.transform.DOLocalRotate(Vector3.zero, 0.25f).OnComplete(() => 
                       {
                           carInteractable.Move(new List<Grid>(),false,null);
                       });
                   });
            }).SetDelay(0.5f);
        }
        yield return new WaitForSeconds(0.2f);
        moving = false;

    }
}
