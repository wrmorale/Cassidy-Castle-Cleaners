using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DustBunny : Enemy, IFrameCheckHandler
{
    [SerializeField]
    public GameObject pounceCollider;
    [SerializeField]
    private FrameParser pounceClip;
    [SerializeField]
    private FrameChecker pounceChecker;

    private FrameParser activeClip;
    private FrameChecker activeChecker;

    private string currentAttack;
    float postActionCooldown = 0.0f;

    enum ActionState {Inactionable, AttackCancelable}
    private ActionState actionState;

    bool isAggro = false;
    public enum BunnyState{
        Idle,
        Attacking
    }
    public BunnyState state = BunnyState.Idle;

    void FixedUpdate(){
        
    }

    void Update(){
        //Debug.Log("Longest attack range: " + longestAttackRange);
        if (isAttacking){
            //Won't move, should only be affected by gravity
            updateMe(Time.deltaTime);
        }
        else{
            enemyMovement();
            //Action cooldown timer is 0 by default, so it will attack as soon as possible
            //One problem seems to be the playerInRange function...
            if (playerInRange(longestAttackRange) && actionCooldownTimer <= 0 && playerInRange(movementRange)) //If off cooldown and player in range, perform action
            {
                enemyAction();
            }
        }

        actionCooldownTimer -= Time.deltaTime;

        /*
        //Do we have to call Super()?
        if (isAttacking){
            //Basically wait for attack animation to finish playing
            stateInfo = animator.GetCurrentAnimatorStateInfo(0);
            //Have to check the state name otherwise it may be checking the wrong animation
            if(stateInfo.normalizedTime >= 1 && stateInfo.IsName("Pounce")){
                animator.SetBool("Pounce", false);
                isAttacking = false;
                actionCooldownTimer = postActionCooldown;
                Debug.LogError("Finished attack animation");
            }
        }*/
    }

    //Problem with enemy animation: The bunny jumps forward to a spot but then resets to its original position.
    //The bunny should really just be jumping in place and then let the actual movement speed move them around
    private void enemyMovement() {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // if player is in range
        if(playerInRange(movementRange)){ //This means enemies will stop persuit if player gets far enough. Do we want that?
            // move enemy towards player
            // Only move if the current animation is complete? Is that just a misunderstanding of how looping works?
            animator.SetBool("Moving", true);
            Vector3 toPlayer = playerBody.position - enemyBody.position;
            toPlayer.y = 0; //Ignore player's vertical position
            movement = toPlayer.normalized * movementSpeed;
            enemyBody.MovePosition(enemyBody.position + (movement * Time.fixedDeltaTime));
        }
        else {
            //enemy idle movement
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 3f) {
                elapsedTime = 0;
                isIdle = !isIdle;
                if (isIdle) {
                    //If left idle for long enough, it could wander far from its original position
                    idleMovement = new Vector3(Random.Range(-idleMovementRange, idleMovementRange), 0, Random.Range(-idleMovementRange, idleMovementRange));
                    movement = idleMovement.normalized * movementSpeed;
                    animator.SetBool("Moving", true);
                } 
                else {
                    movement = Vector3.zero;
                    animator.SetBool("Moving", false);
                }
            }
            enemyBody.MovePosition(enemyBody.position + (movement * Time.fixedDeltaTime));
        }
        if (movement != Vector3.zero) {
            enemyBody.rotation = Quaternion.LookRotation(movement);
        }
    }

    public void enemyAction(){
        //if off ability cooldown can use ability depending on chance to use that ability
        isAttacking = true;
        Debug.Log("Pouncing!");
        useAbility(0);
        //Since the frame handler doesn't seem to be working, we'll play the animnation a different way, with no hitbox.
        //animator.SetBool("Pounce", true);
        //animator.SetBool("Moving", false);
        postActionCooldown = abilities[0].abilityCooldown;
        
    }

    private void useAbility(int abilityNum){
        handleAttacks(abilities[abilityNum]);
        /*
        //first check what type of ability it is and will do stuff depending on type of ability
        if(abilities[abilityNum].abilityType == "Movement"){
            animator.SetBool("MovementAttack", true);
            checkCollision(abilities[abilityNum].abilityDamage);
            StartCoroutine(waitForAnimation("MovementAttack"));
        }*/
        //have other ability types as else if statments and we can add simple code to deal damage correctly. 
    }

    public void onActiveFrameStart() {
        //have if statements to see which ability to play here
        if(currentAttack == "Pounce"){
            pounceCollider.SetActive(true);
        }
    }
    public void onActiveFrameEnd() {
        state = BunnyState.Idle;
        if(currentAttack == "Pounce"){
            pounceCollider.SetActive(false);
            //activeClip.animator.SetBool("Pounce", false); //I feel like having this here is wrong...
        }
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
    public void onLastFrameEnd(){ //I think called when the animation ends.
        state = BunnyState.Idle;
        activeClip.animator.SetBool("Pounce", false);
        isAttacking = false;
        actionCooldownTimer = postActionCooldown; //Cooldown starts AFTER the action finishes
    }

    void Awake(){
        pounceClip.initialize();
        pounceChecker.initialize(this, pounceClip);

        activeChecker = pounceChecker;
        activeClip = pounceClip;
    }

    public void updateMe(float time){ // yes we need this
        activeChecker.checkFrames();

        if (actionState == ActionState.Inactionable){}
        if (actionState == ActionState.AttackCancelable)
        {
            actionState = ActionState.Inactionable;
        }
    }

    public void handleAttacks(Ability ability){
        int frames = 0; // amount of frames in anim 
        actionState = ActionState.Inactionable;
        state = BunnyState.Attacking;

        currentAttack = ability.abilityName;

        if (currentAttack == "Pounce")
        {
            activeChecker = pounceChecker; //I believe this checks each frame to see if the hitbox should be active
            activeClip = pounceClip; //I believe this is the animation clip, though the variable is private and unassigned
        }
        frames = activeClip.getTotalFrames();
        activeClip.animator.SetBool(ability.abilityName, true); //<- These two statements might be the cause? Would have to read over frame checker doc
        activeClip.animator.Play(activeClip.animatorStateName, 0); //<-
        activeChecker.initCheck();
        activeChecker.checkFrames();
        //All of these seem to match what is in the Golem code
    }
}
