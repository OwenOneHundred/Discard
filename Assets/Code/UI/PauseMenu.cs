using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    
   public void StartGame()
   {
    SceneManager.LoadScene(1);
   }
    
    public void QuitGame()
    {
        Application.Quit();
    }

    

   public void Pause()
   {
    Time.timeScale = 0f;
    _pauseMenu.SetActive(true);
  
   }


   public void ResumeButton()
   {
    Time.timeScale = 1.0f;
    _pauseMenu.SetActive(false);

   }

   public void Update()
   {
    if (Input.GetKeyDown(KeyCode.Escape))
    {

     Debug.Log("esc was pressed");
    
    }
   }

}
