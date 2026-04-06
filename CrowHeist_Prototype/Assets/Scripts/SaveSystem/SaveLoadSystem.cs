using KinematicCharacterController.Examples;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

    public const int SlotCount = 3;

    [Header("Save Settings")]
    [SerializeField] private string saveFilePrefix = "gamesave_slot";

    [Header("References")]
    [SerializeField] private Controller2Point5D player;
    [SerializeField] private RoombAi roomba;

    /// <summary>The last slot used for save or load (1-based). Defaults to 1.</summary>
    public int CurrentSlot { get; private set; } = 1;

    private string GetSavePath(int slot) =>
        Path.Combine(Application.persistentDataPath, $"{saveFilePrefix}{slot}.json");

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    private void Start()
    {
        if (player == null)
            player = FindObjectOfType<Controller2Point5D>();

        if (roomba == null)
            roomba = FindObjectOfType<RoombAi>();
    }

    private void Update()
    {
        // Quick-save / quick-load still target the last used slot (default slot 1)
        if (Input.GetKeyDown(KeyCode.F5))
            SaveGame(CurrentSlot);

        if (Input.GetKeyDown(KeyCode.F9))
            LoadGame(CurrentSlot);

        if (Input.GetKeyDown(KeyCode.F10))
            DeleteAllSaveData();
    }

    // ──────────────────────────────────────────────
    //  Save
    // ──────────────────────────────────────────────

    /// <summary>Save to the specified slot (1-3).</summary>
    public void SaveGame(int slot)
    {
        slot = ClampSlot(slot);
        CurrentSlot = slot;
        string savePath = GetSavePath(slot);

        try
        {
            SaveData data = new SaveData();

            // Player data
            if (player != null)
            {
                data.playerPosition = new Vector3Data(player.transform.position);
                data.playerRotation = new QuaternionData(player.transform.rotation);
                data.playerIsDirty = player.isDirty;

                if (player.heldObject != null)
                {
                    UniqueID heldID = player.heldObject.gameObject.GetComponent<UniqueID>();
                    data.heldObjectID = heldID != null ? heldID.ID : string.Empty;
                }
                else
                {
                    data.heldObjectID = string.Empty;
                }
            }
            else
            {
                Debug.LogWarning("SaveLoadSystem: Player reference is null!");
            }

            // Score data
            data.score = GameManager.Score;
            data.altCoinsScore = GameManager.AltCoinsScore;

            // Roomba data
            if (roomba != null)
            {
                data.roombaData = new RoombaData
                {
                    position    = new Vector3Data(roomba.transform.position),
                    isActivated = roomba.isActivated,
                    isBroken    = roomba.isBroken
                };
            }
            else
            {
                Debug.LogWarning("SaveLoadSystem: Roomba reference is null!");
                data.roombaData = new RoombaData();
            }

            // Item data
            UniqueID[] allItems = FindObjectsOfType<UniqueID>();
            foreach (UniqueID item in allItems)
            {
                if (item.GetComponent<Controller2Point5D>() != null ||
                    item.GetComponent<RoombAi>() != null)
                    continue;

                Pickable pickable = item.GetComponent<Pickable>();
                bool isDirty = false, isHeld = false, hasBeenPickedUp = false;

                if (pickable != null)
                {
                    isDirty         = pickable._isDirty;
                    isHeld          = pickable.pickedUp;
                    hasBeenPickedUp = pickable.HasBeenPickedUp;
                }

                data.items.Add(new ItemData(
                    item.ID,
                    item.gameObject.name.Replace("(Clone)", "").Trim(),
                    (int)item.ObjectType,
                    item.transform.position,
                    item.transform.eulerAngles,
                    isDirty, isHeld,
                    item.gameObject.activeSelf,
                    hasBeenPickedUp
                ));
            }

            // Registry data
            if (PickupRegistry.Instance != null)
                data.registryData = PickupRegistry.Instance.GenerateSaveData();

            data.saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(savePath, json);

            Debug.Log($"[SaveLoadSystem] Slot {slot} saved → {savePath}");
            Debug.Log($"Saved {data.items.Count} items, Score: {data.score}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadSystem] Failed to save slot {slot}: {e.Message}");
        }
    }

    // ──────────────────────────────────────────────
    //  Load
    // ──────────────────────────────────────────────

    /// <summary>Load from the specified slot (1-3).</summary>
    public void LoadGame(int slot)
    {
        slot = ClampSlot(slot);
        CurrentSlot = slot;
        string savePath = GetSavePath(slot);

        if (!File.Exists(savePath))
        {
            Debug.LogWarning($"[SaveLoadSystem] No save file for slot {slot} at: {savePath}");
            return;
        }

        try
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                Debug.LogError("[SaveLoadSystem] Failed to deserialize save data!");
                return;
            }

            // Player
            if (player != null)
            {
                player.transform.position = data.playerPosition.ToVector3();
                player.transform.rotation = data.playerRotation.ToQuaternion();
                player.isDirty = data.playerIsDirty;

                if (player.heldObject != null)
                {
                    Pickable currentHeld = player.heldObject.gameObject.GetComponent<Pickable>();
                    if (currentHeld != null)
                        currentHeld.Drop(currentHeld.transform.position);
                }
            }

            // Scores
            GameManager.Score = data.score;
            GameManager.AltCoinsScore = data.altCoinsScore;

            if (UIManager.Instance != null && UIManager.Instance.CoinsUI != null)
                UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);

            // Roomba
            if (roomba != null && data.roombaData != null)
            {
                roomba.transform.position = data.roombaData.position.ToVector3();
                roomba.isActivated = data.roombaData.isActivated;
                roomba.isBroken = data.roombaData.isBroken;

                if (data.roombaData.isActivated)
                    roomba.Activate();
                else
                    roomba.Deactivate();
            }

            // Items
            Dictionary<string, ItemData> itemDataDict = new Dictionary<string, ItemData>();
            foreach (ItemData itemData in data.items)
                itemDataDict[itemData.uniqueID] = itemData;

            UniqueID[] allItems = FindObjectsOfType<UniqueID>();
            foreach (UniqueID item in allItems)
            {
                if (item.GetComponent<Controller2Point5D>() != null ||
                    item.GetComponent<RoombAi>() != null)
                    continue;

                if (itemDataDict.TryGetValue(item.ID, out ItemData savedData))
                {
                    item.transform.position   = savedData.position.ToVector3();
                    item.transform.eulerAngles = savedData.rotation.ToVector3();
                    item.gameObject.SetActive(savedData.isActive);

                    Pickable pickable = item.GetComponent<Pickable>();
                    if (pickable != null)
                    {
                        pickable._isDirty        = savedData.isDirty;
                        pickable.HasBeenPickedUp = savedData.hasBeenPickedUp;

                        if (savedData.isHeld &&
                            !string.IsNullOrEmpty(data.heldObjectID) &&
                            savedData.uniqueID == data.heldObjectID &&
                            player != null)
                        {
                            pickable.PickUp(player.transform);
                            player.heldObject = pickable.GetComponent<Rigidbody>();
                        }
                    }
                }
            }

            // Registry
            if (PickupRegistry.Instance != null && data.registryData != null)
                PickupRegistry.Instance.LoadFromSaveData(data.registryData);

            Debug.Log($"[SaveLoadSystem] Slot {slot} loaded ← {savePath}");
            Debug.Log($"Loaded {data.items.Count} items, Score: {data.score}, Timestamp: {data.saveTimestamp}");
        }
        catch (Exception e)
        {
            Debug.LogError($"[SaveLoadSystem] Failed to load slot {slot}: {e.Message}");
        }
    }

    // ──────────────────────────────────────────────
    //  Slot Queries
    // ──────────────────────────────────────────────

    public bool SaveExists(int slot) => File.Exists(GetSavePath(ClampSlot(slot)));

    /// <summary>Returns the timestamp string stored in a slot, or null if no save exists.</summary>
    public string GetSlotTimestamp(int slot)
    {
        slot = ClampSlot(slot);
        string path = GetSavePath(slot);
        if (!File.Exists(path)) return null;

        try
        {
            string json = File.ReadAllText(path);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            return data?.saveTimestamp;
        }
        catch
        {
            return null;
        }
    }

    public void DeleteSave(int slot)
    {
        string path = GetSavePath(ClampSlot(slot));
        if (File.Exists(path))
        {
            File.Delete(path);
            Debug.Log($"[SaveLoadSystem] Slot {slot} save file deleted.");
        }
    }

    // ──────────────────────────────────────────────
    //  Legacy / Compat helpers (used by PauseManager)
    // ──────────────────────────────────────────────

    /// <summary>Saves to the current slot. Kept for backward compatibility.</summary>
    public void SaveGame() => SaveGame(CurrentSlot);

    /// <summary>Loads from the current slot. Kept for backward compatibility.</summary>
    public void LoadGame() => LoadGame(CurrentSlot);

    /// <summary>Returns true if the current slot has a save file.</summary>
    public bool SaveExists() => SaveExists(CurrentSlot);

    /// <summary>Deletes the current slot's save file.</summary>
    public void DeleteSave() => DeleteSave(CurrentSlot);

    /// <summary>
    /// DEBUG: Deletes ALL slot save files and resets all runtime save-related state.
    /// Bound to F10 in Update.
    /// </summary>
    public void DeleteAllSaveData()
    {
        for (int i = 1; i <= SlotCount; i++)
            DeleteSave(i);

        GameManager.Score = 0;
        GameManager.AltCoinsScore = 0;

        if (UIManager.Instance != null && UIManager.Instance.CoinsUI != null)
            UIManager.Instance.CoinsUI.UpdateCoins(0);

        if (PickupRegistry.Instance != null)
            PickupRegistry.Instance.ResetAllStates();

        Pickable[] allPickables = FindObjectsOfType<Pickable>();
        foreach (Pickable p in allPickables)
            p.HasBeenPickedUp = false;

        Debug.Log("<color=red>[DEBUG] All save data deleted and runtime state reset.</color>");
    }

    public string GetSaveFilePath() => GetSavePath(CurrentSlot);

    // ──────────────────────────────────────────────
    //  Private Helpers
    // ──────────────────────────────────────────────

    private static int ClampSlot(int slot) => Mathf.Clamp(slot, 1, SlotCount);
}
