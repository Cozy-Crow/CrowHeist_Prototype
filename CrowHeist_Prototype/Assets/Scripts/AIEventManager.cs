using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class AIEventManager : MonoBehaviour
{
    public static AIEventManager instance;

    [SerializeField] public UnityEvent e_pickup = new UnityEvent();
    [SerializeField] public UnityEvent e_makedirty = new UnityEvent();

    [SerializeField] public UnityEvent<Vector3> OnGroundObjectDirty = new UnityEvent<Vector3>();
    [SerializeField] public UnityEvent<Vector3> OnPlayerDirty = new UnityEvent<Vector3>();


    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void SetObjectDirty(bool isDirty)
    {
        if (isDirty)
        {
            e_makedirty.Invoke();
        }
    }

    public void GroundItemDirty(Vector3 dirtyObjectPosition)
    {
        OnGroundObjectDirty.Invoke(dirtyObjectPosition);
    }

    public void PlayerDirty(Vector3 playerPos)
    {
        OnPlayerDirty.Invoke(playerPos);
    }

}
