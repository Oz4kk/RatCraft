using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class InventoryHandler : MonoBehaviour
{
    public Action onActiveSlotChanged;
    public GameObject[] inventory;

    [SerializeField] private LayerMask solidBlockLayer;
    [SerializeField] private List<KeyCodeIndexPair> keyCodeIndexPairs = new List<KeyCodeIndexPair>();

    private int activeSlot = -1;
    private InputManager inputManager;

    private void Start()
    {
        inputManager = GetComponent<InputManager>();
        SetSlot(0);
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

    public void SetSlot(int newSlot)
    {
        if (activeSlot == newSlot)
        {
            return;
        }

        activeSlot = newSlot;
        onActiveSlotChanged?.Invoke();
        Debug.Log($"Active slot: {activeSlot}, Cube name: {inventory[activeSlot]}");
    }

    //UGLY(Richard) - Change name of the method because there isn't mentioned that cube material will change to transparent
    public Material ReturnActiveTransparentCubeMaterial()
    {
        if (inventory[activeSlot].layer != solidBlockLayer)
        {
            Material newMaterial = new Material(inventory[activeSlot].GetComponent<MeshRenderer>().sharedMaterial);
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
        //Debug.Log(inventory[activeSlot].name);
        return inventory[activeSlot];
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
