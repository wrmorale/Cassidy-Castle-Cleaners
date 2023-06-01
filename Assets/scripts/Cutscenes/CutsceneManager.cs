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
    public bool roomCleared;
    public Image[] cutsceneImages;
    public Image displayImage;

    void Start()
    {
        displayImage = GetComponent<Image>();
    }

    void Update()
    {
        if (roomCleared){
            StartCutscene();
        }
    }

    public void StartCutscene ()
    {
        cutsceneIndex = 0;
        
        DisplayNextCutscene();
    }

    public void DisplayNextCutscene() 
    {
        // set cutscene image 
        displayImage = cutsceneImages[cutsceneIndex];

        if(cutsceneIndex == 5){
            EndCutscene();
        }
        
        cutsceneIndex++;
    }

    void EndCutscene ()
    {   
        Debug.Log("End of cutscene.");
        cutsceneIndex = 0;
    }
}
