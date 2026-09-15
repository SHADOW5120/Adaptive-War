using Core;
using Entities.Units;
using Pathfinding;
using UnityEngine;

/// <summary>
/// Handles two responsibilities only:
///   1. Sync UnitState (Walk / Idle / Die)
///   2. Update DirX and DirY on the Animator from AIPath velocity
///      so the 8-directional Blend Tree shows the correct walk sprite.
///
/// Does NOT set AIPath.destination — AIDestinationPPO handles that.
/// </summary>
public class UnitMovement : MonoBehaviour
{
    private AIPath _aiPath;
    private Unit _unit;
    private Animator _animator;

    public Vector2 CurrentDirection { get; private set; } = Vector2.down;
    public bool IsMoving => _aiPath != null &&
                                       _aiPath.velocity.sqrMagnitude > 0.01f;

    private void Awake()
    {
        _aiPath = GetComponent<AIPath>();
        _unit = GetComponent<Unit>();
        _animator = GetComponent<Animator>();

        // Disable A* auto-rotation; Blend Tree handles visual direction
        if (_aiPath != null)
            _aiPath.enableRotation = false;
    }

    private void Update()
    {
        if (_unit != null && _unit.CurrentState == UnitState.Die)
        {
            if (_aiPath != null) _aiPath.isStopped = true;
            return;
        }

        SyncAnimator();
    }

    /// <summary>Force idle state — called by AttackTargetAction on exit.</summary>
    public void SetIdle()
    {
        if (_unit != null && _unit.CurrentState != UnitState.Die)
            _unit.CurrentState = UnitState.Idle;
        if (_animator != null)
            _animator.SetBool("isWalk", false);
    }

    private void SyncAnimator()
    {
        if (_animator == null) return;

        bool moving = IsMoving;
        _animator.SetBool("isWalk", moving);

        if (!moving) return;

        Vector2 vel = ((Vector2)_aiPath.velocity).normalized;
        if (vel.sqrMagnitude < 0.001f) return;

        CurrentDirection = vel;
        _animator.SetFloat("DirX", vel.x);
        _animator.SetFloat("DirY", vel.y);
    }
}