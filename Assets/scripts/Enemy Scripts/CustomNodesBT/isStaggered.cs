using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

public class StaggerCheckNode : ActionNode
{
    private Enemy enemy;

    public override void OnStart()
    {
        enemy = context as Enemy;
    }

    public override void OnStop()
    {
        // Reset the isStaggered flag when the behavior is interrupted
        enemy.isStaggered = false;
    }

    public override State OnUpdate()
    {
        if (enemy.isStaggered)
        {
            // Interrupt the current running behavior and execute the Stagger behavior
            return State.Interrupt;
        }
        else
        {
            // Continue with the current running behavior
            return State.Success;
        }
    }
}