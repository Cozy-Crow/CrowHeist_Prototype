using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class TrinketReader : MonoBehaviour
{
    [SerializeField] private TrinketsSO[] trinketSOArray;
    [SerializeField] private TextAsset trinketCSV;

    [SerializeField] private string trinketAssetFolder = "Assets/Data/Trinkets";

    private Dictionary<string, TrinketsSO> trinketDict = new();
    public Dictionary<string, TrinketsSO> TrinketDict => trinketDict;

    public void UpdateTrinketData()
    {
        //Clear existing dictionary incase of cache error
        trinketDict.Clear();

        //Load existing trinkets into dictionary
        for(int i = 0; i < trinketSOArray.Length; i++)
        {
            trinketDict.Add(trinketSOArray[i].TrinketName, trinketSOArray[i]);
        }

        //Read from CSV
        string[] trinketArray = trinketCSV.text.Split('\n');

        //Skip first line (header)
        for (int i = 1; i < trinketArray.Length; i++)
        {
            if (string.IsNullOrWhiteSpace(trinketArray[i]))
                continue;

            string[] data = trinketArray[i].Split(',');

            string trinketName = data[0].Trim();
            string description = data[1].Trim();

            if (trinketDict.TryGetValue(trinketName, out TrinketsSO existing))
            {
                existing.TrinketName = trinketName;
                existing.Description = description;
                continue;
            }

            TrinketsSO newTrinket = CreateTrinketSO(trinketName, description);
            trinketDict.Add(trinketName, newTrinket);
        }

#if UNITY_EDITOR
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#endif
    }

    private TrinketsSO CreateTrinketSO(string trinketName, string description)
    {
        TrinketsSO newTrinket = ScriptableObject.CreateInstance<TrinketsSO>();
        newTrinket.name = trinketName;
        newTrinket.TrinketName = trinketName;
        newTrinket.Description = description;
        newTrinket.isUnlocked = false;

#if UNITY_EDITOR
        EnsureFolderExists(trinketAssetFolder);

        string assetPath = $"{trinketAssetFolder}/{trinketName}.asset";
        AssetDatabase.CreateAsset(newTrinket, assetPath);
#endif

        return newTrinket;
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
            trinket.isUnlocked = true;
        }
    }

    public void LockAll()
    {
        if (trinketSOArray == null || trinketSOArray.Length == 0) return;
        
        foreach (var trinket in trinketSOArray)
        {
            trinket.isUnlocked = false;
        }
    }
}
