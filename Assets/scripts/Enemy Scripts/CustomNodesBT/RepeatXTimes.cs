using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TheKiwiCoder;

[System.Serializable]
public class RepeatXTimes : DecoratorNode
{
    public int timesToRepeat = 1;
    int timesRepeated = 0;

    protected override void OnStart() {
        timesRepeated = 0;
    }

    protected override void OnStop() {
        timesRepeated = 0;
    }

    protected override State OnUpdate() {
        switch (child.Update())
        {
            case State.Running:
                break;
            case State.Failure:
                return State.Failure;
            case State.Success:
                timesRepeated += 1;
                if (timesRepeated >= timesToRepeat)
                {
                    return State.Success;
                }
                else
                {
                    break;
                }
        }
        return State.Running;
    }
}
