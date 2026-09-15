using UnityEngine;
using System.Collections;
using Pathfinding;
using Core;
using Entities.Units;

public class FSMBrain : MonoBehaviour
{
    [Header("AI Scanning")]
    public float EvaluationInterval = 0.2f;
    public string TargetTag = "Player";

    [Header("Behavior Probabilities")]
    [Tooltip("Percentage chance to panic and retreat when low on HP (0-100). Default: 10")]
    [Range(0, 100)] public float RetreatChancePct = 10f;
    [Tooltip("Percentage chance to return to spawn when target is lost (e.g., 70 means 70% return, 30% guard)")]
    [Range(0, 100)] public float ReturnToSpawnChancePct = 70f;

    [Header("Combat Thresholds (Edge-to-Edge)")]
    public float AttackRange = 1.2f;
    public float ChaseRange = 7.0f;

    [Header("HP & Retreat Settings")]
    public float LowHpPercentage = 0.25f;
    public float RetreatDuration = 2.5f;

    [Header("Combat Output")]
    public float AttackDamage = 10f;
    public float AttackCooldown = 1.5f;

    // --- Memory Variables ---
    private Vector3 _spawnPosition;
    private bool _hadTarget;
    private bool _isReturningToSpawn;

    private bool _hpRiskEvaluated;
    private bool _isRetreating;
    private float _retreatTimer;
    private float _lastAttackTime;

    // --- Component References ---
    private Transform _currentTarget;
    private Entity _targetEntity;
    private Collider2D _targetCollider;
    private Unit _selfUnit;
    private AIPath _aiPath;
    private UnitAnimator _unitAnimator;

    private void Awake()
    {
        _selfUnit = GetComponent<Unit>();
        _aiPath = GetComponent<AIPath>();
        _unitAnimator = GetComponent<UnitAnimator>();
    }

    private void Start()
    {
        _lastAttackTime = Time.time - AttackCooldown;
        _spawnPosition = transform.position;
        StartCoroutine(AILoop());
    }

    private IEnumerator AILoop()
    {
        while (true)
        {
            if (_selfUnit != null && _selfUnit.CurrentState != UnitState.Die)
            {
                FindNearestTarget();
                EvaluateTacticalSituation();
            }
            yield return new WaitForSeconds(EvaluationInterval);
        }
    }

    private void FindNearestTarget()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag(TargetTag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (var obj in players)
        {
            var ent = obj.GetComponent<Entity>();
            if (ent == null || ent.IsDead) continue;
            float d = Vector3.Distance(obj.transform.position, myPos);
            if (d < minDist) { minDist = d; closest = obj; }
        }

        if (closest != null)
        {
            _currentTarget = closest.transform;
            _targetEntity = closest.GetComponent<Entity>();
            _targetCollider = closest.GetComponent<Collider2D>();

            _hadTarget = true;
            _isReturningToSpawn = false;
        }
        else
        {
            _currentTarget = null;
            _targetEntity = null;
            _targetCollider = null;

            if (_hadTarget)
            {
                _hadTarget = false;

                float roll = Random.Range(0f, 100f);
                if (roll <= ReturnToSpawnChancePct)
                {
                    _isReturningToSpawn = true;
                    Debug.Log($"[{gameObject.name}] Target lost. Roll ({roll:F1}) <= {ReturnToSpawnChancePct}. Returning to spawn.");
                }
                else
                {
                    _isReturningToSpawn = false;
                    Debug.Log($"[{gameObject.name}] Target lost. Roll ({roll:F1}) > {ReturnToSpawnChancePct}. Guarding in place.");
                }
            }
        }
    }

    public void EvaluateTacticalSituation()
    {
        if (_currentTarget == null)
        {
            if (_isReturningToSpawn)
            {
                _selfUnit.CurrentState = UnitState.Walk;
                if (_aiPath != null)
                {
                    _aiPath.isStopped = false;
                    _aiPath.destination = _spawnPosition;
                }

                if (Vector3.Distance(transform.position, _spawnPosition) <= 0.5f)
                {
                    _isReturningToSpawn = false;
                    _selfUnit.CurrentState = UnitState.Idle;
                    if (_aiPath != null) _aiPath.isStopped = true;
                }
            }
            else
            {
                _selfUnit.CurrentState = UnitState.Idle;
                if (_aiPath != null) _aiPath.isStopped = true;
            }
            return;
        }

        float hpPct = _selfUnit.CurrentHealth / _selfUnit.MaxHealth;
        float edgeDistance = GetEdgeDistance();

        if (hpPct < LowHpPercentage)
        {
            if (!_hpRiskEvaluated)
            {
                _hpRiskEvaluated = true;

                float roll = Random.Range(0f, 100f);
                if (roll <= RetreatChancePct)
                {
                    _isRetreating = true;
                    _retreatTimer = RetreatDuration;
                    Debug.Log($"[{gameObject.name}] Low HP Panic! Roll ({roll:F1}) <= {RetreatChancePct}. Retreating for {RetreatDuration}s!");
                }
            }
        }
        else
        {
            _hpRiskEvaluated = false;
            _isRetreating = false;
        }

        if (_isRetreating)
        {
            _retreatTimer -= EvaluationInterval;

            if (_retreatTimer <= 0)
            {
                float reRoll = Random.Range(0f, 100f);
                if (reRoll <= RetreatChancePct)
                {
                    _retreatTimer = RetreatDuration;
                    Debug.Log($"[{gameObject.name}] Re-roll ({reRoll:F1}) <= {RetreatChancePct}. Still retreating!");
                }
                else
                {
                    _isRetreating = false;
                    Debug.Log($"[{gameObject.name}] Re-roll ({reRoll:F1}) > {RetreatChancePct}. Regained courage. Back to fight!");
                }
            }

            if (_isRetreating)
            {
                _selfUnit.CurrentState = UnitState.Walk;
                if (_aiPath != null)
                {
                    _aiPath.isStopped = false;
                    Vector3 retreatDir = (transform.position - _currentTarget.position).normalized;
                    _aiPath.destination = transform.position + retreatDir * 3f;
                }
                return;
            }
        }

        switch (_selfUnit.CurrentState)
        {
            case UnitState.Idle:
                if (edgeDistance <= ChaseRange)
                {
                    _selfUnit.CurrentState = UnitState.Walk;
                }
                break;

            case UnitState.Walk:
                if (edgeDistance <= AttackRange)
                {
                    _selfUnit.CurrentState = UnitState.Attack;
                }
                else if (edgeDistance > ChaseRange)
                {
                    _selfUnit.CurrentState = UnitState.Idle;
                }
                else
                {
                    if (_aiPath != null)
                    {
                        _aiPath.isStopped = false;
                        _aiPath.destination = _currentTarget.position;
                    }
                }
                break;

            case UnitState.Attack:
                if (edgeDistance > AttackRange)
                {
                    _selfUnit.CurrentState = UnitState.Walk;
                }
                else
                {
                    if (_aiPath != null) _aiPath.isStopped = true;
                    if (Time.time - _lastAttackTime >= AttackCooldown)
                    {
                        if (_unitAnimator != null) _unitAnimator.TriggerAttack();
                        if (_targetEntity != null) _targetEntity.TakeDamage(AttackDamage);
                        _lastAttackTime = Time.time;
                    }
                }
                break;
        }
    }

    private float GetEdgeDistance()
    {
        if (_targetCollider != null)
        {
            Vector2 closestPoint = _targetCollider.ClosestPoint(transform.position);
            return Vector2.Distance(transform.position, closestPoint);
        }
        return Vector3.Distance(transform.position, _currentTarget.position);
    }
}