using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using States;
using Extensions;

public class Golem : Enemy
{
    //Has some kind of dash attack that actually moves it, and otherwise does not move while attacking.

    
    private AttackManager attackManager;
    private Vector3 directionToPlayer;
    private float distanceToPlayer;
    [Header("Golem Stuff")]
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
        attackManager = gameObject.GetComponent<AttackManager>();
    }

    //Has problem similar to Bunny: The animation moves the model when it really shouldn't
    //

    
    

    //private void enemyMovement() {
    //    directionToPlayer = playerBody.position - enemyBody.position;
    //    // if player is in range
    //    if(playerInRange(movementRange)) { //If every enemy had the Walk bool, we could just make a node that calls this in every enemy script.
    //        //But I have a feeling we will have enemies with slightly different naming conventions...
    //        animator.SetBool("Walk", true);
    //        // move enemy towards player
    //        /*Fix this: Can potentially make the golem face in weird directions*/
    //        movement = directionToPlayer.normalized * movementSpeed * Time.fixedDeltaTime;
    //        enemyBody.MovePosition(enemyBody.position + movement);
    //    }
    //}

    /*Called every frame if not attacking*/
    //public void enemyAction(){
    //    directionToPlayer = playerBody.position - enemyBody.position;
    //    distanceToPlayer = directionToPlayer.magnitude;
    //    float randomNumber = Random.Range(0, 100);
    //    //Spin
    //    if (distanceToPlayer <= abilities[0].abilityRange && randomNumber < abilities[0].abilityChance && actionCooldownTimer <= 0){
    //        animator.SetBool("Walk", false); //Can move this into a state machine node, putting animator as member of Context
    //        state = GolemState.Attacking; //This too
    //        attackManager.handleAttacks(abilities[0]); //And this, adding attackManager to Context
    //        actionCooldownTimer = abilities[0].abilityCooldown; //Same here, although probably want to make it a different node
    //        //And should go after the attack finishes, not when it starts
    //    } //light 1
    //    else if (distanceToPlayer <= abilities[1].abilityRange && actionCooldownTimer <= 0){
    //        animator.SetBool("Walk", false);
    //        state = GolemState.Attacking;
    //        attackManager.handleAttacks(abilities[1]);
    //        actionCooldownTimer = abilities[1].abilityCooldown;
    //    } //light 2 if light 1 went before
    //    if (light1Complete && distanceToPlayer <= abilities[2].abilityRange){
    //        animator.SetBool("Walk", false);
    //        state = GolemState.Attacking;
    //        actionCooldownTimer = abilities[2].abilityCooldown;
    //        attackManager.handleAttacks(abilities[2]);
    //    }
    //}
}