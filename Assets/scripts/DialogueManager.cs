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
    public GameObject portrait;
    public int dialogueIndex;
    //Bools and other things for states of the Tutorial
    [SerializeField]private string curState; 
    [SerializeField]private bool controlgiven = false;
    [SerializeField]private int abilitiesUsed = 0;
    [SerializeField]private bool targetLocked = false;
    [SerializeField]private float dummy1TopHealth = 0;
    [SerializeField]private float dummy2TopHealth = 0;
    [SerializeField]private float dummy1Health = 0;
    [SerializeField]private float dummy2Health = 0;
    [SerializeField]private int dummiesHit = 0; 

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
            Tuple.Create(12, "enemiesDefeated"), Tuple.Create("Cassidy","Excellent! This room is now clean. And you’re still alive, that’s good too. Proceed to the door to the next room.")
        }

    };

    // Start is called before the first frame update
    void Start()
    {
        dialoguebox = GameObject.Find("DialogueBox");
        continueButton = GameObject.Find("ContinueButton");
        controls = GameObject.Find("Controls");
        portrait = GameObject.Find("Portrait");
        controls.SetActive(false);
        tutorialManager = gameObject.GetComponent<TutorialManager>();
        tutorialManager.stopActions();
        tutorialManager.disableBookstack();
        tutorialManager.hideBunnies();
        dummy1TopHealth = tutorialManager.dummy1.GetComponent<Enemy>().maxHealth;
        dummy1Health = dummy1TopHealth;
        dummy2TopHealth = tutorialManager.dummy2.GetComponent<Enemy>().maxHealth;
        dummy2Health = dummy2TopHealth;
        StartDialogue();
    }

    void Update()
    {

        if(dialogueIndex == 3)
        {
            cleanPile();
        }

        if(dialogueIndex == 5)
        {
            teachMana();  
        }

        if(dialogueIndex == 8)
        {
            lockedDummy();
        }

        if(dialogueIndex == 10)
        {
            dummyDone();    
        }

        if(dialogueIndex == 11)
        {
            part1Done();
            startCombat();
        }

        if(dialogueIndex == 13){
            roomCleared();
        }
    }

     public void StartDialogue ()
    {
        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        curState = (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2;
        Debug.Log("Starting conversation with " + speaker);
        //Debug.Log("current key" + (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2);

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

        // setting portrait 
         if (string.Compare(speaker,"Cassidy") == 0){
            portrait.SetActive(true);
        }
        else if (string.Compare(speaker, "The Maid") == 0){
            Debug.Log("disappearPortrait: " + speaker);
            portrait.SetActive(false);
        }
        // move on to next line in dialogueDatabase
        dialogueIndex++;
    }

    void cleanPile()
    {
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

    void teachMana()
    {
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
                if (tutorialManager.controller.abilityActions[i].triggered 
                && tutorialManager.controller.playerAbilities[i] != null 
                && tutorialManager.controller.channeledAbility != 1)
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
            targetLocked = false;
        }
    }

    void dummyDone()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            tutorialManager.dummy1.GetComponent<Enemy>().currentHealth = dummy1TopHealth;
            tutorialManager.dummy2.GetComponent<Enemy>().currentHealth = dummy2TopHealth;
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        gameManager.infiniteManaCheat = true;
        dummy1Health = tutorialManager.dummy1.GetComponent<Enemy>().currentHealth;
        dummy2Health = tutorialManager.dummy2.GetComponent<Enemy>().currentHealth;
        targetLocked = targetLock.isTargeting;
        //if(targetLocked && ) Find a way to check when the player has hit an enemy 
        if((dummy1Health < dummy1TopHealth || dummy2Health < dummy2TopHealth) && targetLocked)
        {
            dummiesHit++;
            dummy1TopHealth = dummy1Health;
            dummy2TopHealth = dummy2Health;
        }

        if(dummiesHit >= 10)
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            gameManager.infiniteManaCheat = false;
            controlgiven = false;
        }
    }

    void part1Done()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            tutorialManager.enableBookstack();
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
    }

    void startCombat()
    {
        if(tutorialManager.CombatTrigger.triggered == true)
        {
            tutorialManager.showBunnies();
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
        }
    }
    
    void roomCleared()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();
            tutorialManager.activateBunnies();               
            controlgiven = true;
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);

        if(gameManager.numberOfEnemies == 2){
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
        }

    }

    void EndDialogue ()
    {   
        dialogueIndex = 0;
        continueButton.SetActive(false);
        dialoguebox.SetActive(false);
        //Debug.Log("End of conversation.");
        tutorialManager.dummy1.SetActive(false);
        tutorialManager.dummy2.SetActive(false);
        gameManager.mana = 0;
        gameManager.updateManaAmount(gameManager.mana);
        tutorialManager.player.health = 25f;
        tutorialManager.player.updateHealthUI();    
    }
}
