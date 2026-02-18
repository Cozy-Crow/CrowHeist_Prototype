using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrinketReader : MonoBehaviour
{
    [SerializeField] private ItemDataSO[] trinketSOArray;
    [SerializeField] private TextAsset trinketCSV;

    [SerializeField] private string trinketAssetFolder = "Assets/Data/Trinkets";

    private Dictionary<string, ItemDataSO> trinketDict = new();
    public Dictionary<string, ItemDataSO> TrinketDict => trinketDict;

    public void UpdateTrinketData()
    {
        //Clear existing dictionary incase of cache error
        trinketDict.Clear();

        //Load existing trinkets into dictionary
        for(int i = 0; i < trinketSOArray.Length; i++)
        {
            trinketDict.Add(trinketSOArray[i].ItemName, trinketSOArray[i]);
        }

        //Read from CSV
        string[] trinketArray = trinketCSV.text.Split('\n');
        trinketSOArray = new ItemDataSO[trinketArray.Length - 1];

        //Skip first line (header)
        for (int i = 1; i < trinketArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(trinketArray[i]))
                continue;

            string[] data = trinketArray[i].Split('\t');
            string trinketName = data[0].Trim();
            string description = data[1].Trim('"');
            string locationHint = data[2].Trim('"');

            if (trinketDict.TryGetValue(trinketName, out ItemDataSO existing))
            {
                existing.SetItemName(trinketName);
                existing.SetDescription(description);
                existing.LocationHint = locationHint;
                trinketSOArray[i - 1] = existing;
                continue;
            }

            ItemDataSO newTrinket = CreateItemDataSO(trinketName, description, locationHint);
            trinketDict.Add(trinketName, newTrinket);
            trinketSOArray[i - 1] = newTrinket;
        }

        Debug.Log($"Trinket data updated. Total trinkets: {trinketDict.Count}");

        #if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        #endif
    }

    private ItemDataSO CreateItemDataSO(string trinketName, string description, string locationHint)
    {
        ItemDataSO newItem = ScriptableObject.CreateInstance<ItemDataSO>();
        Debug.Log($"Creating new trinket without spaces: {trinketName.Replace(" ", "")}");

        string cleanName = trinketName.Replace(" ", "");

        newItem.name = cleanName;
        newItem.SetItemName(trinketName);
        newItem.SetDescription(description);
        newItem.SetItemType(ObjectType.Narrative);
        newItem.LocationHint = locationHint;
        newItem.ResetReadState();

        #if UNITY_EDITOR
        EnsureFolderExists(trinketAssetFolder);

        string assetPath = $"{trinketAssetFolder}/{cleanName}.asset";
        AssetDatabase.CreateAsset(newItem, assetPath);
        #endif

        return newItem;
    }

    #if UNITY_EDITOR
    private void EnsureFolderExists(string path)
    {
        if (AssetDatabase.IsValidFolder(path))
            return;

        string parent = "Assets";
        string[] folders = path.Replace("Assets/", "").Split('/');

        foreach (string folder in folders)
        {
            if (!AssetDatabase.IsValidFolder($"{parent}/{folder}"))
            {
                AssetDatabase.CreateFolder(parent, folder);
            }
            parent += "/" + folder;
        }
    }
    #endif

    public void UnlockAll()
    {
        if (trinketSOArray == null || trinketSOArray.Length == 0) return;

        foreach (var trinket in trinketSOArray)
        {
            trinket.MarkAsRead();
        }
    }

    public void LockAll()
    {
        if (trinketSOArray == null || trinketSOArray.Length == 0) return;

        foreach (var trinket in trinketSOArray)
        {
            trinket.ResetReadState();
        }
    }

    public void LoadAllTrinketsFromPath()
    {
        string resourcesPath = trinketAssetFolder
            .Replace("Assets/Resources/", "")
            .Replace(".asset", "");
        trinketSOArray = Resources.LoadAll<ItemDataSO>(resourcesPath);
    }

    public void DeleteAllTrinketsFromPath()
    {
        string resourcesPath = trinketAssetFolder
            .Replace("Assets/Resources/", "")
            .Replace(".asset", "");
        trinketSOArray = Resources.LoadAll<ItemDataSO>(resourcesPath);

        foreach (var trinket in trinketSOArray)
        {
            #if UNITY_EDITOR
            string assetPath = AssetDatabase.GetAssetPath(trinket);
            AssetDatabase.DeleteAsset(assetPath);
            #endif
        }

        trinketSOArray = new ItemDataSO[0];
    }
}
