using KinematicCharacterController.Examples;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class InteractableExit : Interactable
{
    private Collider _collider;
    protected override void Awake()
    {
        base.Awake();
        _collider = transform.parent.GetComponent<Collider>();
    }

    private void Update()
    {
        if (_interact)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                _collider.enabled = false;
                GameManager.ChangeCamera("Exit");

                var player = GameObject.FindGameObjectWithTag("Player").GetComponent<Controller2Point5D>();
                //player.StartThrow();

                _outline.enabled = false;
                _canvas.SetActive(false);
            }
        }
    }

    public override void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (_canvas is not null)
            {
                _canvas.SetActive(true);
            }
            _outline.enabled = true;
            _interact = true;
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
