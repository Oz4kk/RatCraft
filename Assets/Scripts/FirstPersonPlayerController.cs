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
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;

        controller.Move(move * movementSpeed * Time.deltaTime);
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
