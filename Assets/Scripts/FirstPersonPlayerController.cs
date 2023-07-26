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

    [SerializeField] private Camera verticalCameraRotation;
    [SerializeField] private CharacterController controller;

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
        Jump();
        Mover();
        Gravity();
    }

    //Methodes that use PlayerMovement method
    private void GroundCheck()
    {
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
        if (move == Vector3.zero)
        {
            return;
        }
        controller.Move(move * movementSpeed * Time.deltaTime);
    }
    private void Gravity()
    {
        velocity.y += gravity * Time.deltaTime;
        controller.Move(velocity * Time.deltaTime);
    }
    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Debug.Log("Pressed SPACE and isGrounded is set to " + controller.isGrounded);
            if (controller.isGrounded)
            {
                velocity.y = Mathf.Sqrt(jumpHeigh * -2 * gravity);
            }
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
