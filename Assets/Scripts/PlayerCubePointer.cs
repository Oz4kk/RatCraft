using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    [SerializeField] private GameObject pointerCubePrefab;
    [SerializeField] private GameObject gameController;
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private GameObject pointerCube;
    private InventoryHandler inventoryHandler;
    [SerializeField] float transparency = 0.1f;

    //[SerializeField] private Material materialPrefab1;
    //[SerializeField] private Material materialPrefab2;
    //[SerializeField] private Material materialPrefab3;
    //[SerializeField] private Material materialPrefab4;


    void Start()
    {
        mapGenerator = pointerCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();

        pointerCubePrefab.GetComponent<BoxCollider>().enabled = false;
        pointerCubePrefab.GetComponent<MeshRenderer>().enabled = true;
        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ShowCubePosition()
    {
        if (playerCubePlacement.CalculateUpcomingCubePosition() != null)
        {
            if (inventoryHandler.ReturnActiveCubeMaterial() != null)
            {
                pointerCube.GetComponent<MeshRenderer>().sharedMaterial = inventoryHandler.ReturnActiveCubeMaterial();
                Vector3? pointerPosition = playerCubePlacement.CalculateUpcomingCubePosition();
                pointerCube.transform.position = (Vector3)pointerPosition;
            }
        }
    }
}
