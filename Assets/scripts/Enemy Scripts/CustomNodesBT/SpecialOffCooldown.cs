using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SpecialOffCooldown : ActionNode
{
    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //Check if we have passed the time when the special goes off cooldown
        if (blackboard.specialCooldownEnds <= Time.time)
            return State.Success;
        else
            return State.Failure;
    }
}
