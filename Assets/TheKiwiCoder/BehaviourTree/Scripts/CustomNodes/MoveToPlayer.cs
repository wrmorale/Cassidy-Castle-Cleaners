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
        Vector3 toPlayer = context.enemy.player.position - context.rigidbody.position;
        toPlayer.y = 0; //Ignore player's vertical position
        Vector3 movement = toPlayer.normalized * context.enemy.movementSpeed;
        context.rigidbody.MovePosition(context.rigidbody.position + (movement * Time.deltaTime));
        return State.Success;
    }
}
