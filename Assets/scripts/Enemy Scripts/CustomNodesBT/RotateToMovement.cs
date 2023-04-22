using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RotateToMovement : ActionNode
{
    public enum RotateOption
    {
        Player,
        Movement
    }
    public RotateOption rotateTo = RotateOption.Movement;
    public float speedMultiplier = 1.0f;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if(rotateTo == RotateOption.Movement)
        {
            Quaternion newRotation = Quaternion.LookRotation(context.enemy.movement, context.rigidbody.transform.up);
            // Set x and z rotation to zero
            newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f);
            context.rigidbody.transform.rotation = Quaternion.Slerp(context.rigidbody.transform.rotation, newRotation, Time.fixedDeltaTime * context.enemy.rotationSpeed * speedMultiplier);
            //Some of these stats, like rotationSpeed, could maybe be moved to the blackboard.
        }
        else if(rotateTo == RotateOption.Player)
        {
            Vector3 toPlayer = context.enemy.playerBody.position - context.rigidbody.position;
            toPlayer.y = 0;
            Quaternion newRotation = Quaternion.LookRotation(toPlayer, context.rigidbody.transform.up);
            // Set x and z rotation to zero
            newRotation = Quaternion.Euler(0f, newRotation.eulerAngles.y, 0f);
            context.rigidbody.transform.rotation = Quaternion.Slerp(context.rigidbody.transform.rotation, newRotation, Time.fixedDeltaTime * context.enemy.rotationSpeed * speedMultiplier);
        }

        return State.Success;
    }
}
