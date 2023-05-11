using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class DoRangedAttack : ActionNode
{
    public string attackName = "";
    AttackData attack;

    /*Called the first time this node is ran on a tick.
    If a node returns RUNNING it will not call OnStart()
    on the next tick*/
    protected override void OnStart() {
        attack = context.attackManager.getAttack(attackName);
        if (attack == null)
            return;

        Vector3 heading = (context.enemy.playerBody.position - context.transform.position).normalized;  
        Projectile clone = UnityEngine.Object.Instantiate(context.moth.projectilePrefab, context.moth.bulletSpawn.position, Quaternion.LookRotation(heading));
        clone.gameObject.SetActive(true);
        clone.Initialize(context.moth.projectileSpeed, context.moth.projectileLifetime, context.moth.projectileDamage, 1f, heading);

        
    }

    /*Called after OnUpdate if the node returns SUCCESS or 
    FAILURE. Also called if the current behavior using the
    node function Abort()*/
    protected override void OnStop() {
    }

    /*Called every tick that this node is executed*/
    protected override State OnUpdate() {
        if (attack == null)
        {
            Debug.LogError("Invalid attack name in Node DoAttack");
            return State.Failure;
        }
        if (context.animator.GetBool(attackName)) {
            return State.Running; //Attack animation is still going
        }
        else
        {
            return State.Success; //Attack animation has finished
        }

    }
}
