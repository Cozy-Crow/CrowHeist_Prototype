using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

/// <summary>
/// Editor utility to fix duplicate UniqueIDs in the scene.
/// Use this if you have multiple objects with the same ID.
/// </summary>
public class FixDuplicateIDs : EditorWindow
{
    [MenuItem("Tools/Save System/Fix Duplicate IDs in Scene")]
    public static void ShowWindow()
    {
        GetWindow<FixDuplicateIDs>("Fix Duplicate IDs");
    }

    private void OnGUI()
    {
        GUILayout.Label("Fix Duplicate UniqueIDs", EditorStyles.boldLabel);
        GUILayout.Space(10);

        GUILayout.Label("This tool will find all UniqueID components in the scene");
        GUILayout.Label("and regenerate IDs for any duplicates.");
        GUILayout.Space(10);

        if (GUILayout.Button("Scan for Duplicates", GUILayout.Height(30)))
        {
            ScanForDuplicates();
        }

        GUILayout.Space(10);

        if (GUILayout.Button("Fix All Duplicate IDs", GUILayout.Height(30)))
        {
            FixAllDuplicates();
        }

        GUILayout.Space(20);
        GUILayout.Label("Quick Actions:", EditorStyles.boldLabel);

        if (GUILayout.Button("Generate IDs for All Selected Objects", GUILayout.Height(25)))
        {
            GenerateIDsForSelection();
        }

        if (GUILayout.Button("Clear All IDs in Scene (Regenerate on Play)", GUILayout.Height(25)))
        {
            ClearAllIDs();
        }
    }

    private void ScanForDuplicates()
    {
        UniqueID[] allIDs = GameObject.FindObjectsOfType<UniqueID>();
        Dictionary<string, List<UniqueID>> idGroups = new Dictionary<string, List<UniqueID>>();

        foreach (UniqueID uid in allIDs)
        {
            string id = uid.ID;
            if (!idGroups.ContainsKey(id))
            {
                idGroups[id] = new List<UniqueID>();
            }
            idGroups[id].Add(uid);
        }

        var duplicates = idGroups.Where(kvp => kvp.Value.Count > 1).ToList();

        if (duplicates.Count == 0)
        {
            EditorUtility.DisplayDialog("No Duplicates Found",
                $"All {allIDs.Length} UniqueIDs in the scene are unique!",
                "OK");
        }
        else
        {
            string message = $"Found {duplicates.Count} duplicate IDs:\n\n";
            foreach (var dup in duplicates.Take(5))
            {
                message += $"ID: {dup.Key.Substring(0, 8)}... used by {dup.Value.Count} objects\n";
            }

            if (duplicates.Count > 5)
            {
                message += $"\n...and {duplicates.Count - 5} more duplicate IDs";
            }

            EditorUtility.DisplayDialog("Duplicates Found", message, "OK");
        }

        Debug.Log($"Scan complete: {allIDs.Length} total objects, {duplicates.Count} duplicate IDs found");
    }

    private void FixAllDuplicates()
    {
        if (!EditorUtility.DisplayDialog("Fix Duplicate IDs",
            "This will regenerate IDs for all duplicate UniqueID components in the scene. Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        UniqueID[] allIDs = GameObject.FindObjectsOfType<UniqueID>();
        Dictionary<string, List<UniqueID>> idGroups = new Dictionary<string, List<UniqueID>>();

        // Group by ID
        foreach (UniqueID uid in allIDs)
        {
            string id = uid.ID;
            if (!idGroups.ContainsKey(id))
            {
                idGroups[id] = new List<UniqueID>();
            }
            idGroups[id].Add(uid);
        }

        int fixedCount = 0;

        // Fix duplicates (keep first, regenerate rest)
        foreach (var group in idGroups.Where(kvp => kvp.Value.Count > 1))
        {
            // Skip the first one (keep its ID), regenerate the rest
            for (int i = 1; i < group.Value.Count; i++)
            {
                Undo.RecordObject(group.Value[i], "Fix Duplicate ID");
                group.Value[i].GenerateID();
                EditorUtility.SetDirty(group.Value[i]);
                fixedCount++;
            }
        }

        if (fixedCount > 0)
        {
            EditorUtility.DisplayDialog("IDs Fixed",
                $"Successfully regenerated {fixedCount} duplicate IDs!",
                "OK");
            Debug.Log($"Fixed {fixedCount} duplicate IDs");
        }
        else
        {
            EditorUtility.DisplayDialog("No Duplicates",
                "No duplicate IDs found to fix!",
                "OK");
        }
    }

    private void GenerateIDsForSelection()
    {
        GameObject[] selected = Selection.gameObjects;

        if (selected.Length == 0)
        {
            EditorUtility.DisplayDialog("No Selection",
                "Please select one or more GameObjects with UniqueID components.",
                "OK");
            return;
        }

        int count = 0;
        foreach (GameObject obj in selected)
        {
            UniqueID uid = obj.GetComponent<UniqueID>();
            if (uid != null)
            {
                Undo.RecordObject(uid, "Generate ID");
                uid.GenerateID();
                EditorUtility.SetDirty(uid);
                count++;
            }
        }

        EditorUtility.DisplayDialog("IDs Generated",
            $"Generated new IDs for {count} selected objects.",
            "OK");
        Debug.Log($"Generated new IDs for {count} objects");
    }

    private void ClearAllIDs()
    {
        if (!EditorUtility.DisplayDialog("Clear All IDs",
            "This will clear all UniqueIDs in the scene. They will regenerate when you play the game. Continue?",
            "Yes", "Cancel"))
        {
            return;
        }

        UniqueID[] allIDs = GameObject.FindObjectsOfType<UniqueID>();

        foreach (UniqueID uid in allIDs)
        {
            Undo.RecordObject(uid, "Clear ID");
            uid.SetID(string.Empty);
            EditorUtility.SetDirty(uid);
        }

        EditorUtility.DisplayDialog("IDs Cleared",
            $"Cleared {allIDs.Length} IDs. They will regenerate at runtime.",
            "OK");
        Debug.Log($"Cleared {allIDs.Length} IDs");
    }
}
