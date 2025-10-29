using System.Collections;
using UnityEngine;

public class Cobweb : MonoBehaviour
{
    [SerializeField] private float slowdownFactor = 0.3f;
    [SerializeField] private ParticleSystem destroyEffect;
    
    private PlayerMovement playerMovement;
    private bool playerInWeb = false;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInWeb)
        {
            playerMovement = other.GetComponent<PlayerMovement>();
            if (playerMovement)
            {
                playerMovement.SetSpeedMultiplier(slowdownFactor);
                playerInWeb = true;
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && playerInWeb)
        {
            if (playerMovement)
            {
                playerMovement.SetSpeedMultiplier(1f);
                playerInWeb = false;
            }
        }
    }

    public void CutWithKnife()
    {
        if (playerInWeb && playerMovement)
        {
            playerMovement.SetSpeedMultiplier(1f);
        }
        
        if (destroyEffect)
        {
            destroyEffect.Play();
            StartCoroutine(DestroyAfterEffect());
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private IEnumerator DestroyAfterEffect()
    {
        GetComponent<Collider>().enabled = false;
        GetComponent<Renderer>().enabled = false;
        yield return new WaitForSeconds(destroyEffect.main.duration);
        Destroy(gameObject);
    }
}
