using Core;
using Pathfinding;
using UnityEngine;

/// <summary>
/// Replaces "AI Destination Setter" component.
/// 
/// Logic:
///   - No enemy / outside engageRange  -> chase defaultTarget directly (e.g. MainBuilding)
///   - Inside engageRange, action 0    -> stop (Idle)
///   - Inside engageRange, action 1-8  -> tactical step to TargetMovePosition
///   - Inside engageRange, action 9    -> move toward enemy so AttackTargetAction can hit
///
/// Setup: Remove "AI Destination Setter", add this component.
///        Assign defaultTarget in Inspector.
/// </summary>
[RequireComponent(typeof(AIPath))]
[RequireComponent(typeof(TacticalBrain))]
public class AIDestinationPPO : MonoBehaviour
{
    [Header("Default target when not in combat (e.g. MainBuilding or Player)")]
    public Transform defaultTarget;

    [Tooltip("Distance at which PPO takes over from direct chase")]
    public float combatEngageRange = 6f;

    [Header("Debug")]
    public bool showGizmos = true;

    [SerializeField, Space] private string _mode = "Chase";

    private AIPath _aiPath;
    private TacticalBrain _brain;

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _brain = GetComponent<TacticalBrain>();
        _aiPath.enableRotation = false; // Animator Blend Tree handles facing direction
    }

    private void Update()
    {
        if (defaultTarget == null) return;

        bool hasEnemy = _brain.enemyEntity != null && !_brain.enemyEntity.IsDead;
        float distToDefault = Vector2.Distance(transform.position, defaultTarget.position);

        if (!hasEnemy || distToDefault > combatEngageRange)
        {
            // Chase mode: move straight toward defaultTarget
            _mode = $"Chase -> {defaultTarget.name}";
            _aiPath.destination = defaultTarget.position;
            _aiPath.isStopped = false;
        }
        else
        {
            // PPO tactical mode
            int action = _brain.CurrentAction;

            if (action == 0)
            {
                _mode = "PPO Idle";
                _aiPath.isStopped = true;
            }
            else if (action >= 1 && action <= 8)
            {
                _mode = $"PPO Move [{DirName(action)}]";
                _aiPath.destination = _brain.TargetMovePosition;
                _aiPath.isStopped = false;
            }
            else if (action == 9)
            {
                // Move close enough to enemy to attack
                _mode = "PPO Attack -> closing in";
                _aiPath.destination = _brain.enemyEntity.transform.position;
                _aiPath.isStopped = false;
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, combatEngageRange);
        if (_aiPath != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, _aiPath.destination);
            Gizmos.DrawSphere(_aiPath.destination, 0.15f);
        }
    }

    private static string DirName(int a) => a switch
    {
        1 => "N",
        2 => "NE",
        3 => "E",
        4 => "SE",
        5 => "S",
        6 => "SW",
        7 => "W",
        8 => "NW",
        _ => "?"
    };
}