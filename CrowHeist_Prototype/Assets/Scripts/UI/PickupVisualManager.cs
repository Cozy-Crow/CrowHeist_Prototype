using System.Collections;
using System.Collections.Generic;
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

    [Header("Pickup Data (Legacy - Tag Based)")]
    [SerializeField] private PickupVisualData[] pickupData;

    private GameObject currentModel;
    private ItemDataSO currentItemData;
    private HashSet<string> shownItems = new HashSet<string>();


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
        if (item.GetComponent<Trinket>() != null) return;

        string key = CleanItemName(item.name);
        if (shownItems.Contains(key)) return;
        shownItems.Add(key);

        StopAllCoroutines();
        StartCoroutine(PickupRoutine(item));
    }

    public void ResetShownItems()
    {
        shownItems.Clear();
    }

    private IEnumerator PickupRoutine(GameObject item)
    {
        panel.gameObject.SetActive(true);
        panel.anchoredPosition = offscreenRightPos;

        float holdTime = centerHoldTime;
        currentItemData = GetItemDataSO(item);

        GameObject previewSource = null;
        if (currentItemData != null && currentItemData.PreviewModelPrefab != null)
            previewSource = currentItemData.PreviewModelPrefab;
        else
            previewSource = item;

        if (currentItemData != null)
        {
            itemName.text = currentItemData.ItemName;
            itemDescription.text = currentItemData.Description;
            holdTime = currentItemData.DisplayDuration;
        }
        else
        {
            PickupVisualData data = GetDataForItem(item);
            if (data != null)
            {
                itemName.text = data.displayName;
                itemDescription.text = data.description;
            }
            else
            {
                itemName.text = CleanItemName(item.name);
                itemDescription.text = "";
            }
        }

        yield return MovePanel(offscreenRightPos, centerPos);

        SpawnModelFromSource(previewSource);

        yield return new WaitForSeconds(holdTime);

        yield return MovePanel(centerPos, offscreenRightPos);

        ClearModel();
        currentItemData = null;
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

    private void SpawnModelFromSource(GameObject modelSource)
    {
        ClearModel();

        if (modelSource == null) return;

        GameObject container = new GameObject("PreviewContainer");
        container.transform.SetParent(modelAnchor, false);
        container.transform.localPosition = Vector3.zero;
        container.transform.localRotation = Quaternion.identity;
        container.transform.localScale = Vector3.one;

        GameObject model = Instantiate(modelSource);
        model.transform.SetParent(container.transform, false);
        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        model.transform.localScale = Vector3.one;

        foreach (Collider col in model.GetComponentsInChildren<Collider>())
            col.enabled = false;

        foreach (Rigidbody rb in model.GetComponentsInChildren<Rigidbody>())
            Destroy(rb);

        foreach (MonoBehaviour script in model.GetComponentsInChildren<MonoBehaviour>())
        {
            if (!(script is Transform))
                script.enabled = false;
        }

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

    private ItemDataSO GetItemDataSO(GameObject item)
    {
        UniqueID uniqueID = item.GetComponent<UniqueID>();
        if (uniqueID != null && uniqueID.ItemData != null)
        {
            return uniqueID.ItemData;
        }
        return null;
    }

    private string CleanItemName(string rawName)
    {
        string name = rawName.Replace("(Clone)", "");
        return System.Text.RegularExpressions.Regex.Replace(name, @"\s*\d+$", "").Trim();
    }
}

[System.Serializable]
public class PickupVisualData
{
    public string tag;
    public string displayName;

    [TextArea]
    public string description;
}