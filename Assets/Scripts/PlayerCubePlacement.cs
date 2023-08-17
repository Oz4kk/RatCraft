using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.RendererUtils;

public class PlayerCubePlacement : MonoBehaviour
{
    [SerializeField] float maxDistance = 10000.0f;
    [SerializeField] GameObject cubeInHand;

    void Update()
    {
        PlaceBlock();
    }

    private void PlaceBlock()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            //vystrelim ray od stredu obrzovky v pred
            //ray hitne block
            //spocita se v jakym smeru se hitl block
            //Porovnam abs. x s y
            //Porovnam abs. x s y
            //Porovnam abs. x s y
            //pokud je x vetsi tak porovnam abs. x se z - vetsi hodnota je vyherce
            //pokud je y vetsi tak porovnam abs. y se z - vetsi hodnota je vyherce
            //lokace spawnu pozice bloku + vypocitaby smer 
            //instantiatne to block podle gridu prilnuty k blocku co jsem hitl

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
            {
                Transform hitTransform = hit.transform;
                Vector3 hitPoint = hit.point;

                Vector3 delta = (hitPoint.Abs() - hitTransform.position.Abs()).Abs();

                Debug.Log("Zasahl jsi objekt: " + hitTransform.name + " - " + hitTransform.position.x + hitTransform.position.y + hitTransform.position.z + " /// " + hitPoint.x + " | " + hitPoint.y + " | " + hitPoint.z);
                Debug.Log($"x={delta.x} y={delta.y} z={delta.z}");
                //napsat nebo najit extension metodu pomoci ktery muzu zavolat horni debug radek ale jednodusejc
                Debug.Log($"{delta.ToString()}");

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
                Instantiate(cubeInHand, placementLocation, Quaternion.identity);
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
