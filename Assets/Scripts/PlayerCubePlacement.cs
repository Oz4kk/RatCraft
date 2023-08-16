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

                float x = Mathf.Abs((Mathf.Abs(hitPoint.x) - Mathf.Abs(hitTransform.position.x)));
                float y = Mathf.Abs((Mathf.Abs(hitPoint.y) - Mathf.Abs(hitTransform.position.y)));
                float z = Mathf.Abs((Mathf.Abs(hitPoint.z) - Mathf.Abs(hitTransform.position.z)));


                Debug.Log("Zasahl jsi objekt: " + hitTransform.name + " - " + hitTransform.position.x + hitTransform.position.y + hitTransform.position.z + " /// " + hitPoint.x + " | " + hitPoint.y + " | " + hitPoint.z);
                Debug.Log("x=" + x + " y=" + y + " z=" + z);

                if (x > y)
                {
                    if (x > z)
                    {
                        //x
                        if (Sider(hitTransform.position.x, hitPoint.x))
                        {
                            Debug.Log("1");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x + 1.0f, hitTransform.position.y, hitTransform.position.z);
                            Instantiation(placementLocation);
                        }
                        else
                        {
                            Debug.Log("2");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x - 1.0f, hitTransform.position.y, hitTransform.position.z);
                            Instantiation(placementLocation);
                        }
                    }
                    else
                    {
                        //z
                        if (Sider(hitTransform.position.z, hitPoint.z))
                        {
                            Debug.Log("3");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z + 1.0f);
                            Instantiation(placementLocation);
                        }
                        else
                        {
                            Debug.Log("4");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z - 1.0f);
                            Instantiation(placementLocation);
                        }
                    }    
                }
                else
                {
                    if (y > z)
                    {
                        //y
                        if (Sider(hitTransform.position.y, hitPoint.y))
                        {
                            Debug.Log("5");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y + 1.0f, hitTransform.position.z);
                            Instantiation(placementLocation);
                        }
                        else
                        {
                            Debug.Log("6");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y - 1.0f, hitTransform.position.z);
                            Instantiation(placementLocation);
                        }
                    }
                    else
                    {
                        //z
                        if (Sider(hitTransform.position.z, hitPoint.z))
                        {
                            Debug.Log("7");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z + 1.0f);
                            Instantiation(placementLocation);
                        }
                        else
                        {
                            Debug.Log("8");
                            Vector3 placementLocation = new Vector3(hitTransform.position.x, hitTransform.position.y, hitTransform.position.z - 1.0f);
                            Instantiation(placementLocation);
                        }
                    }
                }
            }
        }
    }

    private bool Sider(float hitTransform, float hitPoint)
    {
        float sider = hitTransform - hitPoint;
        if (sider > 0)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    private void Instantiation(Vector3 placementLocation)
    {
        Instantiate(cubeInHand, placementLocation, Quaternion.identity);
    }
}
