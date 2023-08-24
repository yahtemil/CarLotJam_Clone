using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public GameObject GamePanel;
    public GameObject CompletedPanel;

    public TextMeshProUGUI goldText;
    public TextMeshProUGUI levelText;
    public TextMeshProUGUI addGoldText;

    public static UIManager instance;

    private void Awake()
    {
        instance = this;
        goldText.text = PlayerPrefs.GetInt("gold", 0).ToString("F0");
        levelText.text = "lvl" + " " + PlayerPrefs.GetInt("level", 1).ToString("F0");
    }


    public void CompletedGame()
    {
        addGoldText.text = "20";
        goldText.text = (PlayerPrefs.GetInt("gold", 0) + 20).ToString("F0");
        PlayerPrefs.SetInt("level", PlayerPrefs.GetInt("level", 1) + 1);
        PlayerPrefs.SetInt("gold", PlayerPrefs.GetInt("gold", 0) + 20);
        CompletedPanel.transform.localScale = Vector3.zero;
        CompletedPanel.SetActive(true);
        CompletedPanel.transform.DOScale(Vector3.one, 0.5f).SetEase(Ease.OutBack);
    }

    public void RestartButton()
    {
        int activeSceneIndex = SceneManager.GetActiveScene().buildIndex;
        SceneManager.LoadScene(activeSceneIndex);
    }

    public void ContinueButton()
    {
        int level = PlayerPrefs.GetInt("level", 1);
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if (level >= sceneCount)
        {
            level = level % sceneCount;
        }
        SceneManager.LoadScene(level);
    }


}
