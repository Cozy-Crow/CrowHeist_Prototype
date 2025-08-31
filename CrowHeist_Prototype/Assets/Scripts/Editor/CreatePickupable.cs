using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Pickable))]
public class CreatePickupableEditor : Editor
{
    private string targetLayerName = "Interactable"; // Change this to your desired layer
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector(); // Keeps the default inspector UI

        Pickable pickupable = (Pickable)target;

        if (GUILayout.Button("Create Pickupable"))
        {
            SetupPickupable(pickupable.gameObject);
        }
    }

    private void SetupPickupable(GameObject obj)
    {
        int layerID = LayerMask.NameToLayer(targetLayerName);
        obj.layer = layerID;


        if (!obj.GetComponent<Outline>())
        {
            obj.AddComponent<Outline>();
        }

        if (!obj.transform.Find("Trigger"))
        {
            GameObject interactionTrigger = new GameObject("InteractionTrigger");
            interactionTrigger.transform.SetParent(obj.transform);
            interactionTrigger.transform.localPosition = Vector3.zero; // Position it at the parent’s location

            // Add a BoxCollider and set it as a trigger
            BoxCollider triggerCollider = interactionTrigger.AddComponent<BoxCollider>();
            triggerCollider.isTrigger = true;

            // Ensure the "Interactable" script is attached (Replace "Interactable" with your actual script name)
            if (!interactionTrigger.GetComponent<Interactable>())
            {
                interactionTrigger.AddComponent<Interactable>();
                Debug.Log("Added Interactable script to the child object.");
            }


        }
    }
}
