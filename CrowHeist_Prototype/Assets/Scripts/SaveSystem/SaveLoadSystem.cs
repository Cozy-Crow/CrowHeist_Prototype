using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;


public class SaveLoadSystem : MonoBehaviour
{
    public static SaveLoadSystem Instance { get; private set; }

    private string _saveFilePath;
    private const string SAVE_FILE_NAME = "savegame.json";

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _saveFilePath = Path.Combine(Application.persistentDataPath, SAVE_FILE_NAME);
            Debug.Log($"Save file path: {_saveFilePath}");
        }
        else
        {
            Destroy(gameObject);
        }
    }


    public void SaveGame()
    {
        Debug.Log("Saving game...");

        SaveData saveData = new SaveData();

        // Save game state
        saveData.score = GameManager.Score;
        saveData.altCoinsScore = GameManager.AltCoinsScore;
        saveData.currentSceneName = SceneManager.GetActiveScene().name;

        // Save player data
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            saveData.playerData.position = player.transform.position;
            saveData.playerData.rotation = player.transform.rotation.eulerAngles;

            var controller = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (controller != null)
            {
                saveData.playerData.isFacingRight = controller._isFacingRight;
                saveData.playerData.isDirty = controller._isDirty;

                // Save held object if any
                if (controller.heldObject != null)
                {
                    var saveableObject = controller.heldObject.GetComponent<SaveableObject>();
                    if (saveableObject != null)
                    {
                        saveData.playerData.heldObjectId = saveableObject.UniqueId;
                    }
                }
            }
        }

        // Save all saveable objects in the scene (including inactive ones like collected coins)
        SaveableObject[] saveableObjects = FindObjectsOfType<SaveableObject>(true);
        foreach (SaveableObject obj in saveableObjects)
        {
            SaveableObjectData objectData = obj.GetSaveData();
            saveData.saveableObjects.Add(objectData);
        }

        // Convert to JSON and save to file
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(_saveFilePath, json);
            Debug.Log($"Game saved successfully! ({saveData.saveableObjects.Count} objects saved)");
            Debug.Log($"Save location: {_saveFilePath}");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to save game: {e.Message}");
        }
    }


    public void LoadGame()
    {
        if (!File.Exists(_saveFilePath))
        {
            Debug.LogWarning("No save file found!");
            return;
        }

        Debug.Log("Loading game...");

        try
        {
            string json = File.ReadAllText(_saveFilePath);
            SaveData saveData = JsonUtility.FromJson<SaveData>(json);

            if (saveData == null)
            {
                Debug.LogError("Failed to parse save data!");
                return;
            }

            // Load the scene if different
            string currentScene = SceneManager.GetActiveScene().name;
            if (currentScene != saveData.currentSceneName)
            {
                Debug.Log($"Loading scene: {saveData.currentSceneName}");
                SceneManager.sceneLoaded += OnSceneLoadedForRestore;
                _pendingSaveData = saveData;
                SceneManager.LoadScene(saveData.currentSceneName);
            }
            else
            {
                // Same scene, restore immediately
                RestoreGameState(saveData);
            }
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Failed to load game: {e.Message}");
        }
    }

    private SaveData _pendingSaveData;

    private void OnSceneLoadedForRestore(Scene scene, LoadSceneMode mode)
    {
        SceneManager.sceneLoaded -= OnSceneLoadedForRestore;

        if (_pendingSaveData != null)
        {
            // Wait one frame for scene to fully initialize
            StartCoroutine(RestoreAfterSceneLoad(_pendingSaveData));
            _pendingSaveData = null;
        }
    }

    private IEnumerator RestoreAfterSceneLoad(SaveData saveData)
    {
        yield return new WaitForEndOfFrame();
        RestoreGameState(saveData);
    }


    private void RestoreGameState(SaveData saveData)
    {
        Debug.Log("Restoring game state...");

        // Restore game scores
        GameManager.Score = saveData.score;
        GameManager.AltCoinsScore = saveData.altCoinsScore;

        // Update UI
        if (UIManager.Instance != null && UIManager.Instance.CoinsUI != null)
        {
            UIManager.Instance.CoinsUI.UpdateCoins(GameManager.Score);
        }

        // Restore player
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            player.transform.position = saveData.playerData.position.ToVector3();
            player.transform.rotation = Quaternion.Euler(saveData.playerData.rotation.ToVector3());

            var controller = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (controller != null)
            {
                controller._isFacingRight = saveData.playerData.isFacingRight;
                controller._isDirty = saveData.playerData.isDirty;
            }
        }

        // First, collect all existing saveable objects (including inactive ones so we can reactivate them)
        SaveableObject[] existingObjects = FindObjectsOfType<SaveableObject>(true);
        Dictionary<string, SaveableObject> existingObjectsDict = new Dictionary<string, SaveableObject>();

        foreach (SaveableObject obj in existingObjects)
        {
            existingObjectsDict[obj.UniqueId] = obj;
        }

        // Restore saved objects
        foreach (SaveableObjectData objectData in saveData.saveableObjects)
        {
            if (existingObjectsDict.ContainsKey(objectData.uniqueId))
            {
                // Object exists, restore its state
                SaveableObject existingObj = existingObjectsDict[objectData.uniqueId];
                existingObj.LoadData(objectData);
                existingObjectsDict.Remove(objectData.uniqueId);
            }
            else
            {
                // Object doesn't exist - was collected/destroyed
                Debug.Log($"Object {objectData.uniqueId} not found - staying destroyed");
            }
        }

        // Destroy objects that weren't in the save file (they were collected/destroyed)
        foreach (var kvp in existingObjectsDict)
        {
            Debug.Log($"Destroying object {kvp.Key} - not in save file");
            Destroy(kvp.Value.gameObject);
        }

        // Restore held object if any
        if (!string.IsNullOrEmpty(saveData.playerData.heldObjectId) && player != null)
        {
            var controller = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (controller != null)
            {
                StartCoroutine(RestoreHeldObject(saveData.playerData.heldObjectId, controller));
            }
        }

        Debug.Log($"Game loaded successfully! ({saveData.saveableObjects.Count} objects restored)");
    }

    private IEnumerator RestoreHeldObject(string objectId, KinematicCharacterController.Examples.Controller2Point5D controller)
    {
        // Wait a frame for objects to be fully initialized
        yield return new WaitForEndOfFrame();

        SaveableObject[] objects = FindObjectsOfType<SaveableObject>(true);
        foreach (SaveableObject obj in objects)
        {
            if (obj.UniqueId == objectId)
            {
                IPickupable pickupable = obj.GetComponent<IPickupable>();
                if (pickupable != null)
                {
                    // Simulate picking up the object
                    pickupable.PickUP(controller.transform.Find("Head"));
                    controller.heldObject = obj.GetComponent<Rigidbody>();
                }
                break;
            }
        }
    }


    public bool SaveFileExists()
    {
        return File.Exists(_saveFilePath);
    }


    public void DeleteSave()
    {
        if (File.Exists(_saveFilePath))
        {
            File.Delete(_saveFilePath);
            Debug.Log("Save file deleted");
        }
    }
}
