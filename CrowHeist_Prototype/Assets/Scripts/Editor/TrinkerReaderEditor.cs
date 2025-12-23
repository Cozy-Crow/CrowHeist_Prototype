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

        if (GUILayout.Button("Create Trinket ScriptableObjects"))
        {
            trinketReader.UpdateTrinketData();
        }
    }
}
