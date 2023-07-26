using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour
{
    //Player look parametres
    [SerializeField] private float mouseSensitivity = 100.0f;
    private float xRotation = 0.0f;
    //Player move parametres
    [SerializeField] private float movementSpeed = 12.0f;
    //Gravity parametres
    Vector3 velocity;
    [SerializeField] private float gravity = -9.81f;
    //Jump parametres
    [SerializeField] private float jumpHeigh = 2.0f;
    //Ground-check parametres
    private float groundDistance = 0.4f;
    private bool isGrounded;

    [SerializeField] private Camera verticalCameraRotation;
    [SerializeField] private CharacterController controller;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerLook();
        PlayerMove();
    }

    private void PlayerMove()
    {
        GroundCheck();
        Mover();
        Jump();
        Gravity();
    }

    //Methodes that use PlayerMovement method
    private void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (controller.isGrounded)
        {
            velocity.y = 0.0f;
        }
    }
    private void Mover()
    {
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * movementSpeed * Time.deltaTime);
    }
    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity);
        }
    }
    private void PlayerLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90.0f, 90.0f);

        verticalCameraRotation.transform.localRotation = Quaternion.Euler(xRotation, 0.0f, 0.0f);
        transform.Rotate(Vector3.up * mouseX);
    }
}
