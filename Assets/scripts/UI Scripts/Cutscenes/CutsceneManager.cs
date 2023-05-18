using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class CutsceneManager : MonoBehaviour
{
    public GameObject continueButton;
    public static int cutsceneIndex;
    public GameManager roomCleared;
    // create a var to see when they clear the room before
    // gameobj var for the cutscene image

    // Start is called before the first frame update
    void Start()
    {
        continueButton = GameObject.Find("ContinueButton");
        continueButton.SetActive(false);
        StartCutscene();
    }

    public void StartCutscene ()
    {
        cutsceneIndex = 0;
        
        DisplayNextCutscene();
        continueButton.SetActive(true);
        // set cutscene image to active
    }

    public void DisplayNextCutscene() 
    {
        // set cutscene image 
        if(cutsceneIndex == 5){
            EndCutscene();
        }
        
        cutsceneIndex++;
    }

    void EndCutscene ()
    {   
        continueButton.SetActive(false);
        Debug.Log("End of cutscene.");
        cutsceneIndex = 0;
    }
}
