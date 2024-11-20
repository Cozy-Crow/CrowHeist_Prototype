using KinematicCharacterController.Examples;
using UnityEngine;
public class InteractableScrewDriver : Interactable
{
    protected override void Awake()
    {
        base.Awake();
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            string itemHeld = other.GetComponent<Controller2Point5D>().Equipped;

            if (itemHeld == "ScrewDriver")
            {
                if (_canvas is not null)
                {
                    _canvas.SetActive(true);
                }
                _outline.enabled = true;
                _interact = true;
            }
        }
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
