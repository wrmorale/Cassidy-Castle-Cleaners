using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class NewAbilitiesManager : MonoBehaviour
{
    public GameObject abilityThree;
    public GameObject abilityFour;
    public Image abilityThreeOverlay;
    public Image abilityFourOverlay;
    public Image abilityThreeIcon;
    public Image abilityFourIcon;
    public GameObject abilitiesCanvas;
    public GameManager gameManager;
    public Player player;
    public playerController controller;

    // dialogue box
    public GameObject dialogueBox;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI displayName;
    public static int dialogueIndex = 0;

    private String[] abilityThreeDialogue = new String[] {
        "Nice work so far. I thought you could use a little boost.",
        "Looks like the app finished installing. You now have access to a third ability.",
        "Time ticking go and finish the job."
    };
    
    private String[] abilityFourDialogue = new String[] {
        "I was looking at the app store for some free abilities and found this one.",
        "Who needs a Dyson vaccum when you can use a good old mop.",
        "Though if you break it I'm not getting you another one."
    };

    void Start()
    {
        abilityThree = GameObject.Find("AbilityThree");
        abilityThreeOverlay = GameObject.Find("AbilityThreeOverlay").GetComponent<Image>();
        
        abilityFour = GameObject.Find("AbilityFour");
        abilityFourOverlay = GameObject.Find("AbilityFourOverlay").GetComponent<Image>();

        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        abilitiesCanvas = GameObject.Find("AbilitiesCanvas");
        abilitiesCanvas.SetActive(false);

        displayName = GameObject.Find("Name").GetComponent<TextMeshProUGUI>();
        dialogueText = GameObject.Find("Dialogue").GetComponent<TextMeshProUGUI>();
        displayName.text = "Cassidy";
    }

    public void displayAbilityThree() {
        if (gameManager.roomCleared){
            freezePlayer();

            // make the canvas appear
            abilitiesCanvas.SetActive(true);

            // begin dialogue
            if (dialogueIndex == 0){
                // todo: load and play loading animation
            }
            else if (dialogueIndex == 1){
                // todo: play soap bar animation
                abilityThreeOverlay.enabled = false;
                abilityThreeIcon.enabled = false;
            }
            else if (dialogueIndex == 3){
                abilityThreeIcon.enabled = true;
            }
            
            if (dialogueIndex < 3){
                dialogueText.text = abilityThreeDialogue[dialogueIndex];
                dialogueIndex++;
            }
            else {
                resetAbilityCheckpoint();
            }  
        }
    }

    public void displayAbilityFour() {
        if (gameManager.roomCleared){
            freezePlayer();

            // make the canvas appear
            abilitiesCanvas.SetActive(true);

            // begin dialogue
            if (dialogueIndex == 0){
                // todo: load and play loading animation
            }
            else if (dialogueIndex == 1){
                // todo: play mop animation
                abilityFourOverlay.enabled = false;
                abilityFourIcon.enabled = false;
            }
            else if (dialogueIndex == 3){
                abilityFourIcon.enabled = true;
            }
            
            if (dialogueIndex < 3){
                dialogueText.text = abilityFourDialogue[dialogueIndex];
                dialogueIndex++;
            }
            else {
                resetAbilityCheckpoint();
            }
        }
    }

    public void resetAbilityCheckpoint(){
        abilitiesCanvas.SetActive(false);
        dialogueIndex = 0;
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