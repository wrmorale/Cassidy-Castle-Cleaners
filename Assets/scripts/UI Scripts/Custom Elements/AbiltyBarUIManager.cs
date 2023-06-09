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
    
    void Start()
    {
        iconSlot = GameObject.Find("IconSpace").GetComponent<Image>();
        iconSlotTwo = GameObject.Find("IconSpaceTwo").GetComponent<Image>();
    }

    void Update()
    {
        if (SceneManager.GetActiveScene().buildIndex >= 3)
        {
            DisplayBarIcon();
        }
        if (SceneManager.GetActiveScene().buildIndex >= 5)
        {
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
