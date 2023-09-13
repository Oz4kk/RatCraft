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

    [SerializeField] public GameObject cubePrefab;
    [SerializeField] private GridSize gridSize = new GridSize(5, 5, 5);
    private PlayerSpawn playerSpawn;

    private Dictionary<Vector3, CubeParametres> mapField = new Dictionary<Vector3, CubeParametres>();


    private void Start()
    {
        GeneratePlainOfCubes();
    }

    private void GeneratePlainOfCubes()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 spawnPosition = new Vector3(x, y, z);
                    InstantiateCube(spawnPosition, cubePrefab);
                }
            }
        }
    }

    public void InstantiateCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject cube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            CubeParametres cubeParametres = cube.GetComponent<CubeParametres>();
            //fixnout syntaxi cp
            mapField.Add(spawnPosition, cubeParametres);
        }
        Debug.Log($"Count of mapField: {mapField.Count}");
    }
}
