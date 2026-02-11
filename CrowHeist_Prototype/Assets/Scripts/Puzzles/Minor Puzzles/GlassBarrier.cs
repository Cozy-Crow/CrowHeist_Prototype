using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class GlassBarrier : MonoBehaviour
{
    [SerializeField] private GameObject roomba;
    [SerializeField] private GameObject[] brokenPieces;
    [SerializeField] private ParticleSystem breakEffect;
    [SerializeField] private AudioClip breakSound;
    [SerializeField] private float breakForce = 5f;
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private float minThrowVelocity = 3f;

    private bool roombaActivated = false;

    void Update()
    {
        if (!roombaActivated && roomba != null)
        {
            RoombAi roombAi = roomba.GetComponent<RoombAi>();
            if (roombAi != null && roombAi.isActivated)
            {
                roombaActivated = true;
                AddBreakableComponent();
            }
        }
    }

    void AddBreakableComponent()
    {
        var breakable = gameObject.AddComponent<BreakableObjectFramework>();
        breakable.brokenPieces = brokenPieces;
        breakable.breakEffect = breakEffect;
        breakable.breakSound = breakSound;
        breakable.BreakForce = breakForce;
        breakable.coinPrefab = coinPrefab;
        breakable.minThrowVelocity = minThrowVelocity;
    }
}
