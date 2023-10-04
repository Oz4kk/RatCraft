using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    [SerializeField] private GameObject pointerCubePrefab;

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
        pointerCubeMeshRenderer = pointerCube.GetComponent<MeshRenderer>();
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

                pointerCubeMeshRenderer.enabled = true;
                pointerCubeMeshRenderer.sharedMaterial = inventoryHandler.ReturnActiveCubeMaterial();

                Material newMaterial = new Material(inventoryHandler.ReturnActiveCubeMaterial());
                Color newColor = newMaterial.color;
                newColor.a = 0.5f;
                newMaterial.color = newColor;

                pointerCubeMeshRenderer.sharedMaterial = newMaterial;

                Vector3? pointerPosition = playerCubePlacement.CalculateUpcomingCubePosition();
                pointerCube.transform.position = (Vector3)pointerPosition;
            }
        }
        else
        {
            pointerCubeMeshRenderer.enabled = false;
        }
    }
}
