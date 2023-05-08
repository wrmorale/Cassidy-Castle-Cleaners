using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public GameObject[] popUps;
    private int popUpIndex;

    // Update is called once per frame
    void Update()
    {
        if (popUpIndex == 0){
            FindObjectOfType<DialogueTrigger>().TriggerDialogue();
        }
    }
}
