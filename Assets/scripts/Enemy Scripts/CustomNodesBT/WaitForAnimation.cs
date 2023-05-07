using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class WaitForAnimation : ActionNode
{
    //Waits for an animatorState to complete, returning Running until that happens.
    //NOTE: This only works for animations that complete and proceed to the next state automatically
    //like via a set exit time
    public string animatorStateName = "";
    float timeOnWrongAnim = 0.0f;
    bool checkingForNextState;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
        timeOnWrongAnim = Time.time;
        checkingForNextState = false;
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        AnimatorStateInfo currState = context.animator.GetCurrentAnimatorStateInfo(0);
        if(checkingForNextState && !currState.IsName(animatorStateName)){
            //Animator has poceeded to the next state
            return State.Success;
        }

        if (!checkingForNextState)
        {
            if (currState.IsName(animatorStateName))
            {
                //Animator has started playing or is playing the animation we are waiting for
                //(Have to do this because this node is called slightly before the animator transitions)
                checkingForNextState = true;
            }
            else if (Time.time >= timeOnWrongAnim + 2.0f)
            {
                Debug.LogError("WaitForAnimation on wrong animation for more than 2 seconds!");
                return State.Success;
            }
        }
        //Could add in a failestate where if it is on the incorrect animation state for too long, it will return failure.
        return State.Running;
    }
}
