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

        //Sometimes reports back the incorrect Y value.
        //float distance = (context.enemy.playerBody.position - context.rigidbody.position).magnitude;
        bool playerInRange = context.enemy.playerInRange(range);
        if (playerInRange)
        {
            Debug.Log("Player in range");
            return State.Success;
        }
        else
        {
            Debug.LogWarning("Player out of range");
            return State.Failure;
        }
    }
}
