using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugManager : MonoBehaviour
{
    public bool debugManagerEnabled = false;

    //PlayerCubePlacement
    public void ShowDebugOfHittedObject(Transform hitTransform, Vector3 hitPoint)
    {
        Debug.Log($"Hitted object name: {hitTransform.name} - {hitTransform.position.GetStringOfVector3()} /// {hitPoint.GetStringOfVector3()}");
    }    
    public void ShowDebugOfDelta(Vector3 delta)
    {
        Debug.Log($"{delta.GetStringOfVector3()}");
    }

    //PlayerCubePointer
    public void ShowDebugOfPointerCube(Vector3 upcomingCubePosition)
    {
        Debug.Log($"Cube has changed location to {upcomingCubePosition.GetStringOfVector3()}");
    }

    //InventoryHandler
    public void ShowDebugOfActiveSlot(int activeSlot, GameObject[] inventory)
    {
        Debug.Log($"Active slot: {activeSlot}, Cube name: {inventory[activeSlot]}");
    }

    public void ShowDebugOfGetSelectedCube(int activeSlot, GameObject[] inventory)
    {
        Debug.Log(inventory[activeSlot].name);
    }

    //MapGenerator
    public void ShowDebugOfCubeInstatiation(Dictionary<Vector3, CubeParameters> mapField)
    {
        Debug.Log($"Count of mapField: {mapField.Count}");
    }
}
