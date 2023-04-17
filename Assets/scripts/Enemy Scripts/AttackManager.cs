using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using Extensions;

public class AttackManager : MonoBehaviour, IFrameCheckHandler
{
    [SerializeField]
    private Golem enemyInstance; //Getting rid of this later

    public List<AttackData> attacks;
    private AttackData currentAttack;

    enum ActionState {Inactionable, AttackCancelable}
    private ActionState actionState;

    public void onActiveFrameStart() {
        if(enemyInstance)
            enemyInstance.light1Complete = false;

        currentAttack.hitbox.SetActive(true);
    }

    public void onActiveFrameEnd() {

        currentAttack.hitbox.SetActive(false);
        //Will work as long as every attack name matches its corresponding animator bool
        currentAttack.clip.animator.SetBool(currentAttack.name, false);

        if (enemyInstance)
        {
            enemyInstance.state = Golem.GolemState.Idle;
            enemyInstance.isDashing = false;
            if(currentAttack.name == "Light1")
            {
                enemyInstance.light1Complete = true;
            }
        }

        /*
        enemyInstance.state = Golem.GolemState.Idle; //<- One of the biggest problems with this entire setup: Every enemy has their own defined states!
        enemyInstance.isDashing = false;
        if(currentAttack == "Light1"){
            enemyInstance.light1Complete = true;
            light1Collider.SetActive(false);
            activeClip.animator.SetBool("Light1", false);
        }
        else if(currentAttack == "Light2"){
            light2Collider.SetActive(false);
            activeClip.animator.SetBool("Light2", false);
        }
        else if(currentAttack == "SpinAttack"){
            spinAttackCollider.SetActive(false);
            activeClip.animator.SetBool("SpinAttack", false);
        }
        else if(currentAttack == "Dash"){
            dashCollider.SetActive(false);
            activeClip.animator.SetBool("Dash", false);
        }*/
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
        currentAttack.clip.animator.SetBool(currentAttack.name, false);

        if (enemyInstance)
        {
            enemyInstance.state = Golem.GolemState.Idle;
            enemyInstance.isDashing = false;
        }
        //enemyInstance.state = Golem.GolemState.Idle;
        //enemyInstance.isDashing = false;
        //activeClip.animator.SetBool("Light1", false);
        //activeClip.animator.SetBool("Light2", false);
        //activeClip.animator.SetBool("SpinAttack", false);
        //activeClip.animator.SetBool("Dash", false);
    }

    void Awake()
    {
        foreach(AttackData attack in attacks)
        {
            attack.clip.initialize();
            attack.checker.initialize(this, attack.clip);
        }

        currentAttack = attacks[0];

        /*
        light1Clip.initialize();
        light1Checker.initialize(this, light1Clip);
        light2Clip.initialize();
        light2Checker.initialize(this, light2Clip);
        spinAttackClip.initialize();
        spinAttackChecker.initialize(this, spinAttackClip);
        dashClip.initialize();
        dashChecker.initialize(this, dashClip);

        activeChecker = light1Checker;
        activeClip = light1Clip;*/
    }

    public void updateMe(float time) // yes we need this
    {
        //activeChecker.checkFrames();
        currentAttack.checker.checkFrames();

        if (actionState == ActionState.Inactionable){}
        if (actionState == ActionState.AttackCancelable)
        {
            actionState = ActionState.Inactionable;
        }
    }
    public void handleAttacks(Ability ability)
    {
        actionState = ActionState.Inactionable;
        
        //Search for the attack with matching name
        //Or should we just remove the abilities list from the golem altogether?
        bool matchingAttack = false;
        foreach(AttackData attack in attacks)
        {
            if(attack.name == ability.abilityName)
            {
                currentAttack = attack;
                matchingAttack = true;
                break;
            }
        }

        if (!matchingAttack)
        {
            Debug.LogError("Enemy does not have an ability called " + ability.abilityName);
        }
        else
        {
            if (enemyInstance)
            {
                enemyInstance.state = Golem.GolemState.Attacking;
            }
            currentAttack.clip.animator.SetBool(currentAttack.name, true);
            currentAttack.clip.animator.Play(currentAttack.clip.animatorStateName, 0);
            currentAttack.checker.initCheck();
            currentAttack.checker.checkFrames();
        }

        /*
        int frames = 0; // amount of frames in anim 
        actionState = ActionState.Inactionable;
        enemyInstance.state = Golem.GolemState.Attacking;

        currentAttack = ability.abilityName;

        if (currentAttack == "Light1")
        {
            activeChecker = light1Checker;
            activeClip = light1Clip;
        }
        else if (currentAttack == "Light2")
        {
            activeChecker = light2Checker;
            activeClip = light2Clip;
        }
        else if (currentAttack == "SpinAttack")
        {
            activeChecker = spinAttackChecker;
            activeClip = spinAttackClip;
        }
        else if (currentAttack == "Dash")
        {
            activeChecker = dashChecker;
            activeClip = dashClip;
        }
        frames = activeClip.getTotalFrames();
        activeClip.animator.SetBool(ability.abilityName, true);
        activeClip.animator.Play(activeClip.animatorStateName, 0);
        activeChecker.initCheck();
        activeChecker.checkFrames();*/
    }
}

[System.Serializable]
public class AttackData
{
    public string name;
    public GameObject hitbox;
    public FrameParser clip;
    public FrameChecker checker;

    /*For clip.animator, couldn't we just have 1 variable for the animator of the enemy, 
     * since all attacks are going to use the same one?*/
    public AttackData(string n, GameObject h, FrameParser cl, FrameChecker ch)
    {
        name = n;
        hitbox = h;
        clip = cl;
        checker = ch;
    }
}
