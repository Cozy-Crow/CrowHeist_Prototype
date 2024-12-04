using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CatController : MonoBehaviour
{
    CharacterController controller;

    private float speed = 7f;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        float x = Input.GetAxis("horizontalCat") * speed * Time.deltaTime;
        float z = Input.GetAxis("verticalCat") * speed * Time.deltaTime;

        Vector3 moveDelta = new Vector3(x, 0, z);

        controller.Move(moveDelta);
    }
}
