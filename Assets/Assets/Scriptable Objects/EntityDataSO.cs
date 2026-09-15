using UnityEngine;

[CreateAssetMenu(fileName = "EntityDataSO", menuName = "Scriptable Objects/EntityDataSO")]
public class EntityDataSO : ScriptableObject
{
    [Header("General Information")]
    public string entityName = "New Entity";
    public Sprite icon;

    [TextArea(3, 5)]
    public string description = "Enter description here...";

    public Core.EntityType entityType;

    [Header("Economy & Core Stats")]
    public int cost = 100;
    public float maxHealth = 100f;

    [Header("In-Game Prefab")]
    public GameObject prefab; // The actual GameObject to spawn
}
