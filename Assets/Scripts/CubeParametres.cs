using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeParametres : MonoBehaviour
{
    [SerializeField] private float brittleness;

    GameObject cubePrefab;

    public CubeParametres(GameObject cubePrefab) 
    { 
        this.cubePrefab = cubePrefab;
    }
}
