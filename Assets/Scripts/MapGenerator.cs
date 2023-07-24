using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private Vector3 gridSize = new Vector3(5.0f, 5.0f, 5.0f);
    private float gridSpacing = 2.0f;

    void Start()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 spawnPosition = new Vector3(x, y, z) * gridSpacing;

                    GameObject cube = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }

    void Update()
    {
        
    }
}
