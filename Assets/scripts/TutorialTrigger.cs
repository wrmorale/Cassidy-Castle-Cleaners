using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialTrigger : MonoBehaviour
{   
    public bool triggered = false;

    void Start(){
        
    }
    public void OnTriggerEnter(Collider other){
        if(other.tag == "Player")
        {
            triggered = true;
        }
        
    }

    public void OnTriggerExit(Collider other){
        if(other.tag == "Player" && triggered == true)
        {
            triggered = false;
        }
        
    }   
    
}
