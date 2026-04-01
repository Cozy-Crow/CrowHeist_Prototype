using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Manages the slot-selection panel used for both saving and loading.
/// Attach this to the SaveSlotPanel GameObject.
///
/// In the Inspector wire up:
///   - slotButtons[0..2]  → the three Button GameObjects (Slot 1, 2, 3)
///   - slotLabels[0..2]   → TMP_Text components that show timestamp or "Empty"
///   - backButton         → returns to the main pause menu
///   - titleLabel         → shows "Save Game" or "Load Game"
///
/// Call OpenForSave() or OpenForLoad() from PauseManager.
/// </summary>
public class SaveSlotPanel : MonoBehaviour
{
    public enum Mode { Save, Load }

    [Header("Panel")]
    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleLabel;

    [Header("Slot Buttons (index 0 = Slot 1)")]
    [SerializeField] private Button[] slotButtons   = new Button[SaveLoadSystem.SlotCount];
    [SerializeField] private TMP_Text[] slotLabels  = new TMP_Text[SaveLoadSystem.SlotCount];

    [Header("Back Button")]
    [SerializeField] private Button backButton;

    private Mode currentMode;

    // Callback invoked after a slot action completes so PauseManager can react
    public System.Action<int> OnSlotSelected;
    public System.Action       OnBack;

    private void Start()
    {
        for (int i = 0; i < slotButtons.Length; i++)
        {
            int slotIndex = i + 1; // 1-based slot
            if (slotButtons[i] != null)
                slotButtons[i].onClick.AddListener(() => OnSlotClicked(slotIndex));
        }

        if (backButton != null)
            backButton.onClick.AddListener(Close);

        if (panel != null)
            panel.SetActive(false);
    }

    // ──────────────────────────────────────────────
    //  Open / Close
    // ──────────────────────────────────────────────

    public void OpenForSave()
    {
        currentMode = Mode.Save;
        if (titleLabel != null) titleLabel.text = "Save Game";
        Open();
    }

    public void OpenForLoad()
    {
        currentMode = Mode.Load;
        if (titleLabel != null) titleLabel.text = "Load Game";
        Open();
    }

    public void Close()
    {
        if (panel != null) panel.SetActive(false);
        OnBack?.Invoke();
    }

    public bool IsOpen => panel != null && panel.activeSelf;

    // ──────────────────────────────────────────────
    //  Internal
    // ──────────────────────────────────────────────

    private void Open()
    {
        RefreshSlotLabels();
        if (panel != null) panel.SetActive(true);
    }

    private void RefreshSlotLabels()
    {
        for (int i = 0; i < SaveLoadSystem.SlotCount; i++)
        {
            int slot = i + 1;
            if (slotLabels[i] == null) continue;

            string timestamp = SaveLoadSystem.Instance != null
                ? SaveLoadSystem.Instance.GetSlotTimestamp(slot)
                : null;

            slotLabels[i].text = timestamp != null
                ? $"Slot {slot}\n{timestamp}"
                : $"Slot {slot}\nEmpty";

            // In Load mode, disable buttons for empty slots
            if (slotButtons[i] != null && currentMode == Mode.Load)
                slotButtons[i].interactable = timestamp != null;

            // In Save mode all slots are always available
            if (slotButtons[i] != null && currentMode == Mode.Save)
                slotButtons[i].interactable = true;
        }
    }

    private void OnSlotClicked(int slot)
    {
        if (SaveLoadSystem.Instance == null) return;

        if (currentMode == Mode.Save)
        {
            SaveLoadSystem.Instance.SaveGame(slot);
            RefreshSlotLabels(); // update timestamp shown on the button
        }
        else
        {
            SaveLoadSystem.Instance.LoadGame(slot);
            Close();
        }

        OnSlotSelected?.Invoke(slot);
    }
}
