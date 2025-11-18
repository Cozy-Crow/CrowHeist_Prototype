using System.Collections;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class Cobweb : MonoBehaviour
    {
        [SerializeField] private float slowdownFactor = 0.1f;
        [SerializeField] private ParticleSystem destroyEffect;

        private Controller2Point5D controller;
        private bool playerInWeb = false;
        private float originalSpeed;

        void Start()
        {
            controller = FindObjectOfType<Controller2Point5D>();
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("Knife"))
            {
                CutWithKnife();
            }

            if (other.CompareTag("Player") && !playerInWeb)
                {
                    controller = other.GetComponent<Controller2Point5D>();
                    if (controller)
                    {
                        originalSpeed = controller._moveSpeed;
                        controller._moveSpeed *= slowdownFactor;
                        playerInWeb = true;
                        controller._canJump = false;
                        
                    }
                }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("Player") && playerInWeb)
            {
                if (controller)
                {
                    controller._moveSpeed = originalSpeed;
                    playerInWeb = false;
                    controller._canJump = true;
                }
            }
        }

        public void CutWithKnife()
        {
            if (playerInWeb && controller)
            {
                controller._moveSpeed = originalSpeed;
                playerInWeb = false;
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
}