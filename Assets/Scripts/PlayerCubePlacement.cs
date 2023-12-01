using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] private GameObject gameController;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private float cubePlacementDistance = 10000.0f;

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

    public Vector3? CalculateUpcomingCubePosition()
    {
        RaycastHit hit;
        
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, cubePlacementDistance))
        {
            Transform hitTransform = hit.transform;
            Vector3 hitPoint = hit.point;

            Vector3 delta = (hitPoint - hitTransform.position).Abs();

            DebugManager.Log($"Hitted object name: {hitTransform.name} - {hitTransform.position.GetStringOfVector3()} /// {hitPoint.GetStringOfVector3()}");
            DebugManager.Log($"{delta.GetStringOfVector3()}");

            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);

            bool isXHighest = delta.x > delta.y && delta.x > delta.z;
            bool isYHighest = delta.y > delta.x && delta.y > delta.z;

            if (isXHighest)
            {
                placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
            }
            else if (isYHighest)
            {
                placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
            }
            else
            {
                placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
            }

            return placementLocation;
        }
        return null;
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

    private int GetSideModifier(float hitTransform, float hitPoint)
    {
        if (hitTransform > hitPoint)
        {
            return -1;
        }
        else
        {
            return 1;
        }
    }
}
