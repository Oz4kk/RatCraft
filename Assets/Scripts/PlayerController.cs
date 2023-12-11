using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Action onRaycastHitDifferentCube;

    // Serialize fields
    [SerializeField] private Rigidbody rigidBody;
    [SerializeField] private Camera playerCamera;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector3 groundCheckBoxExtents;

    // Serialize fields for player movement
    [SerializeField] private float movementSpeed = 10.0f;
    [SerializeField] private float jumpForce = 10.0f;
    [SerializeField] private float cameraSensitivity = 50.0f;
    [SerializeField] private float minVerticalCameraClamp = -90.0f;
    [SerializeField] private float maxVerticalCameraClamp = 90.0f;
    [SerializeField] private float cubeBreakDistance;

    // Touching other scripts
    private GameObject gameController;
    private InputManager inputManager;
    private PlayerCubePlacement playerCubePlacement;
    private PlayerCubePointer playerCubePointer;
    private MapGenerator mapGenerator;
    private InventoryHandler inventoryHandler;

    private float verticalCameraRotation;
    private bool isGrounded;

    private float miningTimer;

    private Vector3 previousPlacementLocation;

    public void SetGameController(GameObject gameController)
    {
        this.gameController = gameController;
    }

    private void Start()
    {
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        playerCubePointer = GetComponent<PlayerCubePointer>();
        inputManager = GetComponent<InputManager>();
        inventoryHandler = GetComponent<InventoryHandler>();
        mapGenerator = gameController.GetComponent<MapGenerator>();

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        MovePlayer();
    }

    void Update()
    {
        MouseHandler();
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

        Vector3 velocity = new Vector3(0.0f, rigidBody.velocity.y, 0.0f);


        //If user isn't moving, keep his Y velocity and return
        if (Mathf.Approximately(horizontal, 0.0f) && Mathf.Approximately(vertical, 0.0f))
        {
            rigidBody.velocity = velocity;

            return;
        }

        Vector3 forwardMovement = vertical * transform.forward * Time.fixedDeltaTime * movementSpeed;
        Vector3 sideMovement = horizontal * transform.right * Time.fixedDeltaTime * movementSpeed;

        velocity = forwardMovement + sideMovement;
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

    private void MouseHandler()
    {
        PlaceCube();
        BreakCubeSequence();
    }

    private void BreakCubeSequence()
    {
        // If user don't hold key, null timer and return
        if (!inputManager.GetKey(KeyCode.Mouse1))
        {
            miningTimer = 0.0f;
            return;
        }
        // If raycast don't hit anything, null timer and return
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, cubeBreakDistance))
        {
            miningTimer = 0.0f;
            return;
        }
        // Add time.deltaTime to the mining timer and create gameObject of hitted cube
        miningTimer += Time.deltaTime;
        GameObject actualCube = mapGenerator.mapField[hit.transform.position];
        //CubeParameters cube = hit.transform.gameObject.GetComponent<CubeParameters>();
        //if (cube == null)
        //{
        //    return;
        //}
        // Return if mining timer is lower that brittenes of hitted cube
        if (miningTimer < actualCube.GetComponent<CubeParameters>().brittleness)
        {
            return;
        }
        // Destroy cube in the world, remove it from the mapField dictionary, add increment ammount of hitted cube in the inventory and set mining time to 0
        mapGenerator.DeleteCube(hit, actualCube);
        inventoryHandler.AddNewItem(actualCube.name);
        miningTimer = 0.0f;
    }

    private void PlaceCube()
    {
        Vector3? raycastHitLocation = playerCubePlacement.CalculateUpcomingCubePosition();
        if (raycastHitLocation != previousPlacementLocation)
        {
            onRaycastHitDifferentCube?.Invoke();
        }
        if (raycastHitLocation == null)
        {
            return;
        }
        if (inventoryHandler.inventory[(int)inventoryHandler.activeSlot].amount < 1)
        {
            return;
        }
        if (!inputManager.GetKeyDown(KeyCode.Mouse0))
        {
            return;
        }
        if (playerCubePlacement.DoesPlayerCollideWithCubePlacementLocation((Vector3)raycastHitLocation))
        {
            return;
        }
        GameObject actualCube = mapGenerator.InstantiateAndReturnCube((Vector3)raycastHitLocation, inventoryHandler.GetSelectedCube());
        inventoryHandler.RemoveItemFromInventory(actualCube.name);
    }
}
