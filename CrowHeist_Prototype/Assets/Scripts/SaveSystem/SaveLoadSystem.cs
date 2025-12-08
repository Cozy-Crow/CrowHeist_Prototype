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
        Debug.Log("=== STARTING SAVE ===");

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
            Debug.Log($"[SAVE] Player position: {saveData.playerData.position.ToVector3()}");

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

            // ADDED: Save player velocity
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                saveData.playerData.velocity = playerRb.velocity;
                Debug.Log($"[SAVE] Player velocity: {playerRb.velocity}");
            }
        }

        // Save all saveable objects in the scene (including inactive ones like collected coins)
        SaveableObject[] saveableObjects = FindObjectsOfType<SaveableObject>(true);
        Debug.Log($"[SAVE] Found {saveableObjects.Length} total saveable objects");

        int activeCount = 0;
        int movingCount = 0;
        foreach (SaveableObject obj in saveableObjects)
        {
            SaveableObjectData objectData = obj.GetSaveData();
            saveData.saveableObjects.Add(objectData);

            if (objectData.isActive) activeCount++;
            if (objectData.hadVelocity) movingCount++;
        }

        Debug.Log($"[SAVE] Active objects: {activeCount}, Moving objects: {movingCount}");

        // Convert to JSON and save to file
        string json = JsonUtility.ToJson(saveData, true);

        try
        {
            File.WriteAllText(_saveFilePath, json);
            Debug.Log($"=== SAVE COMPLETE === ({saveData.saveableObjects.Count} objects saved)");
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

        Debug.Log("=== STARTING LOAD ===");

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
        Debug.Log("=== RESTORING GAME STATE ===");

       
        ClearPlayerEffectsImmediate();

        // Stop any coroutines on PixieDust objects themselves
        ClearAllPixieDustEffects();

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
            Debug.Log($"[LOAD] Restoring player to position: {saveData.playerData.position.ToVector3()}");

            // Get components
            var controller = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            var kinematicMotor = player.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();
            Rigidbody playerRb = player.GetComponent<Rigidbody>();

            // Disable controller temporarily to prevent position override
            bool motorWasEnabled = false;
            if (kinematicMotor != null)
            {
                motorWasEnabled = kinematicMotor.enabled;
                kinematicMotor.enabled = false;
                Debug.Log("[LOAD] Disabled KinematicCharacterMotor");
            }

            // Reset physics
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
                Debug.Log("[LOAD] Reset player physics");
            }

            // Set position using motor if available
            if (kinematicMotor != null)
            {
                kinematicMotor.SetPosition(saveData.playerData.position.ToVector3());
                kinematicMotor.SetRotation(Quaternion.Euler(saveData.playerData.rotation.ToVector3()));
                Debug.Log($"[LOAD] Set position via motor: {saveData.playerData.position.ToVector3()}");
            }
            else
            {
                player.transform.position = saveData.playerData.position.ToVector3();
                player.transform.rotation = Quaternion.Euler(saveData.playerData.rotation.ToVector3());
                Debug.Log($"[LOAD] Set position via transform: {saveData.playerData.position.ToVector3()}");
            }

            // Restore controller state
            if (controller != null)
            {
                controller._isFacingRight = saveData.playerData.isFacingRight;
                controller._isDirty = saveData.playerData.isDirty;

              
                if (string.IsNullOrEmpty(saveData.playerData.heldObjectId))
                {
                    if (controller.heldObject != null)
                    {
                        Debug.Log($"[LOAD] 🧹 CLEARING held object BEFORE restore. Was holding: {controller.heldObject.name}");

                        // Try to drop the object properly
                        IPickupable pickupable = controller.heldObject.GetComponent<IPickupable>();
                        if (pickupable != null)
                        {
                            Vector3 dropPosition = player.transform.position + player.transform.forward * 0.5f;
                            pickupable.Drop(dropPosition);
                            Debug.Log($"[LOAD] Dropped {controller.heldObject.name}");
                        }

                        // Clear the reference
                        controller.heldObject = null;
                        Debug.Log($"[LOAD] ✅ controller.heldObject = null");
                    }
                    else
                    {
                        Debug.Log($"[LOAD] ℹ️ No held object to clear (already null)");
                    }

                    // EXTRA SAFETY: Verify it's really null after a frame
                    StartCoroutine(VerifyHeldObjectCleared(controller));
                }
            }

            // Re-enable motor and restore velocity
            StartCoroutine(RestorePlayerAfterFrame(kinematicMotor, motorWasEnabled, playerRb, saveData.playerData.velocity.ToVector3()));
        }


        // First, collect all existing saveable objects (including inactive ones so we can reactivate them)
        SaveableObject[] existingObjects = FindObjectsOfType<SaveableObject>(true);
        Dictionary<string, SaveableObject> existingObjectsDict = new Dictionary<string, SaveableObject>();

        foreach (SaveableObject obj in existingObjects)
        {
            existingObjectsDict[obj.UniqueId] = obj;
        }

        Debug.Log($"[LOAD] Found {existingObjects.Length} existing saveable objects in scene");

        // Restore saved objects
        int restoredCount = 0;
        int destroyedCount = 0;

        foreach (SaveableObjectData objectData in saveData.saveableObjects)
        {
            if (existingObjectsDict.ContainsKey(objectData.uniqueId))
            {
                // Object exists, restore its state
                SaveableObject existingObj = existingObjectsDict[objectData.uniqueId];
                existingObj.LoadData(objectData);
                existingObjectsDict.Remove(objectData.uniqueId);
                restoredCount++;
            }
            else
            {
                // Object doesn't exist - was collected/destroyed
                Debug.Log($"Object {objectData.uniqueId} not found - staying destroyed");
                destroyedCount++;
            }
        }

        Debug.Log($"[LOAD] Restored {restoredCount} objects, {destroyedCount} stayed destroyed");

        // Destroy objects that weren't in the save file (they were collected/destroyed)
        foreach (var kvp in existingObjectsDict)
        {
            Debug.Log($"Destroying object {kvp.Key} - not in save file");
            Destroy(kvp.Value.gameObject);
        }

        // Restore held object if save had one
        if (!string.IsNullOrEmpty(saveData.playerData.heldObjectId) && player != null)
        {
            var controller = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (controller != null)
            {
                Debug.Log($"[LOAD] Restoring held object: {saveData.playerData.heldObjectId}");
                StartCoroutine(RestoreHeldObject(saveData.playerData.heldObjectId, controller));
            }
        }

        Debug.Log("=== LOAD COMPLETE ===");
    }

    private IEnumerator VerifyHeldObjectCleared(KinematicCharacterController.Examples.Controller2Point5D controller)
    {
        // Wait for a few frames to see if something sets heldObject back
        yield return new WaitForEndOfFrame();
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        if (controller.heldObject != null)
        {
           
            // Force clear it again
            controller.heldObject = null;
           
        }
        
    }

    private IEnumerator RestorePlayerAfterFrame(KinematicCharacterController.KinematicCharacterMotor motor, bool wasEnabled, Rigidbody rb, Vector3 velocity)
    {
        yield return new WaitForFixedUpdate();

        // Re-enable motor
        if (motor != null)
        {
            motor.enabled = wasEnabled;
            Debug.Log($"[LOAD] Re-enabled KinematicCharacterMotor, final position: {motor.transform.position}");
        }

        // Restore velocity
        if (rb != null && velocity.magnitude > 0.01f)
        {
            rb.velocity = velocity;
            Debug.Log($"[LOAD] Restored player velocity: {velocity}");
        }

        // CRITICAL: Wait additional frames for physics to settle, then clear any lingering effects
        yield return new WaitForFixedUpdate();
        yield return new WaitForFixedUpdate();

        // Clear effects and restore to the SAVED velocity (not current)
        ClearPlayerEffects(velocity);
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


    /// <summary>
    /// Immediately clears any active effects on the player (called at START of load)
    /// </summary>
    private void ClearPlayerEffectsImmediate()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("[SaveLoadSystem] 🧹 Clearing player effects IMMEDIATELY...");

            // Stop all coroutines on player that might be from effects
            var playerController = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (playerController != null)
            {
                // Stop any levitation or effect coroutines
                playerController.StopAllCoroutines();
                Debug.Log("[SaveLoadSystem] Stopped ALL player coroutines");
            }

            // CRITICAL: Clear KinematicCharacterMotor BaseVelocity (PixieDust uses this!)
            var motor = player.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();
            if (motor != null)
            {
                Vector3 oldMotorVelocity = motor.BaseVelocity;
                motor.BaseVelocity = Vector3.zero;
                Debug.Log($"[SaveLoadSystem] Cleared motor BaseVelocity: {oldMotorVelocity} → 0");
            }

            // SUPER AGGRESSIVE: Reset Rigidbody velocity AND accumulated forces
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                Vector3 oldVelocity = playerRb.velocity;

                // Zero out all physics
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;

                // CRITICAL: Clear accumulated forces by resetting physics state
                playerRb.Sleep();  // Put to sleep to clear forces
                playerRb.WakeUp(); // Wake up for next frame

                // Force velocity to zero again after wake
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;

                Debug.Log($"[SaveLoadSystem] Cleared Rigidbody velocity: {oldVelocity} → 0");
            }

            Debug.Log("[SaveLoadSystem]  Player effects cleared IMMEDIATELY");
        }
        else
        {
            Debug.LogWarning("[SaveLoadSystem] Could not find player to clear effects");
        }
    }

    private void ClearAllPixieDustEffects()
    {
        // Find all PixieDust objects in the scene (including inactive ones)
        PixieDust[] allPixieDust = FindObjectsOfType<PixieDust>(true);

        if (allPixieDust.Length > 0)
        {
            Debug.Log($"[SaveLoadSystem] Found {allPixieDust.Length} PixieDust objects, stopping their coroutines...");

            foreach (PixieDust pixie in allPixieDust)
            {
                if (pixie != null)
                {
                    // Stop all coroutines on this PixieDust object
                    pixie.StopAllCoroutines();
                }
            }

            Debug.Log("[SaveLoadSystem] ✅ All PixieDust effects cleared");
        }
    }

    
    private void ClearPlayerEffects(Vector3 savedVelocity)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Debug.Log("[SaveLoadSystem] Clearing player effects...");

            // Stop all coroutines on player that might be from effects
            var playerController = player.GetComponent<KinematicCharacterController.Examples.Controller2Point5D>();
            if (playerController != null)
            {
                // Stop any levitation or effect coroutines
                playerController.StopAllCoroutines();
                Debug.Log("[SaveLoadSystem] Stopped player coroutines");
            }

            // CRITICAL: Clear KinematicCharacterMotor BaseVelocity (PixieDust uses this!)
            var motor = player.GetComponent<KinematicCharacterController.KinematicCharacterMotor>();
            if (motor != null)
            {
                Vector3 oldMotorVelocity = motor.BaseVelocity;
                motor.BaseVelocity = Vector3.zero;
                Debug.Log($"[SaveLoadSystem] Cleared motor BaseVelocity (was: {oldMotorVelocity})");
            }

            
            Rigidbody playerRb = player.GetComponent<Rigidbody>();
            if (playerRb != null)
            {
                playerRb.velocity = Vector3.zero;
                playerRb.angularVelocity = Vector3.zero;
                playerRb.Sleep();
                playerRb.WakeUp(); 
                playerRb.velocity = savedVelocity;
            }
        }
    }
}
