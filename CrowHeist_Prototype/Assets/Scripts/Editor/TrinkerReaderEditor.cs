using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
[CustomEditor(typeof(TrinketReader))]
public class TrinkerReaderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Draw the default inspector

        TrinketReader trinketReader = (TrinketReader)target;

        if (GUILayout.Button("Update ScriptableObjects"))
        {
            trinketReader.UpdateTrinketData();
            EditorUtility.SetDirty(trinketReader);
            Repaint();
        }

        if(GUILayout.Button("Load All Trinkets From Path"))
        {
            trinketReader.LoadAllTrinketsFromPath();
        }

        if(GUILayout.Button("Delete All Trinkets From Path"))
        {
            trinketReader.DeleteAllTrinketsFromPath();
        }

        if (GUILayout.Button("Unlock All Trinkets"))
        {
            trinketReader.UnlockAll();
        }

        if (GUILayout.Button("Lock All Trinkets"))
        {
            trinketReader.LockAll();
        }
    }
}
