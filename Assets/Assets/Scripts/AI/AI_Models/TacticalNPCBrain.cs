using UnityEngine;
using Unity.InferenceEngine;
using Core;

public class TacticalBrain : MonoBehaviour
{
    [Header("AI Model")]
    public ModelAsset aiModelAsset;

    [Header("Must match map_bound in Python training (default 8)")]
    public float mapBound = 8f;

    [Tooltip("Seconds between inference calls. 0.15 = ~6 times per second")]
    public float evalCooldown = 0.15f;

    [Header("Game References")]
    public Entity selfEntity;
    public Entity enemyEntity;

    [Header("Outputs - Read Only")]
    public int CurrentAction = 0;
    public Vector3 TargetMovePosition = Vector3.zero;

    [Header("Debug - Disable in build")]
    public bool debugLog = true;
    public bool debugLogRaw = false;  // print raw obs + all logits
    public int debugLogEvery = 10;     // print every N inference calls

    private int _evalCount = 0;
    private float _lastLogTime = 0f;

    private Worker _worker;
    private float[] _obs = new float[6];
    private float _lastEvalTime = -999f;

    // Action index names, matches action_mapping.json
    private static readonly string[] ActionNames =
        { "Idle","Move N","Move NE","Move E","Move SE",
          "Move S","Move SW","Move W","Move NW","Attack" };

    // Direction vectors per action index (1-8), index 0 is unused placeholder
    private static readonly Vector2[] DirMap =
    {
        Vector2.zero,              // 0  Idle
        new Vector2( 0f,   1f),   // 1  N
        new Vector2( 0.7f, 0.7f), // 2  NE
        new Vector2( 1f,   0f),   // 3  E
        new Vector2( 0.7f,-0.7f), // 4  SE
        new Vector2( 0f,  -1f),   // 5  S
        new Vector2(-0.7f,-0.7f), // 6  SW
        new Vector2(-1f,   0f),   // 7  W
        new Vector2(-0.7f, 0.7f), // 8  NW
    };

    private void Awake()
    {
        if (selfEntity == null)
            selfEntity = GetComponent<Entity>();
    }

    private void Start()
    {
        if (aiModelAsset == null)
        {
            Debug.LogError($"[TacticalBrain] {name}: aiModelAsset not assigned! " +
                           "Drag npc_tactical_iso2d_best into the Inspector.");
            return;
        }

        Model model = ModelLoader.Load(aiModelAsset);
        var backend = SystemInfo.supportsComputeShaders
            ? BackendType.GPUCompute
            : BackendType.CPU;
        _worker = new Worker(model, backend);

        Debug.Log($"[TacticalBrain] {name}: Model loaded OK — backend={backend}");
    }

    private void OnDestroy() => _worker?.Dispose();

    /// <summary>Called by BT Node and AIDestinationPPO.</summary>
    public void EvaluateTacticalSituation()
    {
        // Guard: worker not ready
        if (_worker == null)
        {
            if (debugLog) Debug.LogWarning($"[TacticalBrain] {name}: Worker is null — model not loaded?");
            CurrentAction = 0;
            return;
        }

        // Guard: self entity missing or dead
        if (selfEntity == null || selfEntity.IsDead)
        {
            if (debugLog) Debug.Log($"[TacticalBrain] {name}: selfEntity null/dead -> Idle");
            CurrentAction = 0;
            return;
        }

        // Guard: enemy entity missing or dead
        if (enemyEntity == null || enemyEntity.IsDead)
        {
            if (debugLog && Time.time - _lastLogTime > 1f)
            {
                Debug.Log($"[TacticalBrain] {name}: enemyEntity null/dead -> Idle " +
                          "(BT has not injected enemy yet, or enemy is dead)");
                _lastLogTime = Time.time;
            }
            CurrentAction = 0;
            return;
        }

        // Cooldown throttle
        if (Time.time - _lastEvalTime < evalCooldown) return;
        _lastEvalTime = Time.time;
        _evalCount++;

        // Build observation — order MUST match _get_obs() in Python
        Vector2 myPos = transform.position;
        Vector2 enemyPos = enemyEntity.transform.position;
        float dx = enemyPos.x - myPos.x;
        float dy = enemyPos.y - myPos.y;
        float dist = Vector2.Distance(myPos, enemyPos);

        _obs[0] = selfEntity.CurrentHealth / selfEntity.MaxHealth;   // agent_hp
        _obs[1] = dx / 10f;                                          // dx / 10
        _obs[2] = dy / 10f;                                          // dy / 10
        _obs[3] = dist / 10f;                                          // dist / 10
        _obs[4] = enemyEntity.CurrentHealth / enemyEntity.MaxHealth;   // enemy_hp
        _obs[5] = dist <= 2.0f ? 1f : 0f;                             // in_range flag

        // Inference — InferenceEngine 2.2 API: SetInput + Schedule (no string overload on Schedule)
        using var inputTensor = new Tensor<float>(new TensorShape(1, 6), _obs);
        _worker.SetInput("obs", inputTensor);
        _worker.Schedule();
        using var logits = (_worker.PeekOutput("logits") as Tensor<float>)
                           .ReadbackAndClone();

        int prevAction = CurrentAction;
        CurrentAction = ArgMax(logits);

        // Decode action 1-8 into TargetMovePosition for A*
        if (CurrentAction >= 1 && CurrentAction <= 8)
        {
            Vector2 dir = DirMap[CurrentAction];
            Vector3 target = (Vector3)(myPos + dir * 2f);
            target.x = Mathf.Clamp(target.x, -mapBound, mapBound);
            target.y = Mathf.Clamp(target.y, -mapBound, mapBound);
            target.z = transform.position.z;
            TargetMovePosition = target;
        }

        // Debug logging
        if (!debugLog) return;

        // Verbose mode: raw obs + all logit values
        if (debugLogRaw && _evalCount % debugLogEvery == 1)
        {
            string obsStr =
                $"obs=[hp:{_obs[0]:F2}, dx:{_obs[1]:F2}, dy:{_obs[2]:F2}, " +
                $"dist:{_obs[3]:F2}, eHp:{_obs[4]:F2}, inRange:{_obs[5]:F0}]";

            var sb = new System.Text.StringBuilder("logits=[");
            for (int i = 0; i < logits.shape.length; i++)
                sb.Append($"{ActionNames[i]}:{logits[i]:F2}" +
                          (i < logits.shape.length - 1 ? ", " : ""));
            sb.Append("]");

            Debug.Log($"[TacticalBrain] #{_evalCount} {name}\n  {obsStr}\n  {sb}");
        }

        // Summary: print on action change or every debugLogEvery calls
        bool changed = CurrentAction != prevAction;
        if (changed || _evalCount % debugLogEvery == 0)
        {
            string tag = changed
                ? $"<color=yellow>ACTION CHANGED {ActionNames[prevAction]} -> {ActionNames[CurrentAction]}</color>"
                : $"action={ActionNames[CurrentAction]}";

            string moveStr = (CurrentAction >= 1 && CurrentAction <= 8)
                ? $" | moveTo=({TargetMovePosition.x:F1},{TargetMovePosition.y:F1})"
                : "";

            Debug.Log($"[TacticalBrain] {name} #{_evalCount} | " +
                      $"dist={dist:F2}u | inRange={(dist <= 2f ? "YES" : "no")} | " +
                      $"selfHp={_obs[0]:P0} | enemyHp={_obs[4]:P0} | {tag}{moveStr}");
        }
    }

    private static int ArgMax(Tensor<float> t)
    {
        int best = 0; float max = float.MinValue;
        for (int i = 0; i < t.shape.length; i++)
            if (t[i] > max) { max = t[i]; best = i; }
        return best;
    }
}