using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaceCubeState : IState
{
    private PlayerCubePlacement playerCubePlacement;
    private InventoryHandler inventoryHandler;
    private MapGenerator mapGenerator;
    private InputManager inputManager;

    public PlaceCubeState(PlayerCubePlacement playerCubePlacement, InventoryHandler inventoryHandler, MapGenerator mapGenerator, InputManager inputManager)
    {
        this.playerCubePlacement = playerCubePlacement;
        this.inventoryHandler = inventoryHandler;
        this.mapGenerator = mapGenerator;
        this.inputManager = inputManager;
    }

    public void ExecuteState()
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
        GameObject actualCube = mapGenerator.InstantiateAndReturnCube((Vector3)raycastHitLocation, inventoryHandler.GetSelectedCube());
        CubeParameters actualCubeParametres = actualCube.GetComponent<CubeParameters>();

        inventoryHandler.RemoveItemFromInventory(actualCubeParametres);
    }
}
