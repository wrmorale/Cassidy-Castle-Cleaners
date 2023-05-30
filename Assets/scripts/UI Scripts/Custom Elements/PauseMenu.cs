using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class PauseMenu : MonoBehaviour
{
    public bool isPaused = false;
    public PlayerInput playerInput;
    public InputAction pauseAction;
    public GameObject PauseMenuCanvas;
    public EventSystem eventSystem;
    public GameObject resumeButton;
    public GameObject controlsButton;
    public GameObject controlButton;
    public GameObject backButton;



    void Start()
    {
        Time.timeScale = 1.0f;
        pauseAction = playerInput.actions["Pause"];
        eventSystem = EventSystem.current;
    }

    // Update is called once per frame
    void Update()
    {

        if(pauseAction.triggered)
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
        
        //Debug.Log(eventSystem.currentSelectedGameObject);
    }

    public void ResumeGame()
    {
        PauseMenuCanvas.SetActive(false);
        Time.timeScale = 1.0f;
        isPaused = false;
    }

    public void PauseGame()
    {
        PauseMenuCanvas.SetActive(true);
        Time.timeScale = 0.0f;
        isPaused = true;
    }

    public void MainMenuButton()
    {
        SceneManager.LoadScene("Title_Scene");
    }

    public void ExitGame()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void switchSelected()
    {   
        if(eventSystem.currentSelectedGameObject == controlsButton){
            eventSystem.SetSelectedGameObject(controlButton);
        }else if(eventSystem.currentSelectedGameObject == backButton){
            eventSystem.SetSelectedGameObject(resumeButton);
        }
    }
}
