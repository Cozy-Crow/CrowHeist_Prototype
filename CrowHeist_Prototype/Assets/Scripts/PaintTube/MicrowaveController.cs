using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class MicrowaveController : MonoBehaviour
{
    [Header("Detection")]
    public string knifeTag = "Knife";

    [Header("Effects")]
    public ParticleSystem explosionParticles;
    public AudioClip alarmSound;
    public AudioClip boomSound;
    public Animator microwaveAnimator;
    public float cameraShakeDuration = 0.6f;
    public float cameraShakeMagnitude = 0.3f;

    [Header("Paint Tube Spawn")]
    public GameObject paintTubePrefab;      
    public Transform paintTubeSpawnPoint;     

    [Header("Gameplay state")]
    public bool microwaveLocked = false;
    public float disableDuration = 5f;

    private AudioSource audioSource;

    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (explosionParticles != null)
            explosionParticles.Stop();
    }

    void OnTriggerEnter(Collider other)
    {
        if (microwaveLocked) return;

        if (other.CompareTag(knifeTag))
        {
            Debug.Log("Knife detected inside microwave — triggering effects...");
            StartCoroutine(HandleKnifeDetected());
        }
    }

    System.Collections.IEnumerator HandleKnifeDetected()
    {
        microwaveLocked = true;

        if (alarmSound != null)
            audioSource.PlayOneShot(alarmSound);


        if (explosionParticles != null)
        {
            explosionParticles.transform.SetParent(null);
            explosionParticles.Play();
        }

        if (boomSound != null)
            audioSource.PlayOneShot(boomSound);

        Camera cam = Camera.main;
        if (cam != null)
            StartCoroutine(ShakeCamera(cam.transform, cameraShakeDuration, cameraShakeMagnitude));

        if (microwaveAnimator != null)
            microwaveAnimator.SetBool("Disabled", true);

        yield return new WaitForSeconds(1.2f);

        if (paintTubePrefab != null && paintTubeSpawnPoint != null)
        {
            Instantiate(
                paintTubePrefab,
                paintTubeSpawnPoint.position,
                paintTubeSpawnPoint.rotation
            );
        }

        yield return new WaitForSeconds(disableDuration);

        if (microwaveAnimator != null)
        {
            microwaveAnimator.SetBool("Disabled", false);
            microwaveAnimator.SetTrigger("Reset");
        }

        microwaveLocked = false;
    }

    System.Collections.IEnumerator ShakeCamera(Transform camTransform, float duration, float magnitude)
    {
        Vector3 originalPos = camTransform.localPosition;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            camTransform.localPosition =
                originalPos + new Vector3(
                    Random.Range(-magnitude, magnitude),
                    Random.Range(-magnitude, magnitude),
                    0f
                );

            elapsed += Time.deltaTime;
            yield return null;
        }

        camTransform.localPosition = originalPos;
    }
}
