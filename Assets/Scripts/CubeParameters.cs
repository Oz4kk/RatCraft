using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParameters : MonoBehaviour
{
    [HideInInspector]
    public GameObject cubeInstance;
    [HideInInspector] 
    public float damage = 0;

    [SerializeField]
    private GameObject _cubePrefab;
    // Can I set cubePrefab value like this (in setter) or should it be otherwise
    public GameObject cubePrefab
    {
        get { return _cubePrefab; }
        private set { cubePrefab = _cubePrefab; }
    }

    public CubeType cubeType;
    public float brittleness;

    public bool isCubeInstantiated = false;
    public Vector3 position;
}
