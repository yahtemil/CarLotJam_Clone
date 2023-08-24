using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueButton : MonoBehaviour
{
    public void ContinueProcess()
    {
        int level = PlayerPrefs.GetInt("level", 1);
        int sceneCount = SceneManager.sceneCountInBuildSettings;
        if(level >= sceneCount)
        {
            level = level % sceneCount == 0 ? sceneCount - 1 : sceneCount;
        }
        SceneManager.LoadScene(level);
    }
}
