using KinematicCharacterController.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class InteractableRoomba : Interactable
{
    private NavMeshAgent _navMeshAgent;
    private ScriptMachine _scriptMachine;
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponentInParent<NavMeshAgent>();
        _scriptMachine = GetComponentInParent<ScriptMachine>();
    }

    private void Update()
    {
        if (_interact)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _navMeshAgent.enabled = !_navMeshAgent.enabled;
                _scriptMachine.enabled = !_scriptMachine.enabled;
            }
        }
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
