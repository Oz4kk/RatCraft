using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    [SerializeField] private GameObject baseCubePrefab;
    [SerializeField] private GameObject gameController;
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private GameObject pointerCube;
    private InventoryHandler inventoryHandler;

    [SerializeField] private Material materialPrefab1;
    [SerializeField] private Material materialPrefab2;
    [SerializeField] private Material materialPrefab3;
    [SerializeField] private Material materialPrefab4;


    void Start()
    {
        mapGenerator = baseCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();

        baseCubePrefab.GetComponent<BoxCollider>().enabled = false;
        baseCubePrefab.GetComponent<MeshRenderer>().enabled = true;
        pointerCube = Instantiate(baseCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ShowCubePosition()
    {
        pointerCube.GetComponent<MeshRenderer>().sharedMaterial = inventoryHandler.ReturnActiveBlockMaterial();
        Vector3 pointerPosition = playerCubePlacement.PrepareBlock();
        pointerCube.transform.position = pointerPosition;
    }
}
