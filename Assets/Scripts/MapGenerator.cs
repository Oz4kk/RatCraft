using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Serializable]
    private struct GridSize
    {
        public int x;
        public int y;
        public int z;

        public GridSize(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GridSize gridSize = new GridSize(5, 5, 5);
    private float gridSpacing = 1.0f;

    GameObject[,,] mapField = null;

    void Start()
    {
        mapField = new GameObject[gridSize.x, gridSize.y, gridSize.z];
        for (int x = 0; x < mapField.GetLength(0); x++)
        {
            for (int y = 0; y < mapField.GetLength(1); y++)
            {
                for (int z = 0; z < mapField.GetLength(2); z++)
                {
                    Vector3 spawnPosition = new Vector3(x, y, z);

                    mapField[x,y,z] = Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
                }
            }
        }
    }
}
