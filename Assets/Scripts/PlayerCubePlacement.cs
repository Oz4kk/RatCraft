using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10000.0f;

    private InventoryHandler inventoryHandler;
    private MapGenerator mapGenerator;
    //Mit input v jedny klase = //Napsat vlastni input manager?
    public GameObject player;

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
                    //x
                    placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
                    //doesPlayerCollideWithBlockPlacementLocation = 
                        //DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }
                else if (isYHighest)
                {
                    //y
                    placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
                    //doesPlayerCollideWithBlockPlacementLocation = 
                        //DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }
                else
                {
                    //z
                    placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
                    //doesPlayerCollideWithBlockPlacementLocation = 
                        //DoesPlayerCollideWithBlockPlacementLocation(placementLocation);
                }

                mapGenerator.InstantiateCube(placementLocation, inventoryHandler.GetSelectedCube());
            }
        }
    }

    private bool DoesPlayerCollideWithBlockPlacementLocation(Vector3 placementLocation)
    {
        Vector3 halfExtents = new Vector3(0.5f, 0.5f, 0.5f);

        RaycastHit[] hits = Physics.BoxCastAll(placementLocation, halfExtents, Vector3.up, Quaternion.identity, 1.0f);

        Gizmos.DrawWireCube(placementLocation, halfExtents);

        Debug.Log($"Placement location: {placementLocation}, Hits lengts: {hits.Length}");

        foreach (RaycastHit item in hits)
        {
            Debug.Log($"Hit: {item.collider.gameObject.name}");

            if (item.Equals(player))
            {
                return false;
            }
        }
        return true;
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
