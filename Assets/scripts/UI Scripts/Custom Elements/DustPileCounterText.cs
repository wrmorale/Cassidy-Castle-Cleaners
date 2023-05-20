using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DustPileCounterText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI dustPileCounter;

    public void updateDustPileCounter(float dustpiles){
        dustPileCounter.text = dustpiles.ToString(); 
    }


}