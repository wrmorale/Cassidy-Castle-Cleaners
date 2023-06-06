using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class CutsceneManager : MonoBehaviour
{
    public GameObject continueButton;
    public static int cutsceneIndex;
    public bool roomCleared;
    public Texture2D[] cutsceneImages;
    public Image displayImage;
    public GameObject cutsceneCanvas;
    public GameObject dialogueBox;
    //public GameObject portrait;

    public float fadeDuration = 1.0f;
    private CanvasGroup canvasGroup;

    

    void Start()
    {
        displayImage = GameObject.Find("Cutscene").GetComponent<Image>();
        cutsceneIndex = 0;
        continueButton = GameObject.Find("ContinueButton");
        dialogueBox = GameObject.Find("DialogueBox");
        canvasGroup = GetComponent<CanvasGroup>();
        DisplayNextCutscene();
    }

    public void DisplayNextCutscene() 
    {
        Debug.Log("Cutscene index: " + cutsceneIndex);
        if (cutsceneIndex == 5){
            EndCutscene();
        }
        else {
            displayImage.sprite = Sprite.Create(cutsceneImages[cutsceneIndex], new Rect(0.0f, 0.0f, cutsceneImages[cutsceneIndex].width, cutsceneImages[cutsceneIndex].height), new Vector2(0.5f, 0.5f), 100.0f);
        }
        cutsceneIndex++;
    }

    public void EndCutscene()
    {   
        //Debug.Log("End of cutscene.");
        cutsceneCanvas.SetActive(false);
        continueButton.SetActive(false);
        dialogueBox.SetActive(false);
        //SceneManager.LoadScene("Credits_Scene");
    }

    public void backButton(){
        SceneManager.LoadScene("MainMenu");
    }
}
