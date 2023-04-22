using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class SetAnimatorBool : ActionNode
{
    public string boolName = "";
    public bool setTo = true;

    protected override void OnStart() {
    }

    protected override void OnStop() {
    }

    protected override State OnUpdate() {
        context.animator.SetBool(boolName, setTo);
        return State.Success;
    }
}
