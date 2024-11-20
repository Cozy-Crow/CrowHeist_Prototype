using KinematicCharacterController.Examples;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class InteractableRoomba : Interactable
{
    [SerializeField] private SFXData[] _sfx;

    private NavMeshAgent _navMeshAgent;
    private ScriptMachine _scriptMachine;

    private AudioSource _audioSource;
    private Dictionary<string, SFXData> _sfxDictionary = new Dictionary<string, SFXData>();
    private bool _isBroken = false;
    protected override void Awake()
    {
        base.Awake();
        _navMeshAgent = GetComponentInParent<NavMeshAgent>();
        _scriptMachine = GetComponentInParent<ScriptMachine>();
        _audioSource = GetComponentInParent<AudioSource>();

        foreach(SFXData sfx in _sfx)
        {
            _sfxDictionary.Add(sfx.name, sfx);
        }
    }

    private void Update()
    {
        if (_interact)
        {
            if (Input.GetKeyDown(KeyCode.F))
            {
                if (!_isBroken)
                {
                    _audioSource.loop = false;
                    _audioSource.clip = _sfxDictionary["RoombaBreak"].SFX;
                    _audioSource.PlayOneShot(_sfxDictionary["RoombaBreak"].SFX);
                    _isBroken = true;
                }
                else
                {
                    _audioSource.loop = true;
                    _audioSource.clip = _sfxDictionary["RoombaLoop"].SFX;
                    _audioSource.Play();
                    _isBroken = false;
                }
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
