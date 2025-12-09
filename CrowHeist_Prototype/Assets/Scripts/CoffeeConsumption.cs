using System.Collections;
using UnityEngine;

namespace KinematicCharacterController.Examples
{
    public class CoffeeConsumption : MonoBehaviour
    {
        private Controller2Point5D controller;

        private void Start()
        {
            controller = FindObjectOfType<Controller2Point5D>();
        }

        public void TryConsumeCoffee()
        {
            if (controller != null && controller.heldObject == this.GetComponent<Rigidbody>() && Input.GetKeyDown(KeyCode.E) && !controller.isSpeedBoosted)
        {
            StartCoroutine(SpeedBoost());
        }
        }

        private IEnumerator SpeedBoost()
        {
            GameObject coffeeDrink = null;
            GameObject coffee = null;

            if (controller.heldObject == null || !controller.heldObject.CompareTag("Dashable"))
            {
                yield break;
            }

            if (controller.heldObject.CompareTag("Dashable"))
            {
                coffeeDrink = controller.heldObject.gameObject;
            }

            foreach (Transform child in controller.GetComponentsInChildren<Transform>(true))
            {
                if (child.name == "CoffeeLiquid")
                {
                    coffee = child.gameObject;
                    break;
                }
            }

            controller.isSpeedBoosted = true;
            controller.moveSpeed *= controller.speedBoostMultiplier;

            yield return new WaitForSeconds(controller.speedBoostDuration);

            controller.moveSpeed = controller.normalMoveSpeed;
            controller.isSpeedBoosted = false;
            
            if (coffee != null)
            {
                coffee.transform.localPosition = new Vector3(0, 0.0076f, 0);
            }
            
            if (coffeeDrink != null)
            {
                coffeeDrink.tag = "Mug";
            }
            
            controller.Drop();
        }
    }
}