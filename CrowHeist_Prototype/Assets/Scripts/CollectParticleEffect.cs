using UnityEngine;

public class CollectParticleEffect : MonoBehaviour
{
    [SerializeField] private float _destroyDelay = 3f;
    private ParticleSystem _particleSystem;

    private void Awake()
    {
        _particleSystem = GetComponent<ParticleSystem>();

        if (_particleSystem != null)
        {
            // Auto-destroy based on particle system duration
            float duration = _particleSystem.main.duration + _particleSystem.main.startLifetime.constantMax;
            _destroyDelay = Mathf.Max(_destroyDelay, duration);
        }

        Destroy(gameObject, _destroyDelay);
    }

    private void Start()
    {
        if (_particleSystem != null)
        {
            _particleSystem.Play();
        }
    }
}
