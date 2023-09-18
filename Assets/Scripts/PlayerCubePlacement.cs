using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10000.0f;
    [SerializeField] private LayerMask playerLayer;

    private Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    private InventoryHandler inventoryHandler;
    private MapGenerator mapGenerator;
    //Mit input v jedny klase = //Napsat vlastni input manager?
    //public GameObject player;

    void Awake()
    {
        //Co dela metoda FindObjectOfType?
        mapGenerator = FindObjectOfType<MapGenerator>();
        inventoryHandler = GetComponent<InventoryHandler>();
    }

    void Update()
    {
        PlaceBlock();
    }

    private void PlaceBlock()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
            {
                Transform hitTransform = hit.transform;
                Vector3 hitPoint = hit.point;

                Vector3 delta = (hitPoint - hitTransform.position).Abs();

                //Debug.Log("Zasahl jsi objekt: " + hitTransform.name + " - " + hitTransform.position.x + hitTransform.position.y + hitTransform.position.z + " /// " + hitPoint.x + " | " + hitPoint.y + " | " + hitPoint.z);
                //Debug.Log($"{delta.GetString()}");

                Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);

                bool isXHighest = delta.x > delta.y && delta.x > delta.z;
                bool isYHighest = delta.y > delta.x && delta.y > delta.z;

                bool doesPlayerCollideWithBlockPlacementLocation = false;

                if (isXHighest)
                {
                    placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
                    doesPlayerCollideWithBlockPlacementLocation = DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }
                else if (isYHighest)
                {
                    placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
                    doesPlayerCollideWithBlockPlacementLocation = DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }
                else
                {
                    placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
                    doesPlayerCollideWithBlockPlacementLocation = DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }
                
                if (doesPlayerCollideWithBlockPlacementLocation)
                {
                    mapGenerator.InstantiateCube(placementLocation, inventoryHandler.GetSelectedCube());
                }
            }
        }
    }

    private bool DoesPlayerCollideWithBlockPlacementLocation(Vector3 placementLocation)
    {
        if (Physics.CheckBox(placementLocation, halfExtents, Quaternion.identity, playerLayer))
        {
            return false;
        }

        //Debug.Log($"Placement location: {placementLocation}, Hits lengts: {hits.Length}");

        return true;
    }    
    
    //private bool DoesPlayerCollideWithBlockPlacementLocation2(Vector3 placementLocation)
    //{
    //    Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

    //    Physics.OverlapBox(placementLocation, halfExtents, Quaternion.identity, 7);

    //    //Debug.Log($"Placement location: {placementLocation}, Hits lengts: {hits.Length}");

    //    return true;
    //}

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
