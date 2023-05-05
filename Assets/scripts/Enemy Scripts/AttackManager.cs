using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using Extensions;

public class AttackManager : MonoBehaviour, IFrameCheckHandler
{
    public List<AttackData> attacks;
    private AttackData currentAttack;
    [HideInInspector] public bool attacking = false;

    enum ActionState {Inactionable, AttackCancelable}
    private ActionState actionState;

    void Awake()
    {
        foreach (AttackData attack in attacks)
        {
            attack.clip.initialize();
            attack.checker.initialize(this, attack.clip);
            attack.hitbox.GetComponent<collisionDetection>().damage = attack.damage; //Set the damage of the hitbox
        }

        currentAttack = attacks[0];
    }

    void Update()
    {
        if (attacking)
        {
            updateMe();
        }
            
    }

    public void onActiveFrameStart() {
        Debug.Log("Hitbox activated");
        currentAttack.hitbox.SetActive(true);
    }

    public void onActiveFrameEnd() {
        Debug.Log("Hitbox deactivated");
        currentAttack.hitbox.SetActive(false);
    }
    public void onAttackCancelFrameStart() {
        actionState = ActionState.AttackCancelable;
        //have it so that it can cast another attack after the last attack
        //if it wants to
    }
    public void onAttackCancelFrameEnd() {
        if (actionState == ActionState.AttackCancelable) actionState = ActionState.Inactionable;
    }
    public void onAllCancelFrameStart(){}
    public void onAllCancelFrameEnd(){}
    public void onLastFrameStart(){}
    public void onLastFrameEnd(){
        Debug.Log("Current attack finished");
        currentAttack.clip.animator.SetBool(currentAttack.name, false);
        attacking = false;
    }


    public void updateMe() // yes we need this
    {
        currentAttack.checker.checkFrames();

        if (actionState == ActionState.Inactionable){}
        if (actionState == ActionState.AttackCancelable)
        {
            actionState = ActionState.Inactionable;
        }
    }

    public AttackData getAttack(string attackName)
    {
        foreach (AttackData attack in attacks)
        {
            if (attack.name == attackName)
            {
                return attack;
            }
        }
        Debug.LogError("Enemy does not have an attack called " + attackName);
        return null;
    }

    public void handleAttacks(string attackName)
    {
        actionState = ActionState.Inactionable;
        currentAttack = getAttack(attackName);
        if (currentAttack != null)
        {
            currentAttack.clip.animator.SetBool(currentAttack.name, true);
            //currentAttack.clip.animator.Play(currentAttack.clip.animatorStateName, 0);
            currentAttack.checker.initCheck();
            currentAttack.checker.checkFrames();
            attacking = true;
            Debug.Log("Activating attack: " + currentAttack.name);
        }
    }

    public void stopAttack()
    {
        if (attacking)
        {
            currentAttack.hitbox.SetActive(false);
            currentAttack.clip.animator.SetBool(currentAttack.name, false);
            attacking = false;
            Debug.Log("Current attack canceled");
        }
    }
}

[System.Serializable]
public class AttackData
{
    public string name; //This MUST match the name of the corresponding animator bool
    public float damage = 1.0f;
    public float range = 1.0f;
    public GameObject hitbox;
    public FrameParser clip;
    public FrameChecker checker;

    /*For clip.animator, couldn't we just have 1 variable for the animator of the enemy, 
     * since all attacks are going to use the same one?*/
    /*Unforunatly I can't make the animator on FrameParser private because it's also used
     by the player and that would mess things up.*/
    public AttackData(string n, float d, float r, GameObject h, FrameParser cl, FrameChecker ch)
    {
        name = n;
        damage = d;
        range = r;
        hitbox = h;
        clip = cl;
        checker = ch;
    }
}
