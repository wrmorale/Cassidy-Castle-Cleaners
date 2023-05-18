using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class DialogueManager : MonoBehaviour
{
    public TextMeshProUGUI nameText;
    public TextMeshProUGUI dialogueText;
    public GameObject dialoguebox;
    public GameObject continueButton;
    public GameManager gameManager;
    public GameObject controls;
    public TargetLock targetLock;
    [HideInInspector]public TutorialManager tutorialManager;
    public int dialogueIndex;
    //Bools and other things for states of the Tutorial
    [SerializeField]private string curState; 
    [SerializeField]private bool controlgiven = false;
    [SerializeField]private int abilitiesUsed = 0;
    [SerializeField]private bool targetLocked = false;
    [SerializeField]private int hitWhileLockedOn = 0;


    // dialogueDatabase has a key containing line index and current state, the value is current speaker and line

    // TODO: create list of images to display at different stages of dialouge  
    
    private Dictionary<Tuple<int, string>, Tuple<string, string>> dialogueDatabase = new Dictionary<Tuple<int,string>, Tuple<string, string>>()
    {
        {
            Tuple.Create(0, "cleanPile"), Tuple.Create("Cassidy", "Listen, new hire. I know you want to jump right into things, but we should go ahead and teach you the basics. First, clean these dust piles. Look around if you don't see them right in front of you.")
        },
        {
            Tuple.Create(1, "cleanPile"), Tuple.Create("The Maid", "I'm not that new to cleaning, you know ...")
        },
        {
            Tuple.Create(2, "cleanPile"), Tuple.Create("Cassidy","Cleaning up dust piles gives you mana. See that blue circle in the top-left?")
        },
         {
            Tuple.Create(3, "teachMana"), Tuple.Create("Cassidy","Mana is needed to use your special cleaning abilities. Each require a different amount of mana. Give them a test.")
        },
        {
            Tuple.Create(4, "teachMana"), Tuple.Create("Cassidy", "By the way, this is important: When you enter a room, the sooner you clean a dust pile, the more mana you will receive. I’ll remind you again later.")
        },
        {
            Tuple.Create(5, "lockOn"), Tuple.Create("Cassidy", "Sometimes you’ll need to focus on a specific spot that needs cleaning. Try locking onto one of those dummies.")
        },
        {
            Tuple.Create(6, "lockOn"), Tuple.Create("The Maid","A training dummy? This is getting awfully suspicious…")
        },
        {
            Tuple.Create(7, "lockedDummy"), Tuple.Create("Cassidy","While locked on, your broom swings and abilities will be automatically directed towards your target. This is useful for abilities, since they can be difficult to aim otherwise.")
        },
        {
            Tuple.Create(8, "lockedDummy"), Tuple.Create("Cassidy","Try using abilities while locked-on.")
        },
        {
            Tuple.Create(9, "dummyDone"), Tuple.Create("Cassidy","Ok looks like we’re done over here. Proceed to the other half of the room by climbing over those books")
        },
        {
            Tuple.Create(10, "enemiesAppear"), Tuple.Create("Cassidy","Oh, I forgot to tell you, this magic in this place is causing some of the dust to turn into hostile monsters. Rolling can help you avoid dying; you can’t be hit for a brief time while rolling. Pretty much everything I told you about cleaning should apply to these guys too.")
        },
        {
            Tuple.Create(11, "readytoFight"), Tuple.Create("The Maid","*Sigh*. Looks like I’ll be cleaning house in more ways than one today.")
        },
        {
            Tuple.Create(12, "enemiesDefeated"), Tuple.Create("Cassidy","Excellent! This room is now clean. And you’re still alive, that’s good too. Proceed to the door to the next room, you're health will be replished at the start of each room.")
        }

    };

    // Start is called before the first frame update
    void Start()
    {
        dialoguebox = GameObject.Find("DialogueBox");
        continueButton = GameObject.Find("ContinueButton");
        controls = GameObject.Find("Controls");
        tutorialManager = gameObject.GetComponent<TutorialManager>();
        tutorialManager.stopActions();
        controls.SetActive(false);
        StartDialogue();
    }

    public void StartDialogue ()
    {
        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        curState = (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2;
        Debug.Log("Starting conversation with " + speaker);
        Debug.Log("current key" + (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2);

        nameText.text = speaker;
        
        dialogueIndex = 0;
        
        DisplayNextSentence();
    }

    public void DisplayNextSentence() 
    {
        if(dialogueIndex == 13){
            EndDialogue();
        }

        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        string speakerText = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item2;
        curState = (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2;
        //Debug.Log("Starting conversation with " + speaker);

        // setting the text in the dialogue box
        nameText.text = speaker;
        dialogueText.text = speakerText;
        // move on to next line in dialogueDatabase
        dialogueIndex++;
    }

    void Update()
    {

        if(curState == "cleanPile" &&  dialogueIndex == 3)
        {
            cleanPile();
        }

        if(curState == "teachMana" && dialogueIndex == 5)
        {
            teachMana();  
        }

        if(curState == "lockedDummy" && dialogueIndex == 8)
        {
            lockedDummy();
        }

        if(curState == "dummyDone" && dialogueIndex == 10)
        {
            if(controlgiven == false)
            {
                tutorialManager.resumeActions();                
                controlgiven = true;
            }
            dialoguebox.SetActive(false);
            continueButton.SetActive(false);
            gameManager.infiniteManaCheat = true;
            //if(targetLocked && ) Find a way to check when the player has hit an enemy 

        }

    }

    void cleanPile(){
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);

        if(gameManager.numberOfDustPiles == 0 && curState == "cleanPile")
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
        }
    }

    void teachMana(){
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        if(abilitiesUsed < 3){
            for (int i = 0; i < tutorialManager.controller.playerAbilities.Length; i++)
            {
                if (tutorialManager.controller.abilityActions[i].triggered && tutorialManager.controller.playerAbilities[i] != null)
                {
                    abilitiesUsed++;
                }
            }
        }
        if(abilitiesUsed == 3 ){
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
        }        
    }

    void lockedDummy()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        targetLocked = targetLock.isTargeting;
        if(targetLocked == true)
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
        }
    }

    void EndDialogue ()
    {   
        continueButton.SetActive(false);
        dialoguebox.SetActive(false);
        Debug.Log("End of conversation.");
        dialogueIndex = 0;
    }
}
