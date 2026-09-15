using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;
using Pathfinding;

[Serializable, GeneratePropertyBag]
[NodeDescription(
    name: "Attack Target",
    story: "[Object] Attacks [Target]",
    category: "Action",
    id: "c8fde7b71d2e0778b4aba6e236ac5271")]
public partial class AttackTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    [Header("Attack Settings")]
    public float AttackDamage = 15f;
    public float AttackCooldown = 1.2f;
    [Tooltip("Strict distance from NPC center to the closest edge of target collider.")]
    public float AttackRange = 1.4f;

    [Header("Debug Controls")]
    public bool EnableDebugLog = true;

    private UnitAnimator _unitAnimator;
    private Animator _animator;
    private AIPath _aiPath;
    private float _lastAttackTime;

    private enum AttackNodeState { None, ClosingGap, InRangeAttacking }
    private AttackNodeState _currentState = AttackNodeState.None;

    protected override Status OnStart()
    {
        if (Object.Value == null || Target.Value == null)
        {
            if (EnableDebugLog) Debug.LogError("[AttackTarget] Object or Target is missing on the Blackboard.");
            return Status.Failure;
        }

        _unitAnimator = Object.Value.GetComponent<UnitAnimator>();
        _animator = Object.Value.GetComponent<Animator>();
        _aiPath = Object.Value.GetComponent<AIPath>();

        _lastAttackTime = Time.time - AttackCooldown;
        _currentState = AttackNodeState.None;

        if (EnableDebugLog)
            Debug.Log($"[AttackTarget] Node Started. Source: {Object.Value.name} | Target: {Target.Value.name}");

        return Status.Running;
    }

    protected override Status OnUpdate()
    {
        if (Target.Value == null) return Status.Success;

        Entity targetEntity = Target.Value.GetComponent<Entity>();
        if (targetEntity == null || targetEntity.IsDead) return Status.Success;

        Vector3 myPos = Object.Value.transform.position;

        // Edge-to-Edge Distance Calculation using Collider2D
        Collider2D targetCollider = Target.Value.GetComponent<Collider2D>();
        float edgeDistance;

        if (targetCollider != null)
        {
            Vector2 closestPoint = targetCollider.ClosestPoint(myPos);
            edgeDistance = Vector2.Distance(myPos, closestPoint);
        }
        else
        {
            edgeDistance = Vector3.Distance(myPos, Target.Value.transform.position);
        }

        // Failsafe escape check (Buffer added to edge distance)
        if (edgeDistance > AttackRange + 2.0f)
        {
            if (EnableDebugLog)
                Debug.LogWarning($"[AttackTarget] Target escaped! Edge Distance: {edgeDistance:F2}. Aborting.");

            if (_aiPath != null) _aiPath.isStopped = true;
            return Status.Failure;
        }

        SetAttackDirection();

        // CASE 1: Outside precise weapon range -> Move closer to the surface
        if (edgeDistance > AttackRange)
        {
            if (_currentState != AttackNodeState.ClosingGap)
            {
                _currentState = AttackNodeState.ClosingGap;
                if (EnableDebugLog)
                    Debug.Log($"[AttackTarget] State -> ClosingGap. Edge distance: {edgeDistance:F2} (Max range: {AttackRange})");
            }

            if (_aiPath != null)
            {
                _aiPath.isStopped = false;
                // Pathfind directly to the target transform pivot (A* handles collision boundaries automatically)
                _aiPath.destination = Target.Value.transform.position;
            }
        }
        // CASE 2: Inside weapon range -> Stop moving and hit
        else
        {
            if (_currentState != AttackNodeState.InRangeAttacking)
            {
                _currentState = AttackNodeState.InRangeAttacking;
                if (EnableDebugLog)
                    Debug.Log($"[AttackTarget] State -> InRangeAttacking. Distance to edge: {edgeDistance:F2}");
            }

            if (_aiPath != null && !_aiPath.isStopped)
            {
                _aiPath.isStopped = true;
            }

            if (Time.time - _lastAttackTime >= AttackCooldown)
            {
                if (EnableDebugLog)
                    Debug.Log($"[AttackTarget] Hit Triggered! Inflicted {AttackDamage} damage.");

                _unitAnimator?.TriggerAttack();
                targetEntity.TakeDamage(AttackDamage);
                _lastAttackTime = Time.time;
            }
        }

        return Status.Running;
    }

    protected override void OnEnd()
    {
        if (_aiPath != null) _aiPath.isStopped = true;
        _currentState = AttackNodeState.None;
    }

    private void SetAttackDirection()
    {
        if (_animator == null || Target.Value == null) return;
        Vector2 dir = ((Vector2)(Target.Value.transform.position - Object.Value.transform.position)).normalized;
        _animator.SetFloat("DirX", dir.x);
        _animator.SetFloat("DirY", dir.y);
    }
}