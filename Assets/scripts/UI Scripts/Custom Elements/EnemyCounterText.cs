using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemyCounterText : MonoBehaviour
{

    [SerializeField]
    private TextMeshProUGUI enemiesCounter;

    public void updateEnemyCounter(float enemies){
        enemiesCounter.text = enemies.ToString(); 
    }


}