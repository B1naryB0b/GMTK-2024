using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class MainMenu : MonoBehaviour
{
    [SerializeField] private Animator CreditButton;
    [SerializeField] private Animator PlayButton;
    [SerializeField] private Animator Title;
    [SerializeField] private Animator Scroll;
    public void PlayGame()
    {
        SceneManager.LoadSceneAsync(1);
    }

    public void Credits()
    {
        //fade out ui
        CreditButton.SetTrigger("Enter");
        PlayButton.SetTrigger("Enter");
        Title.SetTrigger("Enter");
        Invoke("ScrollCredits", 1.0f);
        //credit scroll from top.
        //fade in ui
    }
    private void ScrollCredits()
    {
        Scroll.SetTrigger("Enter");
        Invoke("RestoreButtons", 10.0f);
    }

    private void RestoreButtons()
    {
        Title.SetTrigger("BringBack");
        PlayButton.SetTrigger("BringBack");
        CreditButton.SetTrigger("BringBack");
    }
}
