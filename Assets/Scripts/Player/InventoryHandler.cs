using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

[Serializable]
public struct KeyCodeIndexPair
{
    public KeyCode keycode;
    public int index;
}

public enum CubeType
{
    baseCube,
    blueCube,
    brownCube,
    greenCube,
    pinkCube,
}

public class InventoryHandler : MonoBehaviour
{
    public struct Inventory
    {
        public GameObject cube;
        public byte amount;

        public Inventory(GameObject cube, byte amount)
        {
            this.cube = cube;
            this.amount = amount;
        }

    }

    public Action onActiveSlotChanged;
    public Inventory[] inventory;
    public List<KeyCodeIndexPair> keyCodeIndexPairs = new List<KeyCodeIndexPair>();
    //public Dictionary<CubeType, Inventory>

    [SerializeField] private LayerMask solidBlockLayer;
    [SerializeField] private GameObject blueCube;
    [SerializeField] private GameObject brownCube;
    [SerializeField] private GameObject greenCube;
    [SerializeField] private GameObject pinkCube;


    public int? activeSlot = null;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        SetInventory();

        SetSlot(0);
    }

    private void SetInventory()
    {
        inventory = new Inventory[4];
        inventory[0] = new Inventory(blueCube, 0);
        inventory[1] = new Inventory(brownCube, 0);
        inventory[2] = new Inventory(greenCube, 0);
        inventory[3] = new Inventory(pinkCube, 0);

        DebugShowInventory();
    }

    public byte? DoesItemExistInInventory(CubeParameters actualCube)
    {
        // Go through the inventory and return index of actualCube if there is so
        for (byte i = 0; i < inventory.Length; i++)
        {
            if (inventory[i].cube.gameObject.GetComponent<CubeParameters>().cubeType == actualCube.cubeType)
            {
                return i;
            }
        }
        return null;
    }

    public void AddNewItem(CubeParameters actualCubeName)
    {
        byte? index = DoesItemExistInInventory(actualCubeName);
        // Return if index don't exist
        if (index == null)
        {
            return;
        }
        if (inventory[(byte)index].amount >= 64)
        {
            return;
        }
        inventory[(byte)index].amount++;
        DebugShowInventory();
    }

    public void RemoveItemFromInventory(CubeParameters actualCube)
    {
        byte? index = DoesItemExistInInventory(actualCube);
        // Decrement selectedCube in inventory if there is so
        if (index == null)
        {
            return;
        }
        inventory[(byte)index].amount--;
        DebugShowInventory();
    }

    public void DebugShowInventory()
    {
        foreach (var item in inventory)
        {
            Debug.Log($"{item.cube} - {item.amount}");
        }
        Debug.Log("--------------------------------------------");
    }

    public void SetSlot(int? newSlot)
    {
        if (activeSlot == newSlot)
        {
            return;
        }

        activeSlot = newSlot;
        onActiveSlotChanged?.Invoke();

        DebugManager.Log($"Active slot: {activeSlot}, Cube name: {inventory[(int)activeSlot].cube}");
    }

    //UGLY(Richard) - Change name of the method because there isn't mentioned that cube material will change to transparent
    public Material ReturnActiveTransparentCubeMaterial()
    {
        if (inventory[(int)activeSlot].cube.layer != solidBlockLayer)
        {
            Material newMaterial = new Material(inventory[(int)activeSlot].cube.GetComponent<MeshRenderer>().sharedMaterial);
            Color newColor = newMaterial.color;
            newColor.a = 0.5f;
            newMaterial.color = newColor;

            return newMaterial;
        }
        return null;
    }

    public void ChooseCubeWithMouseScroll(float mouseScrollValue)
    {
        if (mouseScrollValue > .0f)
        {
            if (activeSlot == inventory.Length - 1)
            {
                SetSlot(0);
            }
            else
            {
                SetSlot(activeSlot + 1);
            }
        }
        else
        {
            if (activeSlot == 0)
            {
                SetSlot(inventory.Length - 1);
            }
            else
            {
                SetSlot(activeSlot - 1);
            }
        }
    }


    public GameObject GetSelectedCube()
    {
        DebugManager.Log(inventory[(int)activeSlot].cube.name);

        return inventory[(int)activeSlot].cube;
    }
}
