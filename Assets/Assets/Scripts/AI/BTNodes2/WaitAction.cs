using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Pathfinding;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Wait", story: "Wait for [Duration] seconds", category: "Action", id: "cd5700638e4cd125477d260a14e8851e")]
public partial class WaitAction : Action
{
    [SerializeReference] public float Duration = 0.1f;

    private float _startTime;
    private AIPath _aiPath;

    protected override Status OnStart()
    {
        _startTime = Time.time;

        // Stop movement while idling
        // Object reference not available in WaitAction — get via GameObject.Find is bad
        // Instead rely on MoveToTargetAction.OnEnd() having stopped AIPath already
        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Time.time - _startTime >= Duration)
            return Status.Success;

        return Status.Running;
    }

    protected override void OnEnd() { }
}

