using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Pathfinding;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Move To PPO Target",
    story: "[Object] Moves To [BrainPos]",
    category: "Action",
    id: "9c548e628b10a90f5bb9fc3d8cc4916a")]
public partial class MoveToPpoTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<Vector3> BrainPos;

    [Tooltip("How close the agent needs to get to the BrainPos coordinate to consider it reached (e.g., 0.2)")]
    public float PathingTolerance = 0.2f;

    [Header("Debug Controls")]
    public bool EnableDebugLog = false; // Turned off by default to prevent spam

    private AIPath _aiPath;

    protected override Status OnStart()
    {
        if (Object.Value == null) return Status.Failure;
        if (BrainPos == null) return Status.Failure;

        _aiPath = Object.Value.GetComponent<AIPath>();
        if (_aiPath == null)
        {
            if (EnableDebugLog) Debug.LogError("[MoveToPpoTarget] AIPath not found on " + Object.Value.name);
            return Status.Failure;
        }

        _aiPath.isStopped = false;

        // Immediately set the destination to what the Brain requested
        _aiPath.destination = BrainPos.Value;

        if (EnableDebugLog) Debug.Log($"[MoveToPpoTarget] Started. Navigating to BrainPos: {BrainPos.Value}");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (_aiPath == null) return Status.Failure;

        // Keep updating the destination in case the Brain updates BrainPos dynamically 
        // (if BrainEvaluates runs in parallel)
        _aiPath.destination = BrainPos.Value;

        // Calculate distance to the BRAIN'S REQUESTED POSITION, *NOT* the enemy
        float distToBrainPos = Vector3.Distance(Object.Value.transform.position, BrainPos.Value);

        if (EnableDebugLog) Debug.Log($"[MoveToPpoTarget] distToBrainPos={distToBrainPos:F2}");

        // If we reached the coordinate the PPO model asked for, return Success.
        if (distToBrainPos <= PathingTolerance)
        {
            _aiPath.isStopped = true;
            if (EnableDebugLog) Debug.Log($"[MoveToPpoTarget] Reached Brain Target Coord. Returning Success.");
            return Status.Success;
        }

        // Failsafe: If A* cannot reach the point (path is pending/invalid but it's not moving)
        if (!_aiPath.pathPending && (_aiPath.reachedEndOfPath || !_aiPath.hasPath))
        {
            // We give it a small buffer distance to allow A* to start calculating
            if (distToBrainPos > PathingTolerance + 0.5f)
            {
                if (EnableDebugLog) Debug.LogWarning("[MoveToPpoTarget] A* Path finished or failed, but target not reached. Forcing Success to allow Brain re-evaluation.");
                _aiPath.isStopped = true;
                return Status.Success;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_aiPath != null) _aiPath.isStopped = true;
    }
}