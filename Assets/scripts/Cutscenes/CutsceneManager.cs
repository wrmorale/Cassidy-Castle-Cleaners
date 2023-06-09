using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class CutsceneManager : MonoBehaviour
{
    public GameObject continueButton;
    public static int cutsceneIndex;
    public bool roomCleared;
    public Texture2D[] cutsceneImages;
    public Image displayImage;
    public GameObject cutsceneCanvas;
    public GameObject dialogueBox;
    public GameObject CreditsButton;
    public Image controlIcon;
    //public Image enterIcon;
    //public GameObject portrait;
    public InputDevice device;

    public float fadeDuration = 1.0f;
    private CanvasGroup canvasGroup;
    public EventSystem eventSystem;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    private String[] dialogueTexts = new String[] {"Who would have thought being a maid would result in her battling an evil mirror?",
    "The maid finally leaves after a long day of cleaning.",
    "She hops into her truck, on her way to report back to Cassidy. She hopes Cassidy will give her a bonus...",
    "Unfortunately the folder at hand is not a bonus but another assignment, the grind never stops.",
    "The maid wonders what will she be put up against next?"};



    void Start()
    {
        displayImage = GameObject.Find("Cutscene").GetComponent<Image>();
        cutsceneIndex = 0;
        continueButton = GameObject.Find("ContinueButton");
        dialogueBox = GameObject.Find("DialogueBox");
        canvasGroup = GetComponent<CanvasGroup>();
        controlIcon = GameObject.Find("Control").GetComponent<Image>();
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        dialogueName = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        dialogueName.enabled = false;
        DisplayNextCutscene();
        eventSystem = EventSystem.current;
    }
    void Update(){
        //displayIcon();
    }

    public void DisplayNextCutscene() 
    {
        Debug.Log("Cutscene index: " + cutsceneIndex);
        if (cutsceneIndex == 5){
            EndCutscene();
        }
        else {
            displayImage.sprite = Sprite.Create(cutsceneImages[cutsceneIndex], new Rect(0.0f, 0.0f, cutsceneImages[cutsceneIndex].width, cutsceneImages[cutsceneIndex].height), new Vector2(0.5f, 0.5f), 100.0f);
            dialogueText.text = dialogueTexts[cutsceneIndex];
        }

        if (cutsceneIndex < 5){
            cutsceneIndex++;
        }
    }

    public void EndCutscene()
    {   
        //Debug.Log("End of cutscene.");
        cutsceneCanvas.SetActive(false);
        continueButton.SetActive(false);
        dialogueBox.SetActive(false);
        eventSystem.SetSelectedGameObject(CreditsButton);
        //SceneManager.LoadScene("Credits_Scene");
    }
    /*
    public void displayIcon(){
        if (device is Gamepad){
            controlIcon.enabled = true;
            enterIcon.enabled = false;
        }
        else {
            controlIcon.enabled = false;
            enterIcon.enabled = true;
        }
    }*/
}
