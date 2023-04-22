using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class PlayerInRangeOfAttack : ActionNode
{
    public string attackName = "";
 
    protected override void OnStart()
    {
    }

    protected override void OnStop()
    {
    }

    protected override State OnUpdate()
    {
        //If behaior trees use Update instead of FixedUpdate, sometimes reports back the incorrect Y value.
        float distance = (context.enemy.playerBody.position - context.rigidbody.position).magnitude;

        AttackData attack = context.attackManager.getAttack(attackName);
        if(attack != null && distance <= attack.range)
        {
            return State.Success;
        }
        else
        {
            //Player not in range to use attack or invalid attack name
            return State.Failure;
        }
    }
}
