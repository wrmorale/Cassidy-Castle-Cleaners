using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeImage : MonoBehaviour
{
    public Image original;
    public Sprite newImg;

    public void newImage(){
        original.sprite = newImg;
    }
}
