using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class DoAttack : ActionNode
{
    public string attackName = "";
    AttackData attack;

    protected override void OnStart() {
        attack = context.attackManager.getAttack(attackName);
        if (attack == null)
            return;
        context.attackManager.handleAttacks(attackName); //Start the attack animation
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        if (attack == null)
        {
            Debug.LogError("Invalid attack name in Node DoAttack");
            return State.Failure;
        }

        //Attack manager sets a bool with a matching attack name to true, then sets it to false when the attack is finished.
        //So we can use that to tell when the attack is finsihed.
        //Not sure if best way to keep track of that, might change later
        /*Update: Not sure why but Dash's OnLastFrame is called earlier than it's supposed to*/
        if (context.animator.GetBool(attackName)) {
            return State.Running; //Attack animation is still going
        }
        else
        {
            return State.Success; //Attack animation has finished
        }
    }
}
