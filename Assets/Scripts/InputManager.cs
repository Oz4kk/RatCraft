using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public float GetMouseScrollWheelInput()
    {
        return Input.GetAxis("Mouse ScrollWheel");
    }
}
