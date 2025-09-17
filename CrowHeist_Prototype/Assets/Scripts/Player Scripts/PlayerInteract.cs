using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInteract : MonoBehaviour
{
    // Usage:
    // Handles everything the player will do regarding interaction
    // Pick up/Holding/Throwing/General Interacting

    [Header("PickUP [NEW]")]
    [SerializeField] private Transform _pickUpPoint;
    [SerializeField] private Transform _handPoint;
    [SerializeField] private Transform _dropPoint;
    private List<IPickupable> _pickUpsList = new List<IPickupable>();
    public Rigidbody heldObject;
    public bool _isDirty = false; //is this still needed
    private List<Interactable> nearbyInteractables = new List<Interactable>();
    private int currentTargetIndex = 0;



    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    //updates what interactables are in range
    //interactions are currently handled by     
    private void UpdateHighlightedInteractable()
    {
        if (nearbyInteractables.Count == 0) return;

        if (currentTargetIndex >= nearbyInteractables.Count)
        {
            currentTargetIndex = 0;
        }

        foreach (var interactable in nearbyInteractables)
        {
            if (interactable != null)
            {
                interactable.SetOutline(false);
            }
        }

        Interactable target = nearbyInteractables[currentTargetIndex];
        if (target != null)
        {
            target.SetOutline(true);
        }
    }

    //handles the pickup behavior when pressing the interact key
    public void HandlePickUpPI()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            AIEventManager.instance.e_pickup.Invoke();

            if (_pickUpsList.Count > 0) return;

            if (nearbyInteractables.Count > 0 && currentTargetIndex < nearbyInteractables.Count)
            {
                Interactable selected = nearbyInteractables[currentTargetIndex];
                if (selected != null && selected.realObject != null)
                {
                    if (selected.realObject.TryGetComponent(out IPickupable pickUp))
                    {
                        pickUp.PickUP(_pickUpPoint);
                        _pickUpsList.Add(pickUp);
                        heldObject = selected.realObject.GetComponent<Rigidbody>();
                    }
                }
            }
        }
    }

    //function that can be called to make the player drop wht they are hold
    public void DropPI()
    {
        foreach (IPickupable pickUp in _pickUpsList)
        {
            pickUp.Drop(_dropPoint.position);
        }
        _pickUpsList.Clear();
        heldObject = null;
    }

    //
    void OnTriggerEnter(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            if (!nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Add(interactable);
                UpdateHighlightedInteractable();
            }
        }
    }

    //
    void OnTriggerExit(Collider other)
    {
        if (other.TryGetComponent(out Interactable interactable))
        {
            if (nearbyInteractables.Contains(interactable))
            {
                nearbyInteractables.Remove(interactable);
                UpdateHighlightedInteractable();
            }
        }
    }
}
