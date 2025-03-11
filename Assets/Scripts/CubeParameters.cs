using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParameters : MonoBehaviour
{
    // When Instantiating cube cubeData & position must be set!
    
    public CubeType cubeType;
    public float brittleness;
    [HideInInspector] 
    public float damage = 0;
    
    public Vector3 position;
    [HideInInspector]
    public CubeData cubeData;
}
