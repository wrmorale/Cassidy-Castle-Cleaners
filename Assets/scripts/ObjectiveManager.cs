using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;

public class ObjectiveManager : MonoBehaviour
{
    public TextMeshProUGUI objectiveText;
    public GameObject objectivePanel;
    public Image check;
    public Image checkbox;

    // dictionary for the objective list 
    private Dictionary<string, string> objectiveList = new Dictionary<string, string>()
    {   {"cleanPile", "Clean the 5 dust piles"},
        {"teachMana", "Use your abilities 3 times"},
        {"lockedDummy", "Lock onto a dummy"},
        {"lockedDummyMelee", "Use your primary attacks\n6 times while locked on"},
        {"lockedDummyAbility", "Use abilities 4 times\nwhile locked on"},
        {"dummyDone", "Jump over the books"},
        {"fighting",  "Defeat the Dust Bunnies\n(Use roll to avoid damage)"},
        {"enemiesDefeated", "Proceed through the door"}
    };

    void Start()
    {
        objectivePanel = GameObject.Find("ObjectivePanel");
        check = GameObject.Find("Check").GetComponent<Image>();
        checkbox = GameObject.Find("Checkbox").GetComponent<Image>();
        check.enabled = false;
        checkbox.enabled = false;
        objectiveText.text = "";
        objectivePanel.SetActive(false);
    }

    public void checkObjective() {
        check.enabled = true;
    }

    public void updateObjectiveTextCleaning(int dustPilesCleaned) {
        objectiveText.text = "Clean the " + (5 - dustPilesCleaned) + "/5 dust piles";
    }

    public void updateObjectiveTextAbilityUse(int abilityUses) {
        objectiveText.text = "Use your abilities " + abilityUses + "/3 times";
    }

    public void updateObjectiveTextPrimaryAttack(int primaryAttacks) {
        objectiveText.text = "Use your primary attacks " + primaryAttacks + "/6\ntimes while locked on";
    }

    public void updateObjectiveTextAbilityAttack(int abilityAttacks) {
        objectiveText.text = "Use your abilities " + abilityAttacks + "/4 times\ntimes while locked on";
    }


    public void startObjective(string state) {
        objectivePanel.SetActive(true);
        checkbox.enabled = true;
        objectiveText.text = objectiveList[state];
    }

    public void displayNextObjective(string state) {
        check.enabled = false;
        objectiveText.text = objectiveList[state];
    }

    public void endObjective() {
        objectivePanel.SetActive(false);
        objectiveText.text = "";
        check.enabled = false;
        checkbox.enabled = false;
    }

    public void transitionObjective(string state) {
        checkbox.enabled = false;
        check.enabled = false;
        objectiveText.text = objectiveList[state];
    }

    public void reappearBox() {
        checkbox.enabled = true;
    }
}