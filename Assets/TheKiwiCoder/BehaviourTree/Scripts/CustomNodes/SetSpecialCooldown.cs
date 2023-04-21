using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetSpecialCooldown : ActionNode
{
    public float setTo = 0.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.enemy.specialCooldownTimer = setTo;
        Debug.Log("Set special cooldown to " + setTo);
        return State.Success;
    }
}
