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


    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();
    private PlayerSpawn playerSpawn;


    private void Start()
    {
        GeneratePlainOfCubes();
    }



    private void GeneratePlainOfCubes()
    {
        for (float x = 0; x < gridSize.x; x++)
        {
            for (float y = 0; y < gridSize.y; y++)
            {
                for (float z = 0; z < gridSize.z; z++)
                {
                    float sample = Mathf.PerlinNoise(x * 0.1f, z * 0.1f);
                    if (sample > 0.2f * y)
                    {
                        Vector3 spawnPosition = new Vector3(x, y, z);
                        InstantiateCube(spawnPosition, cubePrefab);
                    }
                    Debug.Log(sample);
                }
            }
        }
    }

    public void InstantiateCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject cube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            CubeParameters cubeParameters = cube.GetComponent<CubeParameters>();
            mapField.Add(spawnPosition, cubeParameters);
        }
        Debug.Log($"Count of mapField: {mapField.Count}");
    }
}
