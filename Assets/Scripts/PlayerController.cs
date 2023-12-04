using System;
using System.Collections;
using System.Collections.Generic;
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

    // Touching other scripts
    private GameObject gameController;
    private InputManager inputManager;
    private PlayerCubePlacement playerCubePlacement;
    private PlayerCubePointer playerCubePointer;
    private MapGenerator mapGenerator;
    private InventoryHandler inventoryHandler;

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
        RaycastHit hit;
        Vector3? raycastHitLocation = null;
        if (playerCubePointer.RayCastSequence(playerCubePlacement.cubePlacementDistance, out hit))
        {
            raycastHitLocation = playerCubePointer.GetPlayerCubePlacementPosition(hit);
            BreakCube(hit);
            PlaceCube(raycastHitLocation);
        }
        if (raycastHitLocation != previousPlacementLocation)
        {
            onRaycastHitDifferentCube?.Invoke();
        }
    }

    private void BreakCube(RaycastHit raycastHitLocation)
    {
        //if (raycastHitLocation == null)
        //{
        //    return;
        //}
        //if (!inputManager.GetKeyDown(KeyCode.Mouse1))
        //{
        //    return;
        //}

        //Vector3 raycastHitLocation2 = (Vector3)raycastHitLocation;
        //raycastHitLocation2 = new Vector3(raycastHitLocation2.x, raycastHitLocation2.y-1.0f, raycastHitLocation2.z);
        
        //GameObject actualCube = mapGenerator.mapField[raycastHitLocation2];

        //Destroy(actualCube);
    }

    private void PlaceCube(Vector3? raycastHitLocation)
    {
        if (raycastHitLocation == null)
        {
            return;
        }
        if (!inputManager.GetKeyDown(KeyCode.Mouse0))
        {
            return;
        }

        if (!playerCubePlacement.DoesPlayerCollideWithCubePlacementLocation((Vector3)raycastHitLocation))
        {
            GameObject actualCube = mapGenerator.InstantiateAndReturnCube((Vector3)raycastHitLocation, inventoryHandler.GetSelectedCube());
        }
    }



}





class A
{

}

class B : A
{

}

class C : A
{
    
}

class S
{
    private T NovaMetoda<T>(A a) where T: A
    {
        return (T)a;
    }

    private void RendomMethoda()
    {
        A b = new B();
        A c = new C();

        
        B d = NovaMetoda<B>(b);
        C k = NovaMetoda<C>(c);
    }
}