using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class IsometricController : MonoBehaviour
{
    [SerializeField] private float _moveSpeed = 5f;
    private Vector3 _forward,_right;

    [SerializeField] private float _jumpHeight = 2.0f;
    [SerializeField] private float _jumpForce = 0.5f;
    private bool _isJumping = false;

    private Rigidbody _rigidbody;

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();

        _forward = Camera.main.transform.forward;
        _forward.y = 0;
        _forward = Vector3.Normalize(_forward);
        _right = Quaternion.Euler(new Vector3(0, 90, 0)) * _forward;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && !_isJumping)
        {
            _isJumping = true;
            StartCoroutine(Jump());
        }else
        {
            Move();
        }
    }

    private void Move()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 rightMovement = _right * _moveSpeed * Time.deltaTime * horizontal;
        Vector3 upMovement = _forward * _moveSpeed * Time.deltaTime * vertical;

        Vector3 heading = Vector3.Normalize(rightMovement + upMovement);

        transform.forward = heading;
        transform.position += rightMovement;
        transform.position += upMovement;
    }

    private IEnumerator Jump()
    {
        float originalY = transform.position.y;
        float maxHeight = originalY + _jumpHeight;
        _rigidbody.useGravity = false;
        _isJumping = true;

        yield return null;

        _rigidbody.AddForce(Vector3.up * _jumpHeight, ForceMode.Impulse);

        while (transform.position.y < maxHeight)
        {
            transform.position += transform.up * _jumpForce * Time.deltaTime;
            yield return null;
        }

        _rigidbody.useGravity = true;

        while (transform.position.y > originalY)
        {
            //transform.position -= transform.up * _jumpForce * Time.deltaTime;
            yield return null;
        }

        _rigidbody.useGravity = true;
        _isJumping = false;

        yield return null;
    }
}
