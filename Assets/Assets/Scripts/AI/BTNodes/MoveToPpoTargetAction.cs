//using System;
//using Unity.Behavior;
//using UnityEngine;
//using Action = Unity.Behavior.Action;
//using Unity.Properties;
//using Pathfinding;

///// <summary>
///// BT Node: Self Moves To PPO_TargetPos
///// Directly controls AIPath.destination each tick.
///// AIDestinationPPO must be REMOVED � this node owns movement.
///// Returns Success when NPC reaches the target position.
///// Returns Failure if NPC gets stuck (optional, not implemented here).
///// </summary>
//[Serializable, GeneratePropertyBag]
//[NodeDescription(name: "Move To PPO Target", story: "[Object] Moves To [Target]", category: "Action", id: "ec2004f3fba9f20f0643a96ca2cc6934")]
//public partial class MoveToPpoTargetAction : Action
//{
//    [SerializeReference] public BlackboardVariable<GameObject> Object;
//    [SerializeReference] public BlackboardVariable<GameObject> Target;
//    public float StopDistance = 0.3f;

//    private AIPath _aiPath;

//    protected override Status OnStart()
//    {
//        if (Object.Value == null) return Status.Failure;

//        _aiPath = Object.Value.GetComponent<AIPath>();
//        if (_aiPath == null)
//        {
//            Debug.LogError("[MoveToTarget] AIPath component not found on " + Object.Value.name);
//            return Status.Failure;
//        }

//        Debug.Log($"[MoveToTarget] OnStart - Starting movement toward: {Target.Value}");
//        _aiPath.isStopped = false;
//        return Status.Running;
//    }

//    protected override Status OnUpdate()
//    {
//        //if (_aiPath == null) return Status.Failure;

//        //_aiPath.destination = Target.Value;

//        //float dist = Vector3.Distance(Object.Value.transform.position, Target.Value);

//        //Debug.Log($"[MoveToTarget] OnUpdate - Target: {Target.Value} | Current Pos: {Object.Value.transform.position} | Remaining Dist: {dist:F2}");

//        //if (dist <= StopDistance)
//        //{
//        //    Debug.Log($"[MoveToTarget] Success - Reached target within StopDistance ({dist:F2} <= {StopDistance})");
//        //    _aiPath.isStopped = true;
//        //    return Status.Success;
//        //}

//        return Status.Running;
//    }

//    protected override void OnEnd()
//    {
//        Debug.Log("[MoveToTarget] OnEnd - Node execution ended.");
//        if (_aiPath != null)
//        {
//            _aiPath.isStopped = true;
//        }
//    }
//}