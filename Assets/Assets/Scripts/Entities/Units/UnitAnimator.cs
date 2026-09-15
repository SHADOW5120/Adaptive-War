using Core;
using Entities.Units;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private Animator _animator;
    private Unit _unit;
    private UnitMovement _movement;

    // X, Y parameters for the 2D Blend Tree (Still required for 8-directional facing)
    private readonly int _dirXHash = Animator.StringToHash("DirX");
    private readonly int _dirYHash = Animator.StringToHash("DirY");

    // Your specific parameters
    private readonly int _isWalkHash = Animator.StringToHash("isWalk");
    private readonly int _isDeadHash = Animator.StringToHash("isDead");
    private readonly int _attackHash = Animator.StringToHash("attack");

    private void Awake()
    {
        _animator = GetComponent<Animator>();
        _unit = GetComponent<Unit>();
        _movement = GetComponent<UnitMovement>();
    }

    private void Update()
    {
        // If dead, ensure we don't update movement animations
        if (_unit.CurrentState == UnitState.Die) return;

        UpdateAnimations();
    }

    private void UpdateAnimations()
    {
        // Update direction for the Blend Tree (so the correct 8-directional frame is used)
        _animator.SetFloat(_dirXHash, _movement.CurrentDirection.x);
        _animator.SetFloat(_dirYHash, _movement.CurrentDirection.y);

        // Set the boolean to handle the transition between Idle and Walk states
        _animator.SetBool(_isWalkHash, _movement.IsMoving);
    }

    public void TriggerAttack()
    {
        _animator.SetTrigger(_attackHash);
    }

    public void TriggerDeath()
    {
        // Set the boolean to true so the Animator transitions to the Dead state
        _animator.SetBool(_isDeadHash, true);

        // Optionally, force stop walking animation
        _animator.SetBool(_isWalkHash, false);
    }
}
