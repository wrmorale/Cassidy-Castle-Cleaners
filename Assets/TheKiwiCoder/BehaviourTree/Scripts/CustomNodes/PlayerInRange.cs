using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayerInRange : ActionNode
{
    public float range = 0.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        float distance = (context.enemy.player.position - context.transform.position).magnitude;
        if (distance <= range)
            return State.Success;
        else
            return State.Failure;
    }
}
