using KinematicCharacterController.Examples;
using UnityEngine;
public class InteractableScrewDriver : Interactable
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_canvas is not null)
            {
                _canvas.SetActive(false);
            }

            _outline.enabled = false;
            _interact = false;
        }
    }
}
