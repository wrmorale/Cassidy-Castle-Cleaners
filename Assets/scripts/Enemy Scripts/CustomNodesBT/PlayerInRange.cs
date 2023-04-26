using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayerInRange : DecoratorNode
{
    public float range = 0.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        //If behaior trees use Update instead of FixedUpdate, sometimes reports back the incorrect Y value.
        float distance = (context.enemy.playerBody.position - context.rigidbody.position).magnitude;
        if (distance <= range)
        {
            return child.Update();
        }
        else
        {
            return State.Failure;
        }
    }
}
