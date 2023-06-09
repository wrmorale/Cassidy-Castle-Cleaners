using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.EventSystems;

public class NewAbilitiesManager : MonoBehaviour
{
    public GameObject abilityThree;
    public GameObject abilityFour;
    public GameObject continueButton;
    public GameObject abilityThreeOverlay;
    public GameObject abilityFourOverlay;
    public GameObject abilityThreeIcon;
    public GameObject abilityFourIcon;
    public GameObject abilitiesCanvas;
    public GameManager gameManager;
    public Player player;
    public playerController controller;
    public EventSystem eventSystem;
    public GameObject mopAnimation;
    public GameObject soapAnimation;
    public GameObject loadingAnimationThree;
    public GameObject loadingAnimationFour;


    // dialogue box
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI displayName;
    public int dialogueIndex = 0;

    private String[] abilityThreeDialogue = new String[] {
        "Nice work so far. I thought you could use a little boost.",
        "Looks like the app finished installing. You now have access to a third ability.",
        "Time ticking go and finish the job."
    };
    
    private String[] abilityFourDialogue = new String[] {
        "I was looking at the app store for some free abilities and found this one.",
        "Who needs a Dyson vacuum when you can use a good old mop.",
        "Though if you break it I'm not getting you another one."
    };

    void Start()
    {
        eventSystem = EventSystem.current;
        displayName.text = "Cassidy";
        // disable all animations, and icons
        mopAnimation.SetActive(false);
        soapAnimation.SetActive(false);
        loadingAnimationThree.SetActive(false);
        loadingAnimationFour.SetActive(false);
        abilityThreeIcon.SetActive(false);
        abilityFourIcon.SetActive(false);
    }

    public void displayAbilityThree() {
        if (gameManager.roomCleared){
            freezePlayer();

            // make the canvas appear
            if(abilitiesCanvas.activeSelf == false){
                abilitiesCanvas.SetActive(true);
                eventSystem.SetSelectedGameObject(continueButton);
                dialogueText.text = abilityThreeDialogue[dialogueIndex];
            }
            // begin dialogue
            if (dialogueIndex == 0){
                // loading the idle animation for ability three
                loadingAnimationThree.SetActive(true);
            }
            else if (dialogueIndex == 1){
                // play soap animation, disable the loading animation and  grey overlay
                loadingAnimationThree.SetActive(false);
                abilityThreeOverlay.SetActive(false);
                soapAnimation.SetActive(true);
            }
            else if (dialogueIndex == 2){
                // disable soap animation and enable the icon
                soapAnimation.SetActive(false);
                abilityThreeIcon.SetActive(true);
            }
             
        }
    }

    public void advanceDialogue(){
        if(dialogueIndex == 3){
            resetAbilityCheckpoint();
        }
        else if (dialogueIndex < 3){
            dialogueIndex++;
            dialogueText.text = abilityThreeDialogue[dialogueIndex];
        }  
    }

    public void advanceDialogueFour(){
        if(dialogueIndex == 3){
            resetAbilityCheckpoint();
        }  
        dialogueIndex++;
        dialogueText.text = abilityFourDialogue[dialogueIndex];
    }   

    public void displayAbilityFour() {
        if (gameManager.roomCleared){
            freezePlayer();
            // make the canvas appear
            if(abilitiesCanvas.activeSelf == false){
                abilitiesCanvas.SetActive(true);
                eventSystem.SetSelectedGameObject(continueButton);
                dialogueText.text = abilityFourDialogue[dialogueIndex];
            }

            // begin dialogue
            if (dialogueIndex == 0){
                // enable loading animation and have icon thee ability show up
                // since the player obtained it already
                abilityThreeOverlay.SetActive(false);
                abilityThreeIcon.SetActive(true);
                loadingAnimationFour.SetActive(true);
            }
            else if (dialogueIndex == 1){
                // disable loading animation and enable the mop animation
                loadingAnimationFour.SetActive(false);
                abilityFourOverlay.SetActive(false);
                mopAnimation.SetActive(true);
            }
            else if (dialogueIndex == 3){
                // load the icon and disable mop animation
                abilityFourIcon.SetActive(true);
                mopAnimation.SetActive(false);
            }
            if(dialogueIndex == 3){
                resetAbilityCheckpoint();
            }
        }
    }

    public void resetAbilityCheckpoint(){
        abilitiesCanvas.SetActive(false);
        dialogueIndex = 0;
        gameManager.persistentGM.PushLastPlayerHealth(gameManager.playerStats.health, gameManager.mana);
        gameManager.levelLoader.LoadNextLevel();
        gameManager.isNextToExit = false;
    }

    public void freezePlayer(){
        controller.SetState(States.PlayerStates.Talking);
        player.animator.SetBool("Falling", false); 
        player.animator.SetBool("Jumping", false);
        player.animator.SetBool("Walking", false);      
        player.animator.SetBool("Running", false);
        player.animator.SetBool("Attacking", false);
        player.animator.SetBool("Rolling", false);
        player.atkmanager.SetWeaponCollider(false);
        player.atkmanager.combo = 0;
        player.atkmanager.activeClip.animator.SetInteger("Combo", 0);
        player.isInvulnerable = false;
        player.atkmanager.broom.SetActive(false);
        player.atkmanager.pan.SetActive(false);
    }
}