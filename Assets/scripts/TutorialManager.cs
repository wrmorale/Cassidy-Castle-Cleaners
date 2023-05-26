using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameManager gameManager;
    public playerController controller;
    public Player player;
    public GameObject dummy1;
    public GameObject dummy2;
    public GameObject bookstack;
    public TutorialTrigger CombatTrigger;
    public GameObject DustBunny1;
    public GameObject DustBunny2;
    public GameObject DustBunny3;

    void Start()
    {
        //gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        //controller = GameObject.Find("Player").GetComponent<playerController>();
        //bookstack = GameObject.Find("Bookstack_med");
        //CombatTrigger = GameObject.Find("CombatTrigger").GetComponent<TutorialTrigger>();
        //player = GameObject.Find("Player").GetComponent<Player>();
        //dummy1 = GameObject.Find("Dummy1").GetComponent<Enemy>();
        //dummy2 = GameObject.Find("Dummy2").GetComponent<Enemy>();
    }

    public void showBunnies()
    {
        DustBunny1.SetActive(true);
        DustBunny2.SetActive(true);
        DustBunny3.SetActive(true);
        DustBunny1.GetComponent<DustBunny>().enabled = false;
        DustBunny2.GetComponent<DustBunny>().enabled = false;
        DustBunny3.GetComponent<DustBunny>().enabled = false;
    }

    public void activateBunnies()
    {
        DustBunny1.GetComponent<DustBunny>().enabled = true;
        DustBunny2.GetComponent<DustBunny>().enabled = true;
        DustBunny3.GetComponent<DustBunny>().enabled = true;
    }

    public void hideBunnies()
    {
        DustBunny1.SetActive(false);
        DustBunny2.SetActive(false);
        DustBunny3.SetActive(false);
    }
    


    public void enableBookstack()
    {
        bookstack.SetActive(true);
    }

    public void disableBookstack()
    {
        bookstack.SetActive(false);
    }

    public void stopActions()
    {
        controller.SetState(States.PlayerStates.Talking);
        player.animator.SetBool("Falling", false); 
        player.animator.SetBool("Jumping", false);
        player.animator.SetBool("Walking", false);      
        player.animator.SetBool("Running", false);
        player.animator.SetBool("Attacking", false);
        player.animator.SetBool("Rolling", false);
        player.atkmanager.SetWeaponCollider(false);
        player.atkmanager.combo = 0;
        player.atkmanager.activeClip.animator.SetInteger("Combo", 0);
        player.isInvulnerable = false;
        player.atkmanager.broom.SetActive(false);
        player.atkmanager.pan.SetActive(false);
    }

    public void resumeActions(){
        controller.SetState(States.PlayerStates.Idle);

    }

}
