using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class WaitForAnimation : ActionNode
{
    //Waits for an animatorState to complete, returning Running until that happens.
    public string animatorStateName = "";
    float timeOnWrongAnim = 0.0f;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
        timeOnWrongAnim = Time.time;
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        AnimatorStateInfo currState = context.animator.GetCurrentAnimatorStateInfo(0);
        if (currState.IsName(animatorStateName))
        {
            if (currState.normalizedTime >= 0.1f)
            {
                return State.Success; //The specified animation has completed
            }
        }
        else if (Time.time >= timeOnWrongAnim + 5.0f)
        {
            Debug.LogError("WaitForAnimation on wrong animation for more than 5 seconds!");
            return State.Success;
        }
        //Could add in a failestate where if it is on the incorrect animation state for too long, it will return failure.
        return State.Running;
    }
}
