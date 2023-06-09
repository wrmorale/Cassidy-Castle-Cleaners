using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

//NOTE: This node assumes the enemy is grounded and has gravity
//Can later try to add pathfinding

[System.Serializable]
public class MoveToPlayer : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //PlayerBody.position - position of Rigidbody of Enemy
        //I wonder if there is a difference between transform and charController.transform?
        //TESTED: They are the same
        Vector3 toPlayer = context.enemy.playerBody.position - context.transform.position;
        toPlayer.y = 0; //Ignore player's vertical position
        context.enemy.movement += toPlayer.normalized * context.enemy.movementSpeed;
        return State.Success;
    }
}
