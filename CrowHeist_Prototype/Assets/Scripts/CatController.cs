using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    CharacterController controller;

    // Variable of the float speed
    private float speed = 7f;

    // Start is called before the first frame update
    // Getting the character controller component in Unity inspector
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    // Gets the horizontal and vertical axis of the cat game object and
    // multiplying those variables by the speed listed above and the
    // Time.deltaTime function
    void Update()
    {
        float x = Input.GetAxis("horizontalCat") * speed * Time.deltaTime;
        float z = Input.GetAxis("verticalCat") * speed * Time.deltaTime;

        Vector3 moveDelta = new Vector3(x, 0, z);   // This tells the cat how much to move

        controller.Move(moveDelta);   // This tells the controller to start moving the cat
    }
}
