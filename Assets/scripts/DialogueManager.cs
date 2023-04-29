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
    private Queue<string> sentences;

    public GameObject dialoguebox;

    public static int dialogueIndex;

    private Dictionary<string, string> dialogueDatabase = new Dictionary<string, string>()
    {
        {
            "Cassidy", "Listen, new hire. I know you want to jump right into things, but we should go ahead and teach you the basics. First, clean these dust piles. Look around if you don't see them right in front of you.\r\n"
        },
        {
            "The Maid", "I'm not that new to cleaning, you know ..."
        },
        {
            "Cassidy", "Good work. Make sure you move to the next room, and quickly."
        },
        {
            "Cassidy", "Oh, we didn't tell you but I hope you know how to fight, because this castle is full of monsters. Good luck."
        },
        {
            "The Maid", "Talk about a hostile work environment."
        },
        {
            "Cassidy", "Congradulation on not dying. Remember, you get paid to clean everything, not just the floors, so take out these monsters while you are at it."
        }

    };

    // Start is called before the first frame update
    void Start()
    {
        dialogueIndex = 0;

        dialoguebox = GameObject.Find("DialogueBox");
    }

    public void StartDialogue ()
    {
        Debug.Log("Starting conversation with " + dialogueDatabase.ElementAt(dialogueIndex).Key);

        nameText.text = dialogueDatabase.ElementAt(dialogueIndex).Key;

        DisplayNextSentence();
    }

    public void DisplayNextSentence() 
    {
        if (dialogueIndex == 6)
        {
            EndDialogue();
            return;
        }
        else if (dialogueIndex == 2){
            dialoguebox.SetActive(false);
        }

        dialogueText.text = dialogueDatabase.ElementAt(dialogueIndex).Value;
        dialogueIndex++;
    }

    void EndDialogue ()
    {
        Debug.Log("End of conversation.");
    }
}
