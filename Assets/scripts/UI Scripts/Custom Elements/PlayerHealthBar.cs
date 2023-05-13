using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthBar : MonoBehaviour
{
    [SerializeField]
    private Image healthbar;

    public void setHealth(float health){
        healthbar.fillAmount = health;
    }
}
