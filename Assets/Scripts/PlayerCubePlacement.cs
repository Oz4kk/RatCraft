using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePlacement : MonoBehaviour
{
    

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


        }
    }
}
