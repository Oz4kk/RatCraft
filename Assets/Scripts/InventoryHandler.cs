using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

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

    [SerializeField] private LayerMask solidBlockLayer;
    [SerializeField] private List<KeyCodeIndexPair> keyCodeIndexPairs = new List<KeyCodeIndexPair>();
    [SerializeField] private GameObject blueCube;
    [SerializeField] private GameObject brownCube;
    [SerializeField] private GameObject greenCube;
    [SerializeField] private GameObject pinkCube;

    private int? activeSlot = null;
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
    }

    void Update()
    {
        ChooseItem();
    }
    private void ChooseItem()
    {
        ChooseCubeWithMouseScroll();
        ChooseItemWithKeyboard();
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

    private void ChooseCubeWithMouseScroll()
    {
        float mouseScroll = inputManager.GetAxis("Mouse ScrollWheel");
        if (mouseScroll == .0f)
        {
            return;
        }
        if (mouseScroll > .0f)
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

    private void ChooseItemWithKeyboard()
    {
        foreach (KeyCodeIndexPair item in keyCodeIndexPairs)
        {
            if (inputManager.GetKeyDown(item.keycode))
            {
                SetSlot(item.index);
                break;
            }
        }
    }

    [Serializable]
    private struct KeyCodeIndexPair
    {
        public KeyCode keycode;
        public int index;
    }
}
