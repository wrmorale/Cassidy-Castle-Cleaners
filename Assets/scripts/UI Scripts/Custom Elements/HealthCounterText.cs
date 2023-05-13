using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class HealthCounterText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI healthCounter;

    private float MaxHealth;

    public void setMaxHealth(float MHealth){
        MaxHealth = MHealth;
    }

    public void updateHealthCounter(float health){
        healthCounter.text =  health.ToString() + "/" + MaxHealth.ToString(); 
    }


}