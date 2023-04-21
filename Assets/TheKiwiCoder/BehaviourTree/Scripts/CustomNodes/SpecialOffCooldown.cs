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
        /*Does the context it's using not get updated properly? Does it only reference what
         * the enemy's stats were right at the start?*/
        if (context.enemy.specialCooldownTimer <= 0)
            return State.Success;
        else
            return State.Failure;
    }
}
