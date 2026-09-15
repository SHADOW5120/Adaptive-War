//using System;
//using Unity.Behavior;
//using UnityEngine;
//using Action = Unity.Behavior.Action;
//using Unity.Properties;
//using Pathfinding;

///// <summary>
///// BT Node: Wait for Duration seconds.
///// Used as Idle fallback when PPO_Action == 0.
///// Also stops AIPath movement while waiting.
///// </summary>
//[Serializable, GeneratePropertyBag]
//[NodeDescription(
//    name: "Wait",
//    story: "Wait for [Duration] seconds",
//    category: "Action/Delay",
//    id: "11d0058e94cfe4e16aa7940f033ad522")]
//public partial class WaitAction : Action
//{
//    [SerializeReference] public float Duration = 0.1f;

//    private float _startTime;
//    private AIPath _aiPath;

//    protected override Status OnStart()
//    {
//        _startTime = Time.time;

//        // Stop movement while idling
//        // Object reference not available in WaitAction — get via GameObject.Find is bad
//        // Instead rely on MoveToTargetAction.OnEnd() having stopped AIPath already
//        return Status.Running;
//    }

//    protected override Status OnUpdate()
//    {
//        if (Time.time - _startTime >= Duration)
//            return Status.Success;

//        return Status.Running;
//    }

//    protected override void OnEnd() { }
//}