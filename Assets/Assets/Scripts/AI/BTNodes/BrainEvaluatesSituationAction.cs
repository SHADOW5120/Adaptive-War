//using System;
//using Unity.Behavior;
//using UnityEngine;
//using Action = Unity.Behavior.Action;
//using Unity.Properties;

///// <summary>
///// BT Node: Brain Evaluates Situation
/////
///// Runs PPO inference and writes results to Blackboard.
///// BT Branch nodes read PPO_Action to decide which path to take:
/////   9 ? AttackTargetAction
/////   0 ? WaitAction
/////   1-8 ? MoveToTargetAction
/////
///// AIDestinationPPO is NOT used — BT nodes own all movement.
///// </summary>
//[Serializable, GeneratePropertyBag]
//[NodeDescription(
//    name: "Brain Evaluates Situation",
//    story: "[Brain] Evaluates Situation And Target [Enemy]",
//    category: "Action",
//    id: "369a9a8261c3a89b3338513052f74639")]
//public partial class BrainEvaluatesSituationAction : Action
//{
//    [SerializeReference] public BlackboardVariable<TacticalBrain> Brain;
//    [SerializeReference] public BlackboardVariable<GameObject> Enemy;
//    [SerializeReference] public BlackboardVariable<int> PPO_Action;
//    [SerializeReference] public BlackboardVariable<Vector3> PPO_TargetPos;

//    protected override Status OnStart()
//    {
//        if (Brain == null || Brain.Value == null)
//        {
//            Debug.LogError("[BrainEvaluates] Brain is null — check Blackboard.");
//            return Status.Failure;
//        }
//        if (Enemy == null || Enemy.Value == null)
//        {
//            Debug.LogError("[BrainEvaluates] Enemy is null — FindNearest must run first.");
//            return Status.Failure;
//        }
//        if (PPO_Action == null || PPO_Action.ObjectValue == null)
//        {
//            Debug.LogError("[BrainEvaluates] PPO_Action variable is not bound in the node inspector.");
//            return Status.Failure;
//        }
//        if (PPO_TargetPos == null || PPO_TargetPos.ObjectValue == null)
//        {
//            Debug.LogError("[BrainEvaluates] PPO_TargetPos variable is not bound in the node inspector.");
//            return Status.Failure;
//        }

//        Debug.Log("[BrainEvaluates] OnStart - Evaluation initialized successfully.");
//        return Status.Running;
//    }

//    protected override Status OnUpdate()
//    {
//        // Enemy destroyed or dead ? restart BT to find new target
//        if (Enemy.Value == null) return Status.Failure;

//        Entity enemy = Enemy.Value.GetComponent<Entity>();
//        if (enemy != null && enemy.IsDead) return Status.Failure;

//        // Inject current enemy into brain every tick
//        if (enemy != null)
//            Brain.Value.enemyEntity = enemy;

//        // Run inference (TacticalBrain self-throttles via evalCooldown)
//        Brain.Value.EvaluateTacticalSituation();

//        // Check internal brain action before writing
//        int rawBrainAction = Brain.Value.CurrentAction;

//        // Write to Blackboard — Branch nodes read these
//        PPO_Action.Value = rawBrainAction;
//        PPO_TargetPos.Value = Brain.Value.TargetMovePosition;

//        // Debug log to verify value passing and sync
//        Debug.Log($"[BrainEvaluates] OnUpdate - Brain.CurrentAction: {rawBrainAction} | Written Blackboard PPO_Action: {PPO_Action.Value} | TargetPos: {PPO_TargetPos.Value}");

//        // Return Success ? BT proceeds to Branch in same tick
//        return Status.Success;
//    }

//    protected override void OnEnd()
//    {
//        Debug.Log("[BrainEvaluates] OnEnd - Evaluation node completed its tick.");
//    }
//}