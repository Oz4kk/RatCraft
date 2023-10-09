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

    void Start()
    {
        //Touching other scripts
        mapGenerator = pointerCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();
        inventoryHandler = GetComponent<InventoryHandler>();

        //Setting pointer cube
        pointerCubePrefab.GetComponent<BoxCollider>().enabled = false;
        pointerCubePrefab.GetComponent<MeshRenderer>().enabled = false;
        pointerCube = Instantiate(pointerCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ShowCubePosition()
    {
        //UGLY(Richard)
        if (playerCubePlacement.CalculateUpcomingCubePosition() == null)
        {
            pointerCube.GetComponent<MeshRenderer>().enabled = false;
            return;
        }
        if (inventoryHandler.ReturnActiveCubeMaterial() != null)
        {
            //don't touch getcomponent multiple times
            //don't call same methode multiple times, do it through the instances
            //ShowCubePosition call in update < event driven programming (actions/events/delegats)

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
}
