using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;

public class MainMenu : MonoBehaviour
{

    public EventSystem eventSystem;
    public GameObject StartButton;
    public GameObject StartTutorial;
    public GameObject BackButton;
    PersistentGameManager persistentGameManager;
    

    void Start()
    {
        eventSystem = EventSystem.current;
        persistentGameManager = FindObjectOfType<PersistentGameManager>();
        if (persistentGameManager) //Make sure player starts with the correct stats
        {
            persistentGameManager.resetPlayerStats();
        }
    }

    public void ToTutorial()
    {
        SceneManager.LoadScene("TutorialScene");
    }

    public void ToMainGame()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void ToCredits()
    {
        SceneManager.LoadScene("Credits_Scene");
    }

    public void ToControls()
    {
        SceneManager.LoadScene("Controls_Scene");
    }

    public void ToExit()
    {
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #endif

        Application.Quit();
    }

    public void switchSelected()
    {   
        if(eventSystem.currentSelectedGameObject == StartButton){
            eventSystem.SetSelectedGameObject(StartTutorial);
        }else if(eventSystem.currentSelectedGameObject == BackButton){
            eventSystem.SetSelectedGameObject(StartButton);
        }
    }
}
