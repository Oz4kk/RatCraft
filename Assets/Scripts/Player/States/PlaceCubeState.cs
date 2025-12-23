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
        
        GameObject cubePrefab = inventoryHandler.GetSelectedCube();
        Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(raycastHitLocation.Value.x, raycastHitLocation.Value.z));
        CubeData newCubeData = new CubeData(cubePrefab, (Vector3)raycastHitLocation, chunkCenter);
        
        CubeParameters actualCubeParametres = mapGenerator.InstantiateCube(newCubeData);
        mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter].Add(newCubeData.position, newCubeData);
        
        inventoryHandler.RemoveItemFromInventory(actualCubeParametres);
        mapGenerator.onCubePlaced?.Invoke(newCubeData);
    }
}