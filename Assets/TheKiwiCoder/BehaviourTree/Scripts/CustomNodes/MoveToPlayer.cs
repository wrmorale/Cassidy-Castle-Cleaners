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
        Vector3 toPlayer = context.enemy.playerBody.position - context.rigidbody.position;
        //Maybe the context.player position does not update? But then it wouldn't make sense why the bug happens when I get near him
        toPlayer.y = 0; //Ignore player's vertical position
        Vector3 movement = toPlayer.normalized * context.enemy.movementSpeed * Time.fixedDeltaTime;
        context.rigidbody.MovePosition(context.rigidbody.position + (movement));
        context.enemy.movement = movement; //Updates enemy movement stat for other uses.
        return State.Success;
    }
}
