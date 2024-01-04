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
    private IState currentState;
    private DestroyCubeState destroyState;
    private PlaceCubeState placeCubeState;


    private float verticalCameraRotation;
    private bool isGrounded;

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
        placeCubeState = new PlaceCubeState(playerCubePlacement, inventoryHandler, mapGenerator, inputManager);
        destroyState = new DestroyCubeState(inputManager, mapGenerator, inventoryHandler, cubeBreakDistance);
        currentState = placeCubeState;

        Cursor.lockState = CursorLockMode.Locked;
    }

    private void FixedUpdate()
    {
        GroundCheck();
        MovePlayer();
    }

    void Update()
    {
        PickState();
        UpdatePointerCube();
        MouseHandler();
        Jump();
        CameraRotation();
        ChooseItemInInventory();
    }

    private void ChooseItemInInventory()
    {
        ChooseCubeWithMouseScroll();
        ChooseItemWithKeyboard();
    }


    private void ChooseCubeWithMouseScroll()
    {
        float mouseScrollValue = inputManager.GetAxis("Mouse ScrollWheel");
        if (mouseScrollValue == .0f)
        {
            return;
        }
        inventoryHandler.ChooseCubeWithMouseScroll(mouseScrollValue);
    }
    private void ChooseItemWithKeyboard()
    {
        foreach (KeyCodeIndexPair item in inventoryHandler.keyCodeIndexPairs)
        {
            if (inputManager.GetKeyDown(item.keycode))
            {
                inventoryHandler.SetSlot(item.index);
                break;
            }
        }
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
        if (!Input.GetKey(KeyCode.Mouse0))
        {
            return;
        }
        if (currentState == null)
        {
            return;
        }

        currentState.ExecuteState();
    }

    private void PickState()
    {
        if (!Input.GetKeyDown(KeyCode.Mouse1))
        {
            return;
        }
        if (currentState == placeCubeState)
        {
            playerCubePointer.pointerCubeMeshRenderer.enabled = false;
        }

        currentState = currentState == destroyState ? placeCubeState : destroyState;
    }

    private void UpdatePointerCube()
    {
        if (currentState != placeCubeState)
        {
            return;
        }
        Vector3? raycastHitLocation = playerCubePlacement.CalculateUpcomingCubePosition();
        if (raycastHitLocation != previousPlacementLocation)
        {
            onRaycastHitDifferentCube?.Invoke();
        }
    }
}
