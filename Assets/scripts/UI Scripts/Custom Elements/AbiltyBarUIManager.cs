using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;


public class AbiltyBarUIManager : MonoBehaviour
{
    public Sprite soapBarIcon;
    public Sprite mopIcon;
    public Image iconSlot;
    public Image iconSlotTwo;
    public GameObject button3;
    public GameObject button4;
    
    void Start()
    {
        
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 3)
        {
            button3.SetActive(true);
            DisplayBarIcon();
        }
        if (SceneManager.GetActiveScene().buildIndex >= 5)
        {
            button4.SetActive(true);
            DisplayMopIcon();
        }
    }

    void DisplayBarIcon()
    {
        iconSlot.color = new Color(iconSlot.color.r, iconSlot.color.g, iconSlot.color.b, 255);
        iconSlot.sprite = soapBarIcon;
    }

    void DisplayMopIcon()
    {
        iconSlotTwo.color = new Color(iconSlot.color.r, iconSlot.color.g, iconSlot.color.b, 255);
        iconSlotTwo.sprite = mopIcon;
    }
}
