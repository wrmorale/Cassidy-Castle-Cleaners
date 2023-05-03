using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class LookingAtPlayer : ActionNode
{
    /*Checks if the enemy is rotated towards the player, within degreesOfRange
     IE degreesOfRange = 0.0 means the enemy has to be facing the player exactly*/
    public float degreesOfRange = 10.0f;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        Vector3 toPlayer = context.enemy.playerBody.position - context.transform.position;
        toPlayer.y = 0;
        Quaternion newRotation = Quaternion.LookRotation(toPlayer, context.transform.up);
        float difference = context.transform.rotation.eulerAngles.y - newRotation.eulerAngles.y;

        if (Mathf.Abs(difference) <= degreesOfRange)
            return State.Success;
        else
            return State.Failure;
    }
}
