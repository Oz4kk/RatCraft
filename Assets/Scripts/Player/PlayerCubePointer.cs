using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    [HideInInspector] public MeshRenderer pointerCubeMeshRenderer;

    //SerializeFields
    [SerializeField] private GameObject pointerCubePrefab;
    [SerializeField] private GameObject gameController;

    //Touching other scripts
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private InventoryHandler inventoryHandler;
    private PlayerController playerController;

    private GameObject pointerCube;

    private void Awake()
    {
        //Touching other scripts
        mapGenerator = gameController.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();
        playerController = GetComponent<PlayerController>();

        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pointerCubeMeshRenderer = pointerCube.GetComponent<MeshRenderer>();

        //Subscriptions
        inventoryHandler.onActiveSlotChanged += ChangePointerCubeMaterial;
        playerController.onRaycastHitDifferentCube += ChangePointerCubePosition;        
    }

    private void ChangePointerCubePosition()
    {
        Vector3? upcomingCubePosition = playerCubePlacement.CalculateUpcomingCubePosition();

        if (upcomingCubePosition == null)
        {
            if (pointerCubeMeshRenderer.enabled)
            {
                pointerCubeMeshRenderer.enabled = false;
            }
            return;
        }
        else
        {
            pointerCubeMeshRenderer.enabled = true;
        }

        pointerCube.transform.position = (Vector3)upcomingCubePosition;

        DebugManager.Log($"Cube has changed location to {upcomingCubePosition?.GetStringOfVector3()}");
    }

    private void ChangePointerCubeMaterial()
    {
        Material materialOfActiveCube = inventoryHandler.ReturnActiveTransparentCubeMaterial();
        if (materialOfActiveCube != null)
        {
            pointerCubeMeshRenderer.sharedMaterial = materialOfActiveCube;
        }
    }
}
