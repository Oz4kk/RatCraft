using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    //SerializeFields
    [SerializeField] private GameObject pointerCubePrefab;
    [SerializeField] private GameObject gameController;

    //Touching other scripts
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private InventoryHandler inventoryHandler;

    private GameObject pointerCube;
    private MeshRenderer pointerCubeMeshRenderer;


    void Start()
    {
        //Touching other scripts
        mapGenerator = pointerCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();

        //Setting pointer cube
        pointerCubePrefab.GetComponent<BoxCollider>().enabled = false;
        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
        pointerCubeMeshRenderer = pointerCube.GetComponent<MeshRenderer>();
        pointerCubeMeshRenderer.enabled = false;
    }

    public void RefreshPointerCube()
    {
        Vector3? upcomingCubePosition = playerCubePlacement.CalculateUpcomingCubePosition();
        if (upcomingCubePosition == null)
        {
            pointerCubeMeshRenderer.enabled = false;
            return;
        }
        Material materialOfActiveCube = inventoryHandler.ReturnActiveTransparentCubeMaterial();
        if (materialOfActiveCube != null)
        {
            //ShowCubePosition call in update < event driven programming (actions/events/delegats) - wip

            pointerCubeMeshRenderer.enabled = true;
            pointerCubeMeshRenderer.sharedMaterial = materialOfActiveCube;

            pointerCube.transform.position = (Vector3)upcomingCubePosition;
        }
    }
}
