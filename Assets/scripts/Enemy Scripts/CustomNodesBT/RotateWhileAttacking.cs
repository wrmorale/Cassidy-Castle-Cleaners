using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RotateWhileAttacking : ActionNode
{
    public float speedMultiplier = 1.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (context.attackManager.attacking)
        {
            Vector3 toPlayer = context.enemy.playerBody.position - context.transform.position;
            toPlayer.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(toPlayer, context.transform.up);
            // Set x and z rotation to zero
            newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f);
            context.transform.rotation = Quaternion.Slerp(context.transform.rotation, newRotation, Time.fixedDeltaTime * context.enemy.rotationSpeed * speedMultiplier);
            return State.Running;
        }
        else
        {
            return State.Success;
        }


    }
}
