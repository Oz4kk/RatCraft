using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParameters : MonoBehaviour
{
    public CubeType cubeType;
    public float brittleness;

    public Vector3 position;
    public bool isCubeInstantiated = false;
    
    [HideInInspector]
    public GameObject cubeInstance;
    [HideInInspector] 
    public float damage = 0;
}
