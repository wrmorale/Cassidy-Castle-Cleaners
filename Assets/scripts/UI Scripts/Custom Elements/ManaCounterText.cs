using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ManaCounterText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI manaCounter;

    public void updateManaCounter(float mana){
        manaCounter.text = mana.ToString(); 
    }


}