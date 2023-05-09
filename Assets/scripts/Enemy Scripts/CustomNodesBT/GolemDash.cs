using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class GolemDash : ActionNode
{
    /*Get's player's current position as a target and moves towards it
     for X seconds*/

    public float duration = 1.0f;
    public float dashSpeed = 5.0f;
    float endOn = 0.0f;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
        endOn = Time.time + duration;
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        Vector3 moveDirection = blackboard.moveToPosition - context.transform.position;
        context.enemy.movement += moveDirection.normalized * dashSpeed;

        if (Time.time >= endOn)
        {
            //context.rigidbody.velocity = Vector3.zero; //Wouldn't need this if it was a character controller
            return State.Success;
        }
        else
            return State.Running;
    }
}
