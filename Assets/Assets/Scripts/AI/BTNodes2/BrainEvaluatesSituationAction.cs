using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Brain Evaluates Situation",
    story: "[Brain] Evaluates Situation And Target [Enemy] And [BrainPos]",
    category: "Action",
    id: "86fd3faebda867cfafd738e491d827d2")]
public partial class BrainEvaluatesSituationAction : Action
{
    [SerializeReference] public BlackboardVariable<TacticalBrain> Brain;
    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
    [SerializeReference] public BlackboardVariable<Vector3> BrainPos;
    [SerializeReference] public BlackboardVariable<int> PPO_Action;

    [Header("Hybrid Override Settings")]
    [Tooltip("Distance from NPC center to the closest edge of enemy's collider.")]
    public float ForceAttackRange = 1.2f;

    [Header("Debug Controls")]
    public bool EnableDebugLog = true;

    protected override Status OnStart()
    {
        if (Brain == null || Brain.Value == null)
        {
            Debug.LogError("[BrainEvaluates] Brain is null - check Blackboard.");
            return Status.Failure;
        }
        if (Enemy == null || Enemy.Value == null)
        {
            Debug.LogError("[BrainEvaluates] Enemy is null - FindNearest must run first.");
            return Status.Failure;
        }
        if (PPO_Action == null)
        {
            Debug.LogError("[BrainEvaluates] PPO_Action variable is not bound in the node inspector.");
            return Status.Failure;
        }

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Enemy.Value == null) return Status.Failure;

        Entity enemy = Enemy.Value.GetComponent<Entity>();
        if (enemy != null && enemy.IsDead) return Status.Failure;

        if (enemy != null)
            Brain.Value.enemyEntity = enemy;

        // Execute PPO Model Inference
        Brain.Value.EvaluateTacticalSituation();

        int finalAction = Brain.Value.CurrentAction;
        Vector3 npcPos = Brain.Value.transform.position;

        // Edge-to-Edge Distance Calculation using Collider2D
        Collider2D enemyCollider = Enemy.Value.GetComponent<Collider2D>();
        float distanceToEnemySurface;

        if (enemyCollider != null)
        {
            // Find the closest point on the enemy collider surface to the NPC
            Vector2 closestPoint = enemyCollider.ClosestPoint(npcPos);
            distanceToEnemySurface = Vector2.Distance(npcPos, closestPoint);
        }
        else
        {
            // Fallback to center-to-center if no collider is found
            distanceToEnemySurface = Vector3.Distance(npcPos, Enemy.Value.transform.position);
        }

        // Intercept movement if within actual physical combat range
        if (distanceToEnemySurface <= ForceAttackRange && finalAction >= 1 && finalAction <= 8)
        {
            if (EnableDebugLog)
            {
                Debug.Log($"[BrainEvaluates] Override! Edge Dist ({distanceToEnemySurface:F2}) <= ForceAttackRange ({ForceAttackRange}). Forcing Attack (9).");
            }
            finalAction = 9;
        }

        PPO_Action.Value = finalAction;
        BrainPos.Value = Brain.Value.TargetMovePosition;

        return Status.Success;
    }

    protected override void OnEnd() { }
}