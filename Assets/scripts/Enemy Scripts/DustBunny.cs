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

    enum ActionState {Inactionable, AttackCancelable}
    private ActionState actionState;

    private float lastRootY;
    private GameObject model;
    private GameObject metarig;
    private GameObject hip;

    public enum BunnyState{
        Idle,
        Attacking
    }
    public BunnyState state = BunnyState.Idle;

    void FixedUpdate() {
        enemyMovement();
    }

    void Update() {
        if (state == BunnyState.Idle){
            enemyAttack();
        }
        if (state == BunnyState.Attacking){
            updateMe(Time.deltaTime);
        }
    }

    private void enemyMovement() {
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);
        // MoveRoot();
        // if player is in range
        if(Vector3.Distance(enemyBody.position, playerBody.position) < movementRange) {
            // move enemy towards player
            if (stateInfo.normalizedTime >= 1f){
                animator.SetBool("Moving", true);
                MoveRoot();
                movement = (playerBody.position - enemyBody.position) * movementSpeed;
                enemyBody.MovePosition(enemyBody.position + (movement * Time.fixedDeltaTime));
            }
        }
        else {
            //enemy idle movement
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= 3f) {
                elapsedTime = 0;
                isIdle = !isIdle;
                if (isIdle) {
                    idleMovement = enemyBody.position + new Vector3(Random.Range(-idleMovementRange, idleMovementRange), 0, Random.Range(-idleMovementRange, idleMovementRange));
                    movement = (idleMovement - enemyBody.position).normalized * movementSpeed;
                    animator.SetBool("Moving", true);
                    MoveRoot();
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

    public void enemyAttack(){
        //if able to attack, the enemy does so
        //first checks to see if enemy is in range for an attack
        if(Vector2.Distance(enemyBody.position, playerBody.position) < longestAttackRange && actionCooldownTimer <= 0) {
            enemyAction();
        }
        actionCooldownTimer -= Time.deltaTime;
        abilityCooldownTimer -= Time.deltaTime;
        if(abilityCooldownTimer < 0) {
            abilityCooldownTimer = 0;
        }
    }

    public void enemyAction(){
        //if off ability cooldown can use ability depending on chance to use that ability
        if(abilityCooldownTimer == 0){
            abilityCounter = 0;
            foreach (Ability ability in abilities) {
                //before checking if an ability can be cast check if the player is in ability range
                if(Vector2.Distance(enemyBody.position, playerBody.position) < ability.abilityRange){
                    float randomNumber = Random.Range(0, 100);
                    if (randomNumber < ability.abilityChance) {
                        useAbility(abilityCounter);
                        actionCooldownTimer = (1 / basicAttackSpeed);
                        break;
                    }
                    abilityCounter++;
                }
            }
        }
    }

    private void useAbility(int abilityNum){
        abilityCooldownTimer = abilities[abilityNum].abilityCooldown;
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
            activeClip.animator.SetBool("Pounce", false);
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
    public void onLastFrameEnd(){
        state = BunnyState.Idle;
        activeClip.animator.SetBool("Pounce", false);
    }

    void Awake()
    {
        pounceClip.initialize();
        pounceChecker.initialize(this, pounceClip);

        activeChecker = pounceChecker;
        activeClip = pounceClip;

        model = transform.Find("DustBunnyModel10").gameObject;
        metarig = transform.Find("DustBunnyModel10/metarig").gameObject;
        hip = transform.Find("DustBunnyModel10/metarig/hip").gameObject;
        lastRootY = hip.transform.localPosition.y;
    }

    public void updateMe(float time) // yes we need this
    {
        activeChecker.checkFrames();

        if (actionState == ActionState.Inactionable){}
        if (actionState == ActionState.AttackCancelable)
        {
            actionState = ActionState.Inactionable;
        }
    }
    public void handleAttacks(Ability ability)
    {
        int frames = 0; // amount of frames in anim 
        actionState = ActionState.Inactionable;
        state = BunnyState.Attacking;

        currentAttack = ability.abilityName;

        if (currentAttack == "Pounce")
        {
            activeChecker = pounceChecker;
            activeClip = pounceClip;
        }
        frames = activeClip.getTotalFrames();
        activeClip.animator.SetBool(ability.abilityName, true);
        activeClip.animator.Play(activeClip.animatorStateName, 0);
        activeChecker.initCheck();
        activeChecker.checkFrames();
    }
    public void MoveRoot()
    {
        if (lastRootY != hip.transform.localPosition.y)
        {
            Debug.Log("moveroot");
            float diff = lastRootY - hip.transform.localPosition.y;
            enemyBody.MovePosition(
                transform.forward * diff * metarig.transform.localScale.y * transform.localScale.z
            );
            model.transform.localPosition =
                model.transform.localPosition
                + (Vector3.forward * -diff * metarig.transform.localScale.y);
        }
        lastRootY = hip.transform.localPosition.y;
    }

}
