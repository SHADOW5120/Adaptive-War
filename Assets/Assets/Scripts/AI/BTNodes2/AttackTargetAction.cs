//using System;
//using Unity.Behavior;
//using UnityEngine;
//using Action = Unity.Behavior.Action;
//using Unity.Properties;

//[Serializable, GeneratePropertyBag]
//[NodeDescription(name: "Attack Target", story: "[Object] Attacks [Target]", category: "Action", id: "50d2008751f2906b42396f3fa3cbc09c")]
//public partial class AttackTargetAction : Action
//{
//    [SerializeReference] public BlackboardVariable<GameObject> Object;
//    [SerializeReference] public BlackboardVariable<GameObject> Target;

//    public float AttackDamage = 15f;
//    public float AttackCooldown = 1.2f;
//    public float AttackRange = 1.5f;

//    private UnitAnimator _unitAnimator;
//    private Animator _animator;
//    private float _lastAttackTime;

//    protected override Status OnStart()
//    {
//        if (Object.Value == null || Target.Value == null) return Status.Failure;

//        _unitAnimator = Object.Value.GetComponent<UnitAnimator>();
//        _animator = Object.Value.GetComponent<Animator>();
//        _lastAttackTime = Time.time - AttackCooldown; // allow immediate first hit
//        return Status.Running;
//    }

//    protected override Status OnUpdate()
//    {
//        if (Target.Value == null) return Status.Success;

//        Entity targetEntity = Target.Value.GetComponent<Entity>();
//        if (targetEntity == null || targetEntity.IsDead)
//            return Status.Success;

//        float dist = Vector3.Distance(
//            Object.Value.transform.position,
//            Target.Value.transform.position);

//        // Out of range ? Failure ? BT restarts sequence ? BrainEvaluates re-runs
//        if (dist > AttackRange + 0.2f)
//            return Status.Failure;

//        SetAttackDirection();

//        if (Time.time - _lastAttackTime >= AttackCooldown)
//        {
//            _unitAnimator?.TriggerAttack();
//            targetEntity.TakeDamage(AttackDamage);
//            _lastAttackTime = Time.time;
//        }

//        return Status.Running;
//    }

//    protected override void OnEnd() { }

//    private void SetAttackDirection()
//    {
//        if (_animator == null || Target.Value == null) return;
//        Vector2 dir = ((Vector2)(Target.Value.transform.position
//                        - Object.Value.transform.position)).normalized;
//        _animator.SetFloat("DirX", dir.x);
//        _animator.SetFloat("DirY", dir.y);
//    }
//}

