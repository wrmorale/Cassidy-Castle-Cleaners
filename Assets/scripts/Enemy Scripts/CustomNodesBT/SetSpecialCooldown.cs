using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetSpecialCooldown : ActionNode
{
    /*This node sets a time x seconds in the future, when our special will go "off cooldown"*/
    public float xSecondsInFuture = 0.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        blackboard.specialCooldownEnds = Time.time + xSecondsInFuture;
        Debug.Log("Special will go off cooldown at " + blackboard.specialCooldownEnds);
        return State.Success;
    }
}
