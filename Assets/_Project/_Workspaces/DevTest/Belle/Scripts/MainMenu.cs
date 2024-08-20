using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private AudioClip clip;
    public void PlayGame()
    {
        AudioController.Instance.FadeInAndOut(clip);
        SceneManager.LoadSceneAsync(1);
    }
}
