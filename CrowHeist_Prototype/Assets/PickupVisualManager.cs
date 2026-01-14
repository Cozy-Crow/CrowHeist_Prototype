using System.Collections;
using UnityEngine;
using TMPro;

public class PickupVisualManager : MonoBehaviour
{
    public static PickupVisualManager Instance;

    [Header("UI")]
    [SerializeField] private RectTransform panel;
    [SerializeField] private TMP_Text itemName;
    [SerializeField] private TMP_Text itemDescription;

    [Header("Model Display")]
    [SerializeField] private Transform modelAnchor;

    [Header("Animation")]
    [SerializeField] private float slideDuration = 0.5f;
    [SerializeField] private float centerHoldTime = 2f;
    [SerializeField] private Vector2 offscreenRightPos;
    [SerializeField] private Vector2 centerPos;

    [Header("Pickup Data")]
    [SerializeField] private PickupVisualData[] pickupData;

    private GameObject currentModel;


    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        panel.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (currentModel != null)
        {
            currentModel.transform.Rotate(Vector3.up, 60f * Time.deltaTime, Space.Self);
        }
    }


    public void PlayFirstPickupAnim(GameObject item)
    {
        StopAllCoroutines();
        StartCoroutine(PickupRoutine(item));
    }


    private IEnumerator PickupRoutine(GameObject item)
    {
        // 1. Unhide but offscreen
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = offscreenRightPos;

        // 2. Set name & description
        PickupVisualData data = GetDataForItem(item);
        if (data != null)
        {
            itemName.text = data.displayName;
            itemDescription.text = data.description;
        }
        else
        {
            itemName.text = item.name;
            itemDescription.text = "";
        }

        // 3. Animate to center
        yield return MovePanel(offscreenRightPos, centerPos);

        // 4. Spawn model
        SpawnModel(item);

        // 5. Hold at center
        yield return new WaitForSeconds(centerHoldTime);

        // 6. Animate off screen
        yield return MovePanel(centerPos, offscreenRightPos);

        // 7. Cleanup & hide
        ClearModel();
        panel.gameObject.SetActive(false);
    }


    private IEnumerator MovePanel(Vector2 from, Vector2 to)
    {
        float t = 0f;

        while (t < slideDuration)
        {
            t += Time.deltaTime;
            panel.anchoredPosition = Vector2.Lerp(from, to, t / slideDuration);
            yield return null;
        }

        panel.anchoredPosition = to;
    }
    private void SpawnModel(GameObject item)
    {
        ClearModel();

        GameObject container = new GameObject("PreviewContainer");

        
        container.transform.SetParent(modelAnchor, false);

        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        GameObject model = Instantiate(item);
        model.transform.SetParent(container.transform, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        foreach (Collider col in model.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Rigidbody rb in model.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        container.transform.localScale = Vector3.one * 0.5f;

        currentModel = container;
    }

    private void ClearModel()
    {
        if (currentModel != null)
        {
            Destroy(currentModel);
            currentModel = null;
        }
    }

    private PickupVisualData GetDataForItem(GameObject item)
    {
        foreach (PickupVisualData data in pickupData)
        {
            if (item.CompareTag(data.tag))
                return data;
        }

        return null;
    }
}

// Data Class

[System.Serializable]
public class PickupVisualData
{
    public string tag;
    public string displayName;

    [TextArea]
    public string description;
}
