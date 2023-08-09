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
            //pokud je x vetsi tak porovnam abs. x se z - vetsi hodnota je vyherce
            //pokud je y vetsi tak porovnam abs. y se z - vetsi hodnota je vyherce
            //lokace spawnu pozice bloku + vypocitaby smer 
            //instantiatne to block podle gridu prilnuty k blocku co jsem hitl

            RaycastHit hit;

            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, maxDistance))
            {
                //Transform hitTransform = hit.transform;
                //Vector3 hitPoint = hit.point;

                //Debug.Log("Zasahl jsi objekt: " + hitTransform.name + " " + hitTransform.position.x + hitTransform.position.y + hitTransform.position.z + " - " + hitPoint.x + " " + hitPoint.y + " " + hitPoint.z);
                //if (Mathf.Abs(hitTransform.position.x) > Mathf.Abs(hitTransform.position.y))
                //{
                //    if (Mathf.Abs(hitTransform.position.x) > Mathf.Abs(hitTransform.position.z))
                //    {
                //        Vector3 placementLocation = new Vector3(hitTransform.position.x + 1.0f, hitTransform.position.y, hitTransform.position.z);
                //        Instantiate(cubeInHand, placementLocation, Quaternion.identity);
                //    }
                //    else
                //    {
                //        Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z + 1.0f);
                //        Instantiate(cubeInHand, placementLocation, Quaternion.identity);
                //    }
                //}
                //else
                //{
                //    if (Mathf.Abs(hitTransform.position.y) > Mathf.Abs(hitTransform.position.z))
                //    {
                //        Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y + 1.0f, hitTransform.position.z);
                //        Instantiate(cubeInHand, placementLocation, Quaternion.identity);
                //    }
                //    else
                //    {
                //        Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z + 1.0f);
                //        Instantiate(cubeInHand, placementLocation, Quaternion.identity);
                //    }
                //}     


                Vector3 placementOffset = hit.normal * 1.0f; // Posunutí bloku v daném směru od hitPoint
                Vector3 placementLocation = hit.point + placementOffset;
                Instantiate(cubeInHand, placementLocation, Quaternion.identity);
            }
        }
    }
}
