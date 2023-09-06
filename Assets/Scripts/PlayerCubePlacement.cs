using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] private float maxDistance = 10000.0f;

    [SerializeField] private InventoryHandler inventoryHandler;

    MapGenerator mapGenerator = new MapGenerator();
    //Nedavat pres serialise field ale pre Start();
    //Mit input v jedny klase

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

                Debug.Log("Zasahl jsi objekt: " + hitTransform.name + " - " + hitTransform.position.x + hitTransform.position.y + hitTransform.position.z + " /// " + hitPoint.x + " | " + hitPoint.y + " | " + hitPoint.z);

                Debug.Log($"{delta.GetString()}");
                Debug.Log($"Player - {transform.position}");

                Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z);

                bool isXHighest = delta.x > delta.y && delta.x > delta.z;
                bool isYHighest = delta.y > delta.x && delta.y > delta.z;

                if (isXHighest)
                {
                    //x
                    placementLocation.x += GetSideModifier(hitTransform.position.x, hitPoint.x);
                }
                else if (isYHighest)
                {
                    //y
                    placementLocation.y += GetSideModifier(hitTransform.position.y, hitPoint.y);
                }
                else
                {
                    //z
                    placementLocation.z += GetSideModifier(hitTransform.position.z, hitPoint.z);
                }

                Cube newCube = new Cube(placementLocation, inventoryHandler.GetSelectedCube());

                if (!newCube.doesCoordinateExist(newCube.coordinates))
                {
                    mapGenerator.SetList(newCube);
                    Instantiate(inventoryHandler.GetSelectedCube(), placementLocation, Quaternion.identity);
                }
            }
        }
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
