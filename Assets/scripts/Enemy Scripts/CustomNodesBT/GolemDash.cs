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
    Vector3 moveDirection;
    float startTime;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
        endOn = Time.time + duration;
        moveDirection = blackboard.moveToPosition - context.transform.position;
        moveDirection.y = 0;
        startTime = Time.time;
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        //dashSpeed = Mathf.Lerp(dashSpeed, 0, (Time.time - startTime)/duration);
        context.enemy.movement += moveDirection.normalized * dashSpeed;

        if (Time.time >= endOn)
        {
            context.enemy.movement = Vector3.zero;
            return State.Success;
        }
        else
            return State.Running;
    }
}
