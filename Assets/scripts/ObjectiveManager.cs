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
    {   {"cleanPile", "Clean the dust piles"},
        {"teachMana", "Use your abilities!"},
        {"lockedDummy", "lock on to the dummy"},
        {"lockedDummyMelee", "Use your basic attacks while locked on"},
        {"lockedDummyAbility", "Use abilities while locked on"},
        {"dummyDone", "Climb over the books"},
        {"enemiesAppear", "Defeat the enemies"},
        {"readytoFight", "Defeat the enemies"},
        {"enemiesDefeated", "Proceed to the next room"}
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

    public void transitionObjective() {
        checkbox.enabled = false;
        check.enabled = false;
        objectiveText.text = objectiveList["dummyDone"];
    }

    public void reappearBox() {
        checkbox.enabled = true;
    }
}