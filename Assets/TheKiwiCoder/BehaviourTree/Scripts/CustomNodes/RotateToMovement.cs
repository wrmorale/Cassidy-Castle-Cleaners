using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RotateToMovement : ActionNode
{


    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {

        Quaternion newRotation = Quaternion.LookRotation(context.enemy.movement, context.rigidbody.transform.up);
        // Set x and z rotation to zero
        newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f);
        context.rigidbody.transform.rotation = Quaternion.Slerp(context.rigidbody.transform.rotation, newRotation, Time.fixedDeltaTime * context.enemy.rotationSpeed);
        //Some of these stats, like rotationSpeed, could maybe be moved to the blackboard.

        return State.Success;
    }
}
