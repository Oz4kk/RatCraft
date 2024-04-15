using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParameters : MonoBehaviour
{
    [HideInInspector]
    public GameObject cubeInstance;
    [HideInInspector] 
    public float damage = 0;

    public CubeType cubeType;
    public float brittleness;

    public bool isCubeInstantiated = false;
    public Vector3 position;
}
