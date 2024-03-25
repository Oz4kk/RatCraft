using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParameters : MonoBehaviour
{
    public string name;
    public float brittleness;
    [HideInInspector] public float damage = 0;
    public CubeType cubeType;
    public bool isCubeDataSurrounded = false;

    public GameObject cubeInstance;
    public Vector3 position;
}
