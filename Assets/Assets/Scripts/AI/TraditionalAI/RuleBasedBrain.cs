using UnityEngine;
using System.Collections;
using Pathfinding;
using Core;
using Entities.Units;

public class RuleBasedBrain : MonoBehaviour
{
    [Header("AI Scanning Settings")]
    public float EvaluationInterval = 0.2f;
    public string TargetTag = "Player";

    [Header("Behavior Probabilities")]
    [Tooltip("Percentage chance to panic and retreat when low on HP (0-100). Default: 10")]
    [Range(0, 100)] public float RetreatChancePct = 10f;
    [Tooltip("Percentage chance to return to spawn when target is lost (70% return, 30% guard)")]
    [Range(0, 100)] public float ReturnToSpawnChancePct = 70f;

    [Header("Combat Thresholds (Edge-to-Edge)")]
    public float AttackRange = 1.2f;
    public float DetectionRange = 7.0f;

    [Header("HP Thresholds")]
    public float LowHpPercentage = 0.25f;

    [Header("Combat Settings")]
    public float AttackDamage = 10f;
    public float AttackCooldown = 1.5f;
    private float _lastAttackTime;

    // --- Short-term Memory for Rules ---
    private Vector3 _spawnPosition;
    private Vector3 _retreatDestination;
    private bool _isRetreating = false;
    private bool _hasEvaluatedLowHp = false;

    private bool _hadTargetLastFrame = false;
    private bool _isReturningToSpawn = false;

    // --- References ---
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
        _spawnPosition = transform.position;
        _lastAttackTime = Time.time - AttackCooldown;
        StartCoroutine(RuleLoop());
    }

    private IEnumerator RuleLoop()
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
        GameObject[] targets = GameObject.FindGameObjectsWithTag(TargetTag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = transform.position;

        foreach (var obj in targets)
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
        }
        else
        {
            _currentTarget = null;
            _targetEntity = null;
            _targetCollider = null;
        }
    }

    public void EvaluateTacticalSituation()
    {
        float hpPct = _selfUnit.CurrentHealth / _selfUnit.MaxHealth;

        // =======================================================================
        // RULE 1: ACTIVE RETREATING (HIGHEST PRIORITY)
        // Force the agent to move to the designated point before re-evaluating.
        // =======================================================================
        if (_isRetreating)
        {
            float distToRetreatPoint = Vector3.Distance(transform.position, _retreatDestination);

            if (distToRetreatPoint > 0.6f)
            {
                _selfUnit.CurrentState = UnitState.Walk;
                if (_aiPath != null)
                {
                    _aiPath.isStopped = false;
                    _aiPath.destination = _retreatDestination;
                }
                return;
            }
            else
            {
                Debug.Log($"[{gameObject.name}] Reached designated retreat point. Re-evaluating panic rules...");

                if (hpPct < LowHpPercentage && _currentTarget != null)
                {
                    float roll = Random.Range(0f, 100f);
                    if (roll <= RetreatChancePct)
                    {
                        Vector3 retreatDir = (transform.position - _currentTarget.position).normalized;
                        _retreatDestination = transform.position + retreatDir * 4f;
                        _isRetreating = true;

                        Debug.Log($"[{gameObject.name}] Re-roll failed ({roll:F1} <= {RetreatChancePct}%). Panic extends! Moving to new point.");
                        return;
                    }
                }

                _isRetreating = false;
                _hasEvaluatedLowHp = false;
                Debug.Log($"[{gameObject.name}] Panic cleared. Standing ground.");
            }
        }

        // =======================================================================
        // RULE 2: LOW HP PANIC ROLL
        // Rolls the dice exactly ONCE when entering the low HP threshold to avoid spam.
        // =======================================================================
        if (hpPct < LowHpPercentage && _currentTarget != null)
        {
            if (!_hasEvaluatedLowHp)
            {
                _hasEvaluatedLowHp = true;
                float roll = Random.Range(0f, 100f);

                if (roll <= RetreatChancePct)
                {
                    _isRetreating = true;
                    Vector3 retreatDir = (transform.position - _currentTarget.position).normalized;
                    _retreatDestination = transform.position + retreatDir * 4f;

                    _selfUnit.CurrentState = UnitState.Walk;
                    if (_aiPath != null)
                    {
                        _aiPath.isStopped = false;
                        _aiPath.destination = _retreatDestination;
                    }
                    Debug.Log($"[{gameObject.name}] Low HP! Panic roll succeeded ({roll:F1} <= {RetreatChancePct}%). Fleeing to point.");
                    return;
                }
                else
                {
                    Debug.Log($"[{gameObject.name}] Low HP but roll failed ({roll:F1} > {RetreatChancePct}%). Defending the base to the death!");
                }
            }
        }
        else if (hpPct >= LowHpPercentage)
        {
            _hasEvaluatedLowHp = false;
        }

        // =======================================================================
        // RULE 3: COMBAT AND CHASE
        // =======================================================================
        if (_currentTarget != null)
        {
            _hadTargetLastFrame = true;
            _isReturningToSpawn = false;

            float edgeDistance = GetEdgeDistance();

            if (edgeDistance <= AttackRange)
            {
                _selfUnit.CurrentState = UnitState.Attack;
                if (_aiPath != null) _aiPath.isStopped = true;

                ExecuteAttackLogic();
                return;
            }

            if (edgeDistance <= DetectionRange)
            {
                _selfUnit.CurrentState = UnitState.Walk;
                if (_aiPath != null)
                {
                    _aiPath.isStopped = false;
                    _aiPath.destination = _currentTarget.position;
                }
                return;
            }

            _currentTarget = null;
        }

        // =======================================================================
        // RULE 4: LOST TARGET BEHAVIOR
        // =======================================================================
        if (_currentTarget == null)
        {
            if (_hadTargetLastFrame)
            {
                _hadTargetLastFrame = false;
                float roll = Random.Range(0f, 100f);

                if (roll <= ReturnToSpawnChancePct)
                {
                    _isReturningToSpawn = true;
                    Debug.Log($"[{gameObject.name}] Lost target! Dice roll: {roll:F1} <= {ReturnToSpawnChancePct}%. Traveling back to spawn point.");
                }
                else
                {
                    _isReturningToSpawn = false;
                    Debug.Log($"[{gameObject.name}] Lost target! Dice roll: {roll:F1} > {ReturnToSpawnChancePct}%. Holding ground here.");
                }
            }

            if (_isReturningToSpawn)
            {
                float distToSpawn = Vector3.Distance(transform.position, _spawnPosition);
                if (distToSpawn > 0.5f)
                {
                    _selfUnit.CurrentState = UnitState.Walk;
                    if (_aiPath != null)
                    {
                        _aiPath.isStopped = false;
                        _aiPath.destination = _spawnPosition;
                    }
                    return;
                }
                else
                {
                    _isReturningToSpawn = false;
                }
            }

            _selfUnit.CurrentState = UnitState.Idle;
            if (_aiPath != null) _aiPath.isStopped = true;
        }
    }

    private void ExecuteAttackLogic()
    {
        if (Time.time - _lastAttackTime >= AttackCooldown)
        {
            if (_unitAnimator != null) _unitAnimator.TriggerAttack();
            if (_targetEntity != null) _targetEntity.TakeDamage(AttackDamage);
            _lastAttackTime = Time.time;
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