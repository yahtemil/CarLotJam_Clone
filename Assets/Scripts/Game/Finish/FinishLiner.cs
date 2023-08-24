using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class FinishLiner : MonoBehaviour
{
    public static FinishLiner instance;

    public int carCounter;
    public Transform barrierRaise;
    Sequence sequence;

    private void Awake()
    {
        instance = this;
        sequence = DOTween.Sequence();
    }

    public void CheckCarCounter()
    {
        if(carCounter <= 0)
        {
            UIManager.instance.CompletedGame();
            // Level completed;
        }
    }

    public void BarrierRaise()
    {
        sequence.Kill();
        sequence = DOTween.Sequence();
        sequence.Append(barrierRaise.DOLocalRotate(new Vector3(0f, 0f, 90f), 0.2f));
        sequence.AppendInterval(1f);
        sequence.Append(barrierRaise.DOLocalRotate(Vector3.zero, 0.2f));
        sequence.Play();

    }
}
