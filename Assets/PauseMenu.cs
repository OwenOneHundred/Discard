using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    [SerializeField] GameObject _pauseButton;
   public void StartGame()
   {
    SceneManager.LoadScene(1);
   }
    
    // public void QuitGame();
    // {
    //     Application.Quit();
    // }

    public void Menu()
   {
    SceneManager.LoadScene(0);
   }

   public void PauseButton()
   {
    Time.timeScale = 0f;
    _pauseMenu.SetActive(false);
    _pausebutton.SetActive(false);
   }

   public void ResumeButton()
   {
    Time.timeScale = 1.0f;
    _pauseMenu.SetActive(false);
    _pausebutton.SetActive(true);
   }

}
