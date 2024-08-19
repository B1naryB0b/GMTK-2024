using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelTracker : MonoBehaviour
{
    public int LevelIndex = 0;

    public void GoTONextLevel()
    {

        SceneManager.LoadSceneAsync((LevelIndex + 3));
    }

    public void IncIndex()
    {
        LevelIndex++;
    }
}
