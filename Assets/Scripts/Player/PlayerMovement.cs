using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //Variables
    [SerializeField] private float acceleration;
    [SerializeField] private float jumpPower;

    //Boolean Conditions
    protected bool isHanging;
    protected bool isMoving;

    //Input
    private Vector2 _inputDirection;

    //Player Component Reference
    [SerializeField] private Rigidbody2D _rigidbody2D;
    [SerializeField] private Animator _animator;

    private void Start()
    {
        //Send error message if component reference is missing
        if (_rigidbody2D == null)
            Debug.LogWarning("Player Rigidbody2D not found");

        if (_animator == null)
            Debug.LogWarning("Player Animator not found");
    }

    private void Update()
    {
        HandleInput();
        HorizontalMovement();
    }

    private void FixedUpdate()
    {
        VerticalMovement();
    }

    private void HandleInput()
    {
        //Get input from x and y input
        _inputDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    private void HorizontalMovement()
    {
        _rigidbody2D.velocity = new Vector2(_inputDirection.x * acceleration, _rigidbody2D.velocity.y);


    }

    private void VerticalMovement()
    {
        _rigidbody2D.velocity = new Vector2(_rigidbody2D.velocity.x, jumpPower);
    }
}
