using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectFader : MonoBehaviour
{
    public float fadeSpeed, fadeAmount;
    float originalOpacity;
    Material mat;
    public bool doFade;

    // Start is called before the first frame update
    void Start()
    {
        mat = GetComponentInChildren<Renderer>().material;
        originalOpacity = mat.color.a;
    }

    // Update is called once per frame
    void Update()
    {
        if(doFade)
        {
            FadeObject();   
        }
        else
        {
            ResetFade();
        }
    }

    void FadeObject()
    {
        Color currentColor = mat.color;
        Color transitionColor = new Color(currentColor.r, currentColor.g, currentColor.b, 
            Mathf.Lerp(currentColor.a, fadeAmount, fadeSpeed * Time.deltaTime));
        mat.color = transitionColor;
    }

    void ResetFade()
    {
        Color currentColor = mat.color;
        Color transitionColor = new Color(currentColor.r, currentColor.g, currentColor.b, 
            Mathf.Lerp(currentColor.a, originalOpacity, fadeSpeed * Time.deltaTime));
        mat.color = transitionColor;
    }
}
