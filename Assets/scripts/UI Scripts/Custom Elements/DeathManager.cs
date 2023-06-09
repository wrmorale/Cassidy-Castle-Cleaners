using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class DeathManager : MonoBehaviour
{   
    private Color color;
    public GameObject buttons;
    public GameObject continueButton;
    public GameObject gmUI;
    public GameObject playerUI;
    public GameObject pauseMenu;
    public GameObject abilityUI;
    public EventSystem eventSystem;
    public Image deathScreen;
    public bool fadeIN = false;
    // Start is called before the first frame update
    void Start()
    {
        eventSystem = EventSystem.current;
    }
    void Update()
    {
        if(fadeIN)
        {
            color = deathScreen.color;
            color.a += 0.01f;
            deathScreen.color = color;
            if(color.a >= 1)
            {
                showButtons();
                fadeIN = false;
            }
        }
    }

    public void die()
    {
        gmUI.SetActive(false);
        playerUI.SetActive(false);
        pauseMenu.SetActive(false);
        abilityUI.SetActive(false);
        fadeIN = true;
    }

    

    void showButtons()
    {
        buttons.SetActive(true);
        eventSystem.SetSelectedGameObject(continueButton);
    }

    public void toCheckPoint()
    {
        string activeScene = SceneManager.GetActiveScene().name;
        string checkPoint = activeScene;
        if(activeScene == "TutorialScene"){
            checkPoint = "TutorialScene";
        }else if(activeScene == "SampleScene" || activeScene == "room_2"){
            checkPoint = "SampleScene";
        }else if(activeScene == "room_3" || activeScene == "room_4"){
            checkPoint = "room_3";
        }else if(activeScene == "NewBossScene"){
            checkPoint = "NewBossScene";
        }
        
        SceneManager.LoadScene(checkPoint);
    }

    public void toTitleScene()
    {
        SceneManager.LoadScene("Title_Scene");
    }
}
