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

    [SerializeField] Material[] fieldOfTransparentMaterials; 

    void Start()
    {
        mapGenerator = pointerCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();

        pointerCubePrefab.GetComponent<BoxCollider>().enabled = false;
        pointerCubePrefab.GetComponent<MeshRenderer>().enabled = false;
        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ShowCubePosition()
    {
        if (playerCubePlacement.CalculateUpcomingCubePosition() != null)
        {
            if (inventoryHandler.ReturnActiveCubeMaterial() != null)
            {
                //nesahat vic krat do getcomponent
                //nevolat vice krat stejnou metodu, delat to pres instance
                //ShowCubePosition volam v update < event driven programovani (actions/eventy/delegati)

                pointerCube.GetComponent<MeshRenderer>().enabled = true;

                pointerCube.GetComponent<MeshRenderer>().sharedMaterial = inventoryHandler.ReturnActiveCubeMaterial();

                Material newMaterial = new Material(inventoryHandler.ReturnActiveCubeMaterial());
                Color newColor = newMaterial.color;
                newColor.a = 0.5f;
                newMaterial.color = newColor;

                pointerCube.GetComponent<MeshRenderer>().sharedMaterial = newMaterial;

                Vector3? pointerPosition = playerCubePlacement.CalculateUpcomingCubePosition();
                pointerCube.transform.position = (Vector3)pointerPosition;
            }
        }
        else
        {
            pointerCube.GetComponent<MeshRenderer>().enabled = false;
        }
    }
}
