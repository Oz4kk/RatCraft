using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyCubeState : IState
{
    private InputManager inputManager;
    private MapGenerator mapGenerator;
    private InventoryHandler inventoryHandler;
    private float cubeBreakDistance;

    public DestroyCubeState(InputManager inputManager, MapGenerator mapGenerator, InventoryHandler inventoryHandler, float cubeBreakDistance)
    {
        this.inputManager = inputManager;
        this.mapGenerator = mapGenerator;
        this.inventoryHandler = inventoryHandler;
        this.cubeBreakDistance = cubeBreakDistance;
    }

    public void EnterState()
    {
        // Return if raycast don't hit anything
        RaycastHit hit;
        if (!Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, cubeBreakDistance))
        {
            return;
        }
        // Ensure that you are really hitting a Cube
        CubeParameters actualCube = hit.transform.gameObject.GetComponent<CubeParameters>();
        if (actualCube == null)
        {
            return;
        }
        DestroyCubeSequence(actualCube);
    }

    private void DestroyCubeSequence(CubeParameters actualCube)
    {
        Debug.Log($"{actualCube.GetType()} - {actualCube.damage}");
        actualCube.damage += Time.deltaTime;
        // Return if damage is lower than brittenes of hitted cube
        if (actualCube.damage <= actualCube.brittleness)
        {
            return;
        }
        // Delete Cube in the world, remove it from the corresponding chunkField dictionary, add increment ammount of hitted cube in the inventory and set mining time to 0
        mapGenerator.DeleteCube(actualCube);
        inventoryHandler.AddNewItem(actualCube);
    }
}
