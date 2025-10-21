using UnityEngine;
using System.Collections;

public class CollectiblePopupManager : MonoBehaviour
{
    public static CollectiblePopupManager Instance { get; private set; }

    [Header("Popup Settings")]
    [SerializeField] private GameObject _popupPrefab;
    private Transform _popupParent;
    private Camera _mainCamera;

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

        Initialize();
    }

    private void Initialize()
    {
        _mainCamera = Camera.main;

        // Find or create canvas
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas != null)
        {
            _popupParent = canvas.transform;
        }
        else
        {
            Debug.LogWarning("No Canvas found in scene for popup display!");
        }
    }

    public static void ShowPopup(Vector3 worldPosition, int pointValue, GameObject popupPrefab)
    {
        // Auto-create manager if it doesn't exist
        if (Instance == null)
        {
            GameObject managerObj = new GameObject("CollectiblePopupManager");
            Instance = managerObj.AddComponent<CollectiblePopupManager>();
        }

        if (popupPrefab == null || Instance._mainCamera == null || Instance._popupParent == null)
        {
            return;
        }

        // Convert world position to screen position
        Vector3 screenPos = Instance._mainCamera.WorldToScreenPoint(worldPosition);

        // Instantiate popup
        GameObject popup = Instantiate(popupPrefab, Instance._popupParent);
        CollectiblePopup popupScript = popup.GetComponent<CollectiblePopup>();

        if (popupScript != null)
        {
            popupScript.Initialize(screenPos, pointValue);
        }
    }
}
