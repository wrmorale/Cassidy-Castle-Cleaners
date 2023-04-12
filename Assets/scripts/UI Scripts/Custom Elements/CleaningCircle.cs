using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CleaningCircle : MonoBehaviour
{
    [SerializeField]
    private Image cleaningCircle;

    public void setCleaning(float cleanVal){
        cleaningCircle.fillAmount = cleanVal;
    }
}
