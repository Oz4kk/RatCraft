using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    public float cubePlacementDistance = 10000.0f;
    [SerializeField] private GameObject gameController;
    [SerializeField] private LayerMask playerLayer;

    //Touching other scripts
    private MapGenerator mapGenerator;
    private InventoryHandler inventoryHandler;
    private InputManager inputManager;

    private Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    void Awake()
    {
        inventoryHandler = GetComponent<InventoryHandler>();
        inputManager = GetComponent<InputManager>();
        mapGenerator = gameController.GetComponent<MapGenerator>();
    }

    public bool DoesPlayerCollideWithCubePlacementLocation(Vector3 placementLocation)
    {
        if (Physics.CheckBox(placementLocation, halfExtents, Quaternion.identity, playerLayer))
        {
#if UNITY_EDITOR
            VisualiseBox.DisplayBox(placementLocation, halfExtents, Quaternion.identity, playerLayer);
#endif
            return true;
        }
        return false;
    } 
}
