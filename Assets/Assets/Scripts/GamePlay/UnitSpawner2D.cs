using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using TMPro;

public class UnitSpawner2D : MonoBehaviour
{
    public static bool IsPlacingUnit { get; private set; } = false;

    [Header("Economy")]
    public int playerMoney = 500;
    public TextMeshProUGUI moneyText;

    private UnitDataSO currentUnitData;
    private GameObject currentPreview;
    private SpriteRenderer previewRenderer;

    void Start()
    {
        Debug.Log("[UnitSpawner2D] System initialized. Toggle selection enabled.");
        UpdateMoneyUI();
    }

    private void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = playerMoney.ToString();
        }
    }

    public void SelectUnit(UnitDataSO unitData)
    {
        // TOGGLE LOGIC: If clicking the exact same button while placing, cancel the placement
        if (currentUnitData == unitData)
        {
            Debug.Log("[SelectUnit] Same unit clicked. Canceling placement.");
            CancelPlacement();
            return;
        }

        // NORMAL SELECTION LOGIC
        if (playerMoney >= unitData.cost)
        {
            if (currentPreview != null) Destroy(currentPreview);

            currentUnitData = unitData;
            IsPlacingUnit = true; // Lock camera panning

            currentPreview = new GameObject("PreviewHologram");
            previewRenderer = currentPreview.AddComponent<SpriteRenderer>();

            SpriteRenderer originalSprite = unitData.prefab.GetComponentInChildren<SpriteRenderer>();
            if (originalSprite != null)
            {
                previewRenderer.sprite = originalSprite.sprite;
                previewRenderer.sortingOrder = 999;
            }
        }
        else
        {
            Debug.LogWarning("[SelectUnit] Not enough gold!");
        }
    }

    void Update()
    {
        if (Pointer.current == null) return;

        if (currentUnitData != null && currentPreview != null)
        {
            Vector2 pointerScreenPos = Pointer.current.position.ReadValue();
            Vector3 pointerWorldPos = Camera.main.ScreenToWorldPoint(pointerScreenPos);
            pointerWorldPos.z = 0f;
            currentPreview.transform.position = pointerWorldPos;

            // Placement is valid by default anywhere on the screen
            bool isValidPlacement = true;

            // Block placement if pointer is over UI (like tapping a button)
            if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            {
                isValidPlacement = false;
            }

            if (previewRenderer != null)
            {
                previewRenderer.color = isValidPlacement ? new Color(0f, 1f, 0f, 0.5f) : new Color(1f, 0f, 0f, 0.5f);
            }

            // TAP (Mobile) or LEFT CLICK (PC) to place the unit
            if (Pointer.current.press.wasPressedThisFrame)
            {
                if (isValidPlacement)
                {
                    playerMoney -= currentUnitData.cost;
                    UpdateMoneyUI();

                    Instantiate(currentUnitData.prefab, pointerWorldPos, Quaternion.identity);

                    if (playerMoney < currentUnitData.cost)
                    {
                        CancelPlacement();
                    }
                }
            }

            // Keep Right Click for PC users as a convenient shortcut (Optional)
            if (Mouse.current != null && Mouse.current.rightButton.wasPressedThisFrame)
            {
                CancelPlacement();
            }
        }
    }

    // Helper method to completely clear the placement state
    private void CancelPlacement()
    {
        if (currentPreview != null) Destroy(currentPreview);
        currentUnitData = null;
        IsPlacingUnit = false; // Unlock camera panning
    }
}