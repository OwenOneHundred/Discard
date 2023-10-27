using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenu : MonoBehaviour
{
    [SerializeField] GameObject _pauseMenu;
    bool paused = false;
    
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
        paused = true;
    }

    public void ResumeButton()
    {
        Time.timeScale = 1.0f;
        _pauseMenu.SetActive(false);
        paused = false;
    }

    public void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (paused)
            {
                ResumeButton();
            }
            else
            {
                Pause();
            }
        }
    }

}
