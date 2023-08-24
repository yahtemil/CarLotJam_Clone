using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class TutorialLevelController : MonoBehaviour
{
    public GameObject handIcon;

    public GameObject TutorialPanel;
    public GameObject handPanel;
    public GameObject wherePanel;

    public Transform stickmanPosition;

    public static TutorialLevelController instance;

    private void Awake()
    {
        instance = this;
        if (PlayerPrefs.GetInt("level", 1) == 1)
            StartCoroutine(StartTiming());
        else
            gameObject.SetActive(false);
    }

    IEnumerator StartTiming()
    {
        yield return new WaitForSeconds(1f);
        TutorialPanel.SetActive(true);
        handIcon.SetActive(true);
        handIcon.transform.position = Camera.main.WorldToScreenPoint(stickmanPosition.position);
        //handIcon.transform.GetComponent<RectTransform>().anchoredPosition = stickmanPosition.position;
        handPanel.SetActive(true);
    }

    public void WherePanelOpen()
    {
        handPanel.transform.localScale = Vector3.one;
        handPanel.transform.DOScale(Vector3.zero,0.25f).SetEase(Ease.InBack).OnComplete(() => 
        {
            wherePanel.transform.localScale = Vector3.zero;
            wherePanel.SetActive(true);
            wherePanel.transform.DOScale(Vector3.one, 0.25f).SetEase(Ease.OutBack);
        });
    }

    public void ClosePanels()
    {
        handPanel.transform.DOScale(Vector3.zero, 0.25f);
        wherePanel.transform.DOScale(Vector3.zero, 0.25f);
    }
}
