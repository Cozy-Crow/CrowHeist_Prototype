using KinematicCharacterController.Examples;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

    [Header("Save Settings")]
    [SerializeField] private string saveFileName = "gamesave.json";

    [Header("References")]
    [SerializeField] private Controller2Point5D player;
    [SerializeField] private RoombAi roomba;

    private string SavePath => Path.Combine(Application.persistentDataPath, saveFileName);

    private void Awake()
    {
        // Singleton pattern
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
        // Auto-find references if not set
        if (player == null)
        {
            player = FindObjectOfType<Controller2Point5D>();
        }

        if (roomba == null)
        {
            roomba = FindObjectOfType<RoombAi>();
        }
    }

    private void Update()
    {
        // Manual save/load with keyboard shortcuts
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SaveGame();
        }

        if (Input.GetKeyDown(KeyCode.F9))
        {
            LoadGame();
        }
    }

    public void SaveGame()
    {
        try
        {
            SaveData data = new SaveData();

            // Save player data
            if (player != null)
            {
                data.playerPosition = new Vector3Data(player.transform.position);
                data.playerRotation = new QuaternionData(player.transform.rotation);
                data.playerIsDirty = player.isDirty;

                // Save held object ID if player is holding something
                if (player.heldObject != null)
                {
                    // heldObject is a Rigidbody, so get UniqueID from its GameObject
                    UniqueID heldID = player.heldObject.gameObject.GetComponent<UniqueID>();
                    if (heldID != null)
                    {
                        data.heldObjectID = heldID.ID;
                    }
                    else
                    {
                        data.heldObjectID = string.Empty;
                    }
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

            // Save score data
            data.score = GameManager.Score;
            data.altCoinsScore = GameManager.AltCoinsScore;

            // Save roomba data
            if (roomba != null)
            {
                data.roombaData = new RoombaData
                {
                    position = new Vector3Data(roomba.transform.position),
                    isActivated = roomba.isActivated,
                    isBroken = roomba.isBroken
                    // Note: isDocked is private in RoombAi and cannot be accessed
                };
            }
            else
            {
                Debug.LogWarning("SaveLoadSystem: Roomba reference is null!");
                data.roombaData = new RoombaData();
            }

            // Save all items with UniqueID component
            UniqueID[] allItems = FindObjectsOfType<UniqueID>();
            foreach (UniqueID item in allItems)
            {
                // Don't save the player or roomba as items
                if (item.GetComponent<Controller2Point5D>() != null ||
                    item.GetComponent<RoombAi>() != null)
                {
                    continue;
                }

                Pickable pickable = item.GetComponent<Pickable>();
                bool isDirty = false;
                bool isHeld = false;

                if (pickable != null)
                {
                    isDirty = pickable._isDirty;
                    isHeld = pickable.pickedUp;
                }

                ItemData itemData = new ItemData(
                    item.ID,
                    item.gameObject.name.Replace("(Clone)", "").Trim(),
                    item.transform.position,
                    item.transform.eulerAngles,
                    isDirty,
                    isHeld,
                    item.gameObject.activeSelf
                );

                data.items.Add(itemData);
            }

            // Add timestamp
            data.saveTimestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

            // Convert to JSON and save
            string json = JsonUtility.ToJson(data, true);
            File.WriteAllText(SavePath, json);

            Debug.Log($"Game saved successfully to: {SavePath}");
            Debug.Log($"Saved {data.items.Count} items, Score: {data.score}, Player pos: {data.playerPosition.ToVector3()}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }

    public void LoadGame()
    {
        if (!File.Exists(SavePath))
        {
            Debug.LogWarning($"No save file found at: {SavePath}");
            return;
        }

        try
        {
            string json = File.ReadAllText(SavePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);

            if (data == null)
            {
                Debug.LogError("Failed to deserialize save data!");
                return;
            }

            // Load player data
            if (player != null)
            {
                player.transform.position = data.playerPosition.ToVector3();
                player.transform.rotation = data.playerRotation.ToQuaternion();
                player.isDirty = data.playerIsDirty;

                // Clear currently held object if any
                if (player.heldObject != null)
                {
                    // heldObject is a Rigidbody, get Pickable from its GameObject
                    Pickable currentHeld = player.heldObject.gameObject.GetComponent<Pickable>();
                    if (currentHeld != null)
                    {
                        currentHeld.Drop(currentHeld.transform.position);
                    }
                }
            }

            // Load score data
            GameManager.Score = data.score;
            GameManager.AltCoinsScore = data.altCoinsScore;

            // Update UI
            if (UIManager.Instance != null && UIManager.Instance.CoinsUI != null)
            {
                UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
            }

            // Load roomba data
            if (roomba != null && data.roombaData != null)
            {
                roomba.transform.position = data.roombaData.position.ToVector3();
                roomba.isActivated = data.roombaData.isActivated;
                roomba.isBroken = data.roombaData.isBroken;
                // Note: isDocked is private in RoombAi and cannot be set

                // Update roomba state
                if (data.roombaData.isActivated)
                {
                    roomba.Activate();
                }
                else
                {
                    roomba.Deactivate();
                }
            }

            // Load item data
            Dictionary<string, ItemData> itemDataDict = new Dictionary<string, ItemData>();
            foreach (ItemData itemData in data.items)
            {
                itemDataDict[itemData.uniqueID] = itemData;
            }

            // Apply loaded data to items in scene
            UniqueID[] allItems = FindObjectsOfType<UniqueID>();
            foreach (UniqueID item in allItems)
            {
                // Skip player and roomba
                if (item.GetComponent<Controller2Point5D>() != null ||
                    item.GetComponent<RoombAi>() != null)
                {
                    continue;
                }

                if (itemDataDict.TryGetValue(item.ID, out ItemData savedData))
                {
                    // Restore position and rotation
                    item.transform.position = savedData.position.ToVector3();
                    item.transform.eulerAngles = savedData.rotation.ToVector3();
                    item.gameObject.SetActive(savedData.isActive);

                    // Restore pickable state
                    Pickable pickable = item.GetComponent<Pickable>();
                    if (pickable != null)
                    {
                        pickable._isDirty = savedData.isDirty;

                        // If this item should be held by player
                        if (savedData.isHeld && !string.IsNullOrEmpty(data.heldObjectID) &&
                            savedData.uniqueID == data.heldObjectID)
                        {
                            // Player will pick this up
                            if (player != null)
                            {
                                pickable.PickUp(player.transform);
                                player.heldObject = pickable.GetComponent<Rigidbody>();
                            }
                        }
                    }
                }
            }

            Debug.Log($"Game loaded successfully from: {SavePath}");
            Debug.Log($"Loaded {data.items.Count} items, Score: {data.score}, Player pos: {data.playerPosition.ToVector3()}");
            Debug.Log($"Save timestamp: {data.saveTimestamp}");
        }
        catch (Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    public bool SaveExists()
    {
        return File.Exists(SavePath);
    }

    public void DeleteSave()
    {
        if (File.Exists(SavePath))
        {
            File.Delete(SavePath);
            Debug.Log("Save file deleted.");
        }
    }

    public string GetSaveFilePath()
    {
        return SavePath;
    }
}
