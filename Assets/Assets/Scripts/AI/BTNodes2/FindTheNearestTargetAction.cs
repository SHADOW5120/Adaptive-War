using System;
using Unity.Behavior;
using UnityEngine;
using Action = Unity.Behavior.Action;
using Unity.Properties;

[Serializable, GeneratePropertyBag]
[NodeDescription(name: "Find The Nearest Target", story: "[Object] Finds The Nearest [Target]", category: "Action", id: "6424edaf078d4097542775e000d655d5")]
public partial class FindTheNearestTargetAction : Action
{
    [SerializeReference] public BlackboardVariable<GameObject> Object;
    [SerializeReference] public BlackboardVariable<GameObject> Target;

    // Set this in BT Inspector to the tag of objects to search for
    // FastFreddy (Enemy tag) -> set TargetTag = "Player"
    // Player units (Player tag) -> set TargetTag = "Enemy"
    [SerializeReference] public BlackboardVariable<string> TargetTag;

    protected override Status OnStart()
    {
        if (Object.Value == null) return Status.Failure;

        // Use explicit TargetTag if set, otherwise auto-detect from own tag
        string searchTag;
        if (TargetTag != null && !string.IsNullOrEmpty(TargetTag.Value))
        {
            searchTag = TargetTag.Value;
        }
        else
        {
            // Fallback: auto-detect
            string myTag = Object.Value.tag;
            searchTag = myTag == "Player" ? "Enemy" : "Player";
            Debug.LogWarning($"[FindNearest] TargetTag not set on {Object.Value.name}, " +
                             $"auto-detected: searching for tag '{searchTag}'");
        }

        GameObject[] candidates = GameObject.FindGameObjectsWithTag(searchTag);
        GameObject closest = null;
        float minDist = Mathf.Infinity;
        Vector3 myPos = Object.Value.transform.position;

        foreach (var obj in candidates)
        {
            var e = obj.GetComponent<Entity>();
            if (e == null || e.IsDead) continue;
            float d = Vector3.Distance(obj.transform.position, myPos);
            if (d < minDist) { minDist = d; closest = obj; }
        }

        if (closest == null)
        {
            Debug.Log($"[FindNearest] No living target found with tag '{searchTag}'");
            return Status.Failure;
        }

        Target.Value = closest;
        Debug.Log($"[FindNearest] Found target: {closest.name} (tag={searchTag}, dist={minDist:F1})");
        return Status.Success;
    }

    protected override Status OnUpdate() => Status.Success;
    protected override void OnEnd() { }
}

