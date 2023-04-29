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


    private Dictionary<int, Tuple<string, string>> dialogueDatabase = new Dictionary<int, Tuple<string, string>>()
    {
        {
            0, Tuple.Create("Cassidy", "Listen, new hire. I know you want to jump right into things, but we should go ahead and teach you the basics. First, clean these dust piles. Look around if you don't see them right in front of you.")
        },
        {
            1, Tuple.Create("The Maid", "I'm not that new to cleaning, you know ...")
        },
        {
            2, Tuple.Create("Cassidy", "Good work. Make sure you move to the next room, and quickly.")
        },
        {
            3, Tuple.Create("Cassidy", "Oh, we didn't tell you but I hope you know how to fight, because this castle is full of monsters. Good luck.")
        },
        {
            4, Tuple.Create("The Maid","Talk about a hostile work environment.")
        },
        {
            5, Tuple.Create("Cassidy","Congratulations on not dying. Remember, you get paid to clean everything, not just the floors, so take out these monsters while you are at it.")
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
        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        Debug.Log("Starting conversation with " + speaker);

        nameText.text = speaker;

        DisplayNextSentence();
    }

    public void DisplayNextSentence() 
    {
        string speaker = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item1;
        Debug.Log("Starting conversation with " + speaker);

        nameText.text = speaker;

        string speakerText = (dialogueDatabase.ElementAt(dialogueIndex).Value).Item2;
        if (dialogueIndex == 6)
        {
            EndDialogue();
            return;
        }
        else if (dialogueIndex == 2){
            dialoguebox.SetActive(false);
        }
        else {
             dialogueText.text = speakerText;
             dialogueIndex++;
        }
    }

    void EndDialogue ()
    {
        Debug.Log("End of conversation.");
    }
}
