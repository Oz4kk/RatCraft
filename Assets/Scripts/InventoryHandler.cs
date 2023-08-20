using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryHandler : MonoBehaviour
{
    [SerializeField] private GameObject[] inventory;
    int activeSlot = 0;

    void Update()
    {
        ChooseItem();
    }
    private void ChooseItem()
    {
        ChooseItemViaMouseWheel();

    }

    private void Inventory()
    {
    }

    private void ChooseItemViaMouseWheel()
    {
        float mouseScroll = Input.GetAxis("Mouse ScrollWheel");

        if (mouseScroll > 0.0f)
        {
            if (activeSlot == inventory.Length)
            {
                activeSlot = 0;
            }
            else
            {
                activeSlot++;
            }
        }
        else
        {
            if (activeSlot == 0)
            {
                activeSlot = inventory.Length;
            }
            else
            {
                activeSlot--;
            }
        }
    }
}
