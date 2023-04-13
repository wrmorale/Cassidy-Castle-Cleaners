using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using Extensions;

public class Golem : Enemy
{
    //Has some kind of dash attack that actually moves it, and otherwise does not move while attacking.

    private GolemAttackManager attackManager;
    private Vector3 directionToPlayer;
    private float distanceToPlayer;
    public bool isDashing;
    public bool light1Complete;

    public enum GolemState{
        Idle,
        Attacking
    }
    public GolemState state = GolemState.Idle;

    private void Awake(){
        isDashing = false;
        light1Complete = false;
        attackManager = gameObject.GetComponent<GolemAttackManager>();
    }

    //Has problem similar to Bunny: The animation moves the model when it really shouldn't
    //

    void Update() {
        if (movement != Vector3.zero) { //If moving
            enemyBody.rotation = Quaternion.LookRotation(movement); //Rotates entire prefab to face this direction
            Quaternion newRotation = Quaternion.LookRotation(movement, enemyBody.transform.up); //Same thing, but includes the up transform?
            newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f); //Takes previous rotation and boils it down to specifically the y rotation
            enemyBody.transform.rotation = Quaternion.Slerp(enemyBody.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            //Slowly rotates the golem towards the direction of its new movement
            //Does this mean that the golem does not follow its movement perfectly? I assume likely no, as it probably just gets a new direction the frame after rotating.
            //It also seems to be able to reach the player pretty well.
        }
        if (state == GolemState.Idle){
            enemyAction();
        }
        if (state == GolemState.Attacking){
            attackManager.updateMe(Time.deltaTime);
        }
        dashCooldownTimer -= Time.deltaTime;
        actionCooldownTimer -= Time.deltaTime;
        abilityCooldownTimer -= Time.deltaTime;
        if(abilityCooldownTimer < 0) {
            abilityCooldownTimer = 0;
        }
    }

    void FixedUpdate() {
        if (movement != Vector3.zero) {
            movement = playerBody.position - enemyBody.position;
            //lookAtPos.y = enemyBody.transform.position.y; // do not rotate the player around x
            Quaternion newRotation = Quaternion.LookRotation(movement, enemyBody.transform.up);
            enemyBody.transform.rotation = Quaternion.Slerp(enemyBody.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            // Set x and z rotation to zero
            newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f);
            enemyBody.transform.rotation = Quaternion.Slerp(enemyBody.transform.rotation, newRotation, Time.deltaTime * rotationSpeed);
            /*Why are there 3 different rotations here? Is it rotating more quickly than it should?*/
        }
        if (isDashing) {
            // move enemy towards player during dash animation
            directionToPlayer = playerBody.position - enemyBody.position;
            movement = directionToPlayer.normalized * (movementSpeed * 8) * Time.fixedDeltaTime;
            enemyBody.MovePosition(enemyBody.position + movement);
        }
        if (state == GolemState.Idle){
            enemyMovement();
        } 
    }

    private void enemyMovement() {
        directionToPlayer = playerBody.position - enemyBody.position;
        // if player is in range
        if(playerInRange(movementRange)) {
            animator.SetBool("Walk", true);
            // move enemy towards player
            movement = directionToPlayer.normalized * movementSpeed * Time.fixedDeltaTime;
            enemyBody.MovePosition(enemyBody.position + movement);
        }
    }

    /*Called every frame if not attacking*/
    public void enemyAction(){
        directionToPlayer = playerBody.position - enemyBody.position;
        distanceToPlayer = directionToPlayer.magnitude;
        float randomNumber = Random.Range(0, 100);
        //first checks if it can dash to player
        if (distanceToPlayer <= abilities[3].abilityRange && distanceToPlayer > abilities[0].abilityRange && dashCooldownTimer <= 0){
            animator.SetBool("Walk", false);
            state = GolemState.Attacking;
            isDashing = true;
            attackManager.handleAttacks(abilities[3]);
            dashCooldownTimer = abilities[3].abilityCooldown;
        } //this is for spin
        else if (distanceToPlayer <= abilities[0].abilityRange && randomNumber < abilities[0].abilityChance && actionCooldownTimer <= 0){
            animator.SetBool("Walk", false);
            state = GolemState.Attacking;
            attackManager.handleAttacks(abilities[0]);
            actionCooldownTimer = abilities[0].abilityCooldown;
        } //light 1
        else if (distanceToPlayer <= abilities[1].abilityRange && actionCooldownTimer <= 0){
            animator.SetBool("Walk", false);
            state = GolemState.Attacking;
            attackManager.handleAttacks(abilities[1]);
            actionCooldownTimer = abilities[1].abilityCooldown;
        } //light 2 if light 1 went before
        if (light1Complete && distanceToPlayer <= abilities[2].abilityRange){
            animator.SetBool("Walk", false);
            state = GolemState.Attacking;
            actionCooldownTimer = abilities[2].abilityCooldown;
            attackManager.handleAttacks(abilities[2]);
        }
    }
}