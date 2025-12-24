using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryUI : MonoBehaviour
{
    private InventoryHandler inventoryHandler;
    
    // Start is called before the first frame update
    void Start()
    {
        inventoryHandler = GetComponent<InventoryHandler>();
    }

    
}
