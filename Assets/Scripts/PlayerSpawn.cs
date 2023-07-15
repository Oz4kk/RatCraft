using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnPosition;

    void Start()
    {
        GameObject spawnedPefab = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
    }
}
