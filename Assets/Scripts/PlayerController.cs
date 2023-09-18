using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 groundCheckBoxExtents;
    private InputManager inputManager;

    [SerializeField] private float movementSpeed = 10.0f;

    [SerializeField] private float jumpForce = 10.0f;

    [SerializeField] private float cameraSensitivity = 50.0f;
    [SerializeField] private float minVerticalCameraClamp = -90.0f;
    [SerializeField] private float maxVerticalCameraClamp = 90.0f;
    
    private float verticalCameraRotation;
    private bool isGrounded;

    private void Awake()
    {
        inputManager = GetComponent<InputManager>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        MovePlayer();
    }

    void Update()
    {
        Jump();
        CameraRotation();
    }
    private void GroundCheck()
    {
        isGrounded = Physics.CheckBox(groundCheck.transform.position, groundCheckBoxExtents);
    }

    private void OnDrawGizmosSelected()
    {
        //Debug that I don't use rn. (need to connect it in code, in the moment I need it).
        Gizmos.color = Color.green;
        Gizmos.DrawCube(groundCheck.transform.position, groundCheckBoxExtents);
    }

    private void Jump()
    {
        if (!isGrounded)
        {
            return;
        }
        if (inputManager.GetButtonDown("Jump"))
        {
            rigidBody.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    private void MovePlayer()
    {
        float horizontal = inputManager.GetAxis("Horizontal");
        float vertical = inputManager.GetAxis("Vertical");

        if (Mathf.Approximately(horizontal, 0.0f) && Mathf.Approximately(vertical, 0.0f))
        {
            return;
        }

        Vector3 forwardMovement = vertical * transform.forward * Time.fixedDeltaTime * movementSpeed;
        Vector3 sideMovement = horizontal * transform.right * Time.fixedDeltaTime * movementSpeed;

        Vector3 velocity = forwardMovement + sideMovement;
        velocity.y = rigidBody.velocity.y;

        rigidBody.velocity = velocity;
    }
    private void CameraRotation()
    {
        float mouseX = inputManager.GetAxis("Mouse X") * Time.deltaTime * cameraSensitivity;
        float mouseY = inputManager.GetAxis("Mouse Y") * Time.deltaTime * cameraSensitivity;

        verticalCameraRotation -= mouseY;
        verticalCameraRotation = Mathf.Clamp(verticalCameraRotation, minVerticalCameraClamp, maxVerticalCameraClamp);

        playerCamera.transform.localRotation = Quaternion.Euler(Vector3.right * verticalCameraRotation);
        transform.Rotate(Vector3.up * mouseX);
    }
}
