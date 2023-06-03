using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonPlayerController : MonoBehaviour
{
    //Player look parametres
    public float mouseSensitivity = 100f;
    private Transform bodyTransform;
    private Transform cameraTransform;
    private float xRotation = 0f;

    //Player move parametres
    [SerializeField] private CharacterController controller;
    public float movementSpeed = 12f;

    //Gravity parametres
    Vector3 velocity;
    public float gravity = -9.81f;

    //Ground check paremetres
    [SerializeField] private Transform groundCheck;
    [SerializeField] private LayerMask groundMask;
    private float groundDistance = 0.4f;
    bool isGrounded;

    //Jump parametres
    public float jumpHeigh = 2f;


    void Start()
    {
        bodyTransform = GameObject.FindGameObjectWithTag("Player").transform;
        cameraTransform = GameObject.FindGameObjectWithTag("MainCamera").transform;
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        PlayerLook();
        PlayerMovement();
    }

    private void PlayerMovement()
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

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = 0f;
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
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        bodyTransform.Rotate(Vector3.up * mouseX);
    }
}
