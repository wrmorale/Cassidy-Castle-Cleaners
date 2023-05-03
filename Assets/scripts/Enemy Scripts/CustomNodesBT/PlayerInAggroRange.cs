using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayerInAggroRange : DecoratorNode
{
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        //If behaior trees use Update instead of FixedUpdate, sometimes reports back the incorrect Y value.
        float distance = (context.enemy.playerBody.position - context.transform.position).magnitude;
        if (distance <= context.enemy.aggroRange)
        {
            //Debug.Log("Player in aggro range");
            return child.Update();
        }
        else
        {
            //Debug.LogWarning("Player out of aggro range");
            return State.Failure;
        }
    }
}
