using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{

    public GameManager gameManager;
    public playerController controller;
    public Player player;

    void Start()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        controller = GameObject.Find("Player").GetComponent<playerController>();
        player = GameObject.Find("Player").GetComponent<Player>();
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
