using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonImageChange : MonoBehaviour
{
    public Image originalImage;
    public Sprite keyboardSprite;
    public Sprite controllerSprite;
    private bool pressed = false;

    public void newImage(){
        if(pressed == false){
            originalImage.sprite = controllerSprite;
            pressed = true;
        }else{
            originalImage.sprite = keyboardSprite;
            pressed = false;
        }
        
    }
}
