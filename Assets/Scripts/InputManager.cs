using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{   
    public float GetAxis(string inputName)
    {
        return Input.GetAxis(inputName);
    }

    public bool GetKeyDown(KeyCode keyCode)
    {
        return Input.GetKeyDown(keyCode);
    }

    public bool GetButtonDown(string inputName)
    {
        return Input.GetButtonDown(inputName);
    }
}
