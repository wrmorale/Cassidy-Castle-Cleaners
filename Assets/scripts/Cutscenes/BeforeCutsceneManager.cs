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

public class BeforeCutsceneManager : MonoBehaviour
{
    public GameObject continueButton;
    public static int cutsceneIndex;
    public bool roomCleared;
    public Texture2D[] cutsceneImages;
    public Image displayImage;
    public GameObject cutsceneCanvas;
    public GameObject dialogueBox;
    //public Image controlIcon;
    public InputDevice device;

    public float fadeDuration = 1.0f;
    private CanvasGroup canvasGroup;
    public EventSystem eventSystem;

    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI dialogueName;
    //private String[] dialogueTexts = new String[] { "What is this place? This feels different from the rest of the rooms... more... evil..", };
    private Dictionary <int , Tuple<string, string>> dialogueTexts = new Dictionary <int , Tuple<string, string>>() 
    {
        {
            0,Tuple.Create("The Maid","What is this place? This feels different from the rest of the rooms... more... evil..")
        }, 
        {
            1,Tuple.Create("The Mirror", "Well hello Ms. Castle Cleaner. Have you been enjoying my creations? I hope they haven't been too... messy.")
        }, 
        {
            2,Tuple.Create("", "THUNK")
        }, 
        {
            3,Tuple.Create("The Mirror", "Attacking an empty mirror? That doesn't seem maid like. You'll need to be quicker than that if you want to \"clean\" me.")
        }, 
        {
            4,Tuple.Create("The Maid", "I really need a vacation...")
        },
    };


    void Start()
    {
        displayImage = GameObject.Find("Cutscene").GetComponent<Image>();
        cutsceneIndex = 0;
        continueButton = GameObject.Find("ContinueButton");
        dialogueBox = GameObject.Find("DialogueBox");
        canvasGroup = GetComponent<CanvasGroup>();
        //controlIcon = GameObject.Find("Control").GetComponent<Image>();
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        dialogueName = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        DisplayNextCutscene();
        eventSystem = EventSystem.current;
       

    }

    public void DisplayNextCutscene() 
    {
        //Debug.Log("Cutscene index: " + cutsceneIndex);
        if (cutsceneIndex == 5){
            EndCutscene();
        }
        else {
            displayImage.sprite = Sprite.Create(cutsceneImages[cutsceneIndex], new Rect(0.0f, 0.0f, cutsceneImages[cutsceneIndex].width, cutsceneImages[cutsceneIndex].height), new Vector2(0.5f, 0.5f), 100.0f);
            string speaker = (dialogueTexts.ElementAt(cutsceneIndex).Value).Item1;
            string speakerText = (dialogueTexts.ElementAt(cutsceneIndex).Value).Item2;

            dialogueName.text = speaker;
            dialogueText.text = speakerText;
        }

        if (cutsceneIndex < 5){
            cutsceneIndex++;
        }
    }

    public void EndCutscene()
    {   
        continueButton.SetActive(false);
        dialogueBox.SetActive(false);
        SceneManager.LoadScene("NewBossScene");
    }
}
