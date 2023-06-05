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
    public GameObject abilityBox;
    public GameObject controlone;
    public GameObject controltwo;
    public GameObject controlthree;
    public GameObject controlfour;
    public GameObject controlfive;
    public TargetLock targetLock;
    [HideInInspector]public TutorialManager tutorialManager;
    public ObjectiveManager objectiveManager;
    public PauseMenu pauseMenu;
    public GameObject wall;
    public GameObject portrait;
    public int dialogueIndex;
    private bool alreadyPaused = false;
    private bool inDialogue = true;
    //Bools and other things for states of the Tutorial
    [SerializeField]private string curState; 
    [SerializeField]private bool controlgiven = false;
    [SerializeField]private int abilitiesUsed = 0;
    [SerializeField]private bool targetLocked = false;
    [SerializeField]private float dummy1TopHealth = 0;
    [SerializeField]private float dummy2TopHealth = 0;
    [SerializeField]private float dummy1Health = 0;
    [SerializeField]private float dummy2Health = 0;
    [SerializeField]private int meleedummiesHit = 0; 
    [SerializeField]private int abilitydummiesHit = 0; 
    // dialogueDatabase has a key containing line index and current state, the value is current speaker and line

    // TODO: create list of images to display at different stages of dialouge  
    
    private Dictionary<Tuple<int, string>, Tuple<string, string>> dialogueDatabase = new Dictionary<Tuple<int,string>, Tuple<string, string>>()
    {
        {
            Tuple.Create(0, "cleanPile"), Tuple.Create("Cassidy", "Listen, new hire. I know you want to jump right into things, but we should go ahead and teach you the basics.")
        },
        {
            Tuple.Create(1, "cleanPile"), Tuple.Create("Cassidy",  "First, clean these dust piles. Look around if you don't see them right in front of you.")
        },
        {
            Tuple.Create(2, "cleanPile"), Tuple.Create("The Maid", "I'm not that new to cleaning, you know ...")
        },
        {
            Tuple.Create(3, "cleanPile"), Tuple.Create("Cassidy", "Cleaning up dust piles gives you mana. See that blue circle in the top-left?")
        },
        {
            Tuple.Create(4, "teachMana"), Tuple.Create("Cassidy", "Mana is needed to use your secondary cleaning abilities, which each require a different amount of mana. Make sure they are operational.")
        },
        {
            Tuple.Create(5, "teachMana"), Tuple.Create("Cassidy", "By the way, this is important: When you enter a room, the sooner you clean a dust pile, the more mana you will receive.")
        },
        {
            Tuple.Create(6, "teachMana"), Tuple.Create("Cassidy", "I’ll be sure to remind you again later.")
        },
        {
            Tuple.Create(7, "lockOn"), Tuple.Create("Cassidy", "Sometimes you’ll need to focus on a specific spot that needs cleaning.Try locking onto one of those dummies.")
        },
        {
            Tuple.Create(8, "lockOn"), Tuple.Create("The Maid", "A training dummy? This is getting awfully suspicious…")
        },
        {
            Tuple.Create(9, "lockedDummy"), Tuple.Create("Cassidy", "While locked on, your broom swings will be automatically directed towards your target.")
        },
        {
            Tuple.Create(10, "lockedDummy"), Tuple.Create("Cassidy", "Try attack-er, cleaning the dummies with your broom while locked on. Remember to practive changing targets too.")
        },
        {
            Tuple.Create(11, "lockedDummyMelee"), Tuple.Create("Cassidy","Abilities are also automatically aimed at lock-on targets. This is very useful as they can difficult to aim otherwise and you don't want to be wasting mana.")
        },
        {
            Tuple.Create(12, "lockedDummyAbility"), Tuple.Create("Cassidy", "Try using abilities while locked-on.")
        },                                                            
        {
            Tuple.Create(13, "lockedDummyAbility"), Tuple.Create("Cassidy","Ok looks like we’re done over here. Proceed to the other half of the room by jumping over those books")
        },
        {
            Tuple.Create(14, "dummyDone"), Tuple.Create("Cassidy","Oh, I forgot to tell you, the magic in this place is causing some of the dust to turn into monsters. You'll need to clean them up too.")
        },
        {
            Tuple.Create(15, "enemiesAppear"), Tuple.Create("Cassidy", "You can't take damage briefly while rolling, so use that to avoid getting hit by the monsters.")
        },
        {
            Tuple.Create(16, "readytoFight"), Tuple.Create("The Maid","*Sigh* Talk about a hostile work enviroment...")
        },
        {
            Tuple.Create(17, "fighting"), Tuple.Create("Cassidy","Excellent! This room is now clean. And you’re still alive, that’s good too.")
        },
        {
            Tuple.Create(18, "enemiesDefeated"), Tuple.Create("Cassidy", "Proceed through the door to the next room. Remember to clean ALL of the dust piles and ALL of the monsters!")
        },

    };


    // Start is called before the first frame update
    void Start()
    {
        // dialogue 
        dialoguebox = GameObject.Find("DialogueBox");
        continueButton = GameObject.Find("ContinueButton");
        portrait = GameObject.Find("Portrait");
        abilityBox.SetActive(false);
        
        
        // controls
        //controls = GameObject.Find("Controls");
        //controls.SetActive(false);
        //controlone = GameObject.Find("ControlOne");
        //controlone.SetActive(false);    
        //controltwo = GameObject.Find("ControlTwo");
        //controltwo.SetActive(false);   
        //controlthree = GameObject.Find("ControlThree");
        //controlthree.SetActive(false);
        //controlfour = GameObject.Find("ControlFour");
        //controlfour.SetActive(false);

        // controls
        //controls = GameObject.Find("Controls");
        //controls.SetActive(false);
        //controlone = GameObject.Find("ControlOne");
        //controlone.SetActive(false);    
        //controltwo = GameObject.Find("ControlTwo");
        //controltwo.SetActive(false);   
        //controlthree = GameObject.Find("ControlThree");
        //controlthree.SetActive(false);
        //controlfour = GameObject.Find("ControlFour");
        //controlfour.SetActive(false);

        // tutorial Manager
        tutorialManager = gameObject.GetComponent<TutorialManager>();
        tutorialManager.stopActions();
        tutorialManager.disableBookstack();
        tutorialManager.hideBunnies();

        // env objects
        wall = GameObject.Find("InvisibleWall");
        dummy1TopHealth = tutorialManager.dummy1.GetComponent<Enemy>().maxHealth;
        dummy1Health = dummy1TopHealth;
        dummy2TopHealth = tutorialManager.dummy2.GetComponent<Enemy>().maxHealth;
        dummy2Health = dummy2TopHealth;
        tutorialManager.dummy1.SetActive(false);
        tutorialManager.dummy2.SetActive(false);

        // objective list
        objectiveManager = gameObject.GetComponent<ObjectiveManager>();
        
        StartDialogue();
    }

    // dictionary for objectives
    
    
    void Update()
    {
        if(dialogueIndex == 4)
        {
            cleanPile();
        }

        if(dialogueIndex == 6)
        {
            teachMana();  
        }

        if(dialogueIndex == 10)
        {
            lockedDummy();
        }
        if(dialogueIndex == 12){
            lockedDummyMelee();
        }
        if(dialogueIndex == 14){
            lockedDummyAbility();
        }
        
        if(dialogueIndex == 15)
        { 
            //dummyDone();
            part1Done();
            startCombat();    
        }

        if(dialogueIndex == 18)
        {
            roomCleared();

        }

        if(dialogueIndex == 20)
        {
            FinishTutorial();
        }
        
        HandlePauseInTutorial();

    }

    

     public void StartDialogue ()
    {
        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        curState = (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2;
        
        nameText.text = speaker;
        
        dialogueIndex = 0;
        
        DisplayNextSentence();
    }

    public void DisplayNextSentence() 
    {
        if(dialogueIndex == 19){
            EndDialogue();
        }

        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        string speakerText = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item2;
        curState = (dialogueDatabase.ElementAt(dialogueIndex).Key).Item2;

        // setting the text in the dialogue box
        nameText.text = speaker;
        dialogueText.text = speakerText;

        // setting portrait 
         if (string.Compare(speaker,"Cassidy") == 0){
            portrait.SetActive(true);
        }
        else if (string.Compare(speaker, "The Maid") == 0){
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
            inDialogue = false;
            controlone.SetActive(true);
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        objectiveManager.startObjective(curState);
        abilityBox.SetActive(true);

        if(gameManager.numberOfDustPiles == 0 && curState == "cleanPile")
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
            inDialogue = true;
            objectiveManager.checkObjective();
            controlone.SetActive(false);
            abilityBox.SetActive(false);
        }
    }

    void teachMana()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            controlgiven = true;
            inDialogue = false;
            tutorialManager.controller.castingAllowed = true;
            controltwo.SetActive(true);
        }

        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        abilityBox.SetActive(true);
        objectiveManager.displayNextObjective(curState);

        if(abilitiesUsed <= 3){
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
            inDialogue = true;
            objectiveManager.checkObjective();
            controltwo.SetActive(false);
            abilityBox.SetActive(false);
        }        
    }

    void lockedDummy()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            controlgiven = true;
            inDialogue = false;
            tutorialManager.dummy1.SetActive(true);
            tutorialManager.dummy2.SetActive(true);
            objectiveManager.displayNextObjective(curState);
            controlthree.SetActive(true);

        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        abilityBox.SetActive(true);
        targetLocked = targetLock.isTargeting;
        if(targetLocked == true)
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            controlgiven = false;
            targetLocked = false;
            inDialogue = true;
            objectiveManager.checkObjective();
            abilityBox.SetActive(false);
        }
    }

    void lockedDummyMelee()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            tutorialManager.dummy1.GetComponent<Enemy>().currentHealth = dummy1TopHealth;
            tutorialManager.dummy2.GetComponent<Enemy>().currentHealth = dummy2TopHealth;
            controlgiven = true;
            inDialogue = false;
            tutorialManager.controller.castingAllowed = false;
            objectiveManager.displayNextObjective(curState);
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        abilityBox.SetActive(true);
        //objectiveManager.displayNextObjective(curState);
        //gameManager.infiniteManaCheat = true;
        //gameManager.updateManaAmount(gameManager.mana);
        dummy1Health = tutorialManager.dummy1.GetComponent<Enemy>().currentHealth;
        dummy2Health = tutorialManager.dummy2.GetComponent<Enemy>().currentHealth;
        targetLocked = targetLock.isTargeting;
        //if(targetLocked && ) Find a way to check when the player has hit an enemy 
        if((dummy1Health < dummy1TopHealth || dummy2Health < dummy2TopHealth) && targetLocked)
        {
            meleedummiesHit++;
            dummy1TopHealth = dummy1Health;
            dummy2TopHealth = dummy2Health;
        }

        if(meleedummiesHit >= 6)
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            gameManager.infiniteManaCheat = false;
            controlgiven = false;
            inDialogue = true;
            objectiveManager.checkObjective();
            abilityBox.SetActive(false);
            
        }
    }

    void lockedDummyAbility()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            tutorialManager.dummy1.GetComponent<Enemy>().currentHealth = tutorialManager.dummy1.GetComponent<Enemy>().maxHealth;
            tutorialManager.dummy2.GetComponent<Enemy>().currentHealth = tutorialManager.dummy2.GetComponent<Enemy>().maxHealth;
            dummy1Health = tutorialManager.dummy1.GetComponent<Enemy>().currentHealth;
            dummy2Health = tutorialManager.dummy2.GetComponent<Enemy>().currentHealth;
            dummy1TopHealth = dummy1Health;
            dummy2TopHealth = dummy2Health;
            controlgiven = true;
            inDialogue = false;
            tutorialManager.controller.castingAllowed = true;
            objectiveManager.displayNextObjective(curState);
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        abilityBox.SetActive(true);
        //objectiveManager.displayNextObjective(curState);
        gameManager.infiniteManaCheat = true;
        gameManager.updateManaAmount(gameManager.mana);
        dummy1Health = tutorialManager.dummy1.GetComponent<Enemy>().currentHealth;
        dummy2Health = tutorialManager.dummy2.GetComponent<Enemy>().currentHealth;
        targetLocked = targetLock.isTargeting;
        //if(targetLocked && ) Find a way to check when the player has hit an enemy 
        for (int i = 0; i < tutorialManager.controller.playerAbilities.Length; i++)
            {
                if (tutorialManager.controller.abilityActions[i].triggered 
                && tutorialManager.controller.playerAbilities[i] != null 
                && tutorialManager.controller.channeledAbility != 1)
                {
                    if((dummy1Health < dummy1TopHealth || dummy2Health < dummy2TopHealth) && targetLocked)
                    {
                        abilitydummiesHit++;
                        dummy1TopHealth = dummy1Health;
                        dummy2TopHealth = dummy2Health;
                    }
                }
            }

        if(abilitydummiesHit >= 3)
        {
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            tutorialManager.stopActions();
            gameManager.infiniteManaCheat = false;
            controlgiven = false;
            inDialogue = true;
            objectiveManager.checkObjective();
            controlthree.SetActive(false);
            abilityBox.SetActive(false);
        }
    }

    void part1Done()
    {
        if(controlgiven == false)
        {
            tutorialManager.resumeActions();                
            tutorialManager.enableBookstack();
            controlgiven = true;
            inDialogue = false;
            objectiveManager.transitionObjective(curState);
            controlfour.SetActive(true);
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        wall.SetActive(false);
        abilityBox.SetActive(true);
    }

    void startCombat()
    {
        if(tutorialManager.CombatTrigger.triggered == true)
        {
            tutorialManager.showBunnies();
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            abilityBox.SetActive(false);
            tutorialManager.stopActions();
            controlgiven = false;
            wall.SetActive(true);
            inDialogue = true;
            //objectiveManager.reappearBox();
            //objectiveManager.displayNextObjective(curState);
            controlfour.SetActive(false);
            tutorialManager.dummy1.SetActive(false);
            tutorialManager.dummy2.SetActive(false);
            tutorialManager.tempDummy.SetActive(false);
            //tutorialControls.NextImage();
        }
    }
    
    void roomCleared()
    {
        if(controlgiven == false && gameManager.numberOfEnemies != 0) 
        {
            tutorialManager.resumeActions();
            tutorialManager.activateBunnies();               
            controlgiven = true;
            inDialogue = false;
            controlfive.SetActive(true);
            objectiveManager.reappearBox();
            objectiveManager.displayNextObjective(curState);
            
        }
        dialoguebox.SetActive(false);
        continueButton.SetActive(false);
        abilityBox.SetActive(true);

        if(gameManager.numberOfEnemies == 0){
            dialoguebox.SetActive(true);
            continueButton.SetActive(true);
            abilityBox.SetActive(false);
            tutorialManager.stopActions();
            controlgiven = false;
            inDialogue = true;
            objectiveManager.checkObjective();
            controlfive.SetActive(false);
        }

    }

    public void FinishTutorial()
    {
        if(controlgiven == false)
            {
                tutorialManager.resumeActions();                
                controlgiven = true;
                inDialogue = false;
            }
            dialoguebox.SetActive(false);
            continueButton.SetActive(false);
            abilityBox.SetActive(true);
            dialogueIndex = 0;
            objectiveManager.transitionObjective(curState);
    }

    void EndDialogue ()
    {   
        dialogueIndex = 20;
        //continueButton.SetActive(false);
        //dialoguebox.SetActive(false);
        
        gameManager.mana = 0;
        gameManager.updateManaAmount(gameManager.mana);
        tutorialManager.player.health = 25f;
        tutorialManager.player.updateHealthUI();
        //objectiveManager.endObjective();    
    }

    void HandlePauseInTutorial()
    {
        if(pauseMenu.isPaused)
        {
            dialoguebox.SetActive(false);
            continueButton.SetActive(false);
            portrait.SetActive(false);
            if(!alreadyPaused)
            {
                pauseMenu.eventSystem.SetSelectedGameObject(pauseMenu.resumeButton);
                alreadyPaused = true;
            }   
        }
        else if(!pauseMenu.isPaused)
        {
            if(inDialogue)
            {
                dialoguebox.SetActive(true);
                continueButton.SetActive(true);
                portrait.SetActive(true);
            }
            
            pauseMenu.eventSystem.SetSelectedGameObject(continueButton);
            alreadyPaused = false;
        }
    }
}
