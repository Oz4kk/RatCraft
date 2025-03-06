using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCubeState : IState
{
    private PlayerCubePlacement playerCubePlacement;
    private InventoryHandler inventoryHandler;
    private MapGenerator mapGenerator;

    public PlaceCubeState(PlayerCubePlacement playerCubePlacement, InventoryHandler inventoryHandler, MapGenerator mapGenerator)
    {
        this.playerCubePlacement = playerCubePlacement;
        this.inventoryHandler = inventoryHandler;
        this.mapGenerator = mapGenerator;
    }

    public void EnterState()
    {
        Vector3? raycastHitLocation = playerCubePlacement.CalculateUpcomingCubePosition();
        if (raycastHitLocation == null)
        {
            return;
        }
        if (inventoryHandler.inventory[(int)inventoryHandler.activeSlot].amount < 1)
        {
            return;
        }

        if (playerCubePlacement.DoesPlayerCollideWithCubePlacementLocation((Vector3)raycastHitLocation))
        {
            return;
        }
        Vector2 cubePositionWithoutHeight = new Vector2(raycastHitLocation.Value.x, raycastHitLocation.Value.z);
        Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(cubePositionWithoutHeight);

        GameObject cubePrefab = inventoryHandler.GetSelectedCube();
        
        GameObject actualCube = mapGenerator.InstantiateAndReturnCube((Vector3)raycastHitLocation, cubePrefab);
        CubeData actualCubeData = new CubeData(cubePrefab, (Vector3)raycastHitLocation, chunkCenter);
        mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter].Add(actualCube.transform.position, actualCubeData);

        CubeParameters actualCubeParametres = actualCube.GetComponent<CubeParameters>();
        inventoryHandler.RemoveItemFromInventory(actualCubeParametres);
        mapGenerator.onCubePlaced?.Invoke(actualCube.transform.position);
    }
}
