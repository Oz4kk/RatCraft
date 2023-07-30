using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 10.0f;

    [SerializeField] private float cameraSensitivity = 50.0f;
    private float verticalCameraRotation;
    [SerializeField] private float minVerticalCameraClamp = -90.0f;
    [SerializeField] private float maxVerticalCameraClamp = 90.0f;

    private Rigidbody rigidBody;
    [SerializeField] private Camera playerCamera; 

    private void Awake()
    {
        rigidBody = GetComponent<Rigidbody>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }


    void Update()
    {
        CameraRotation();
    }

    private void MovePlayer()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 forwardMovement = vertical * transform.forward * Time.fixedDeltaTime * movementSpeed;
        Vector3 sideMovement = horizontal * transform.right * Time.fixedDeltaTime * movementSpeed;

        Vector3 velocity = forwardMovement + sideMovement;
        velocity.y = rigidBody.velocity.y;

        rigidBody.velocity = velocity;
    }
    private void CameraRotation()
    {
        float mouseX = Input.GetAxis("Mouse X") * Time.deltaTime * cameraSensitivity;
        float mouseY = Input.GetAxis("Mouse Y") * Time.deltaTime * cameraSensitivity;

        verticalCameraRotation -= mouseY;
        verticalCameraRotation = Mathf.Clamp(verticalCameraRotation, minVerticalCameraClamp, maxVerticalCameraClamp);

        //proc zde nemuzu pouzit Rotate()?
        playerCamera.transform.localRotation = Quaternion.Euler(Vector3.right * verticalCameraRotation);
        transform.Rotate(Vector3.up * mouseX);
    }
}
