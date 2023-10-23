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

    [SerializeField] public GameObject greenCube;
    [SerializeField] public GameObject blueCube;
    [SerializeField] public GameObject brownCube;
    [SerializeField] public GameObject pinkCube;
    [SerializeField] private GridSize gridSize = new GridSize(5, 5, 5);

    //[Range(0.0f, 0.9f)]
    [SerializeField] private float xPerlinScale = 0.0f;    

    //[Range(0.0f, 0.9f)]
    [SerializeField] private float zPerlinScale = 0.0f;

    [SerializeField] private float heightLimit = 0.0f;
    [SerializeField] private int seed = 0;


    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        float a = -gridSize.x * xPerlinScale;
        //float a = UnityEngine.Random.Range(0,500);
        for (float x = 0; x < gridSize.x; x++)
        {
            for (float y = 0; y < gridSize.y; y++)
            {
                for (float z = 0; z < gridSize.z; z++)
                {
                    float perlinValue = Mathf.PerlinNoise(x * xPerlinScale + a, z * zPerlinScale + a);
                    
                    if (perlinValue > heightLimit * y)
                    {
                        float sample = Mathf.PerlinNoise(Mathf.Floor(z/5) * zPerlinScale + a, Mathf.Floor(x/5) * xPerlinScale + a);
                        Vector3 upcomingCubePosition = new Vector3(x, y, z);
                        if (sample > 0.75f)
                        {
                            InstantiateCube(upcomingCubePosition, greenCube);
                        }
                        else if (sample > 0.50f)
                        {
                            InstantiateCube(upcomingCubePosition, blueCube);
                        }
                        else if (sample > 0.25f)
                        {
                            InstantiateCube(upcomingCubePosition, brownCube);
                        }
                        else
                        {
                            InstantiateCube(upcomingCubePosition, pinkCube);
                        }
                    }
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

        DebugManager.Log($"Count of mapField: {mapField.Count}");
    }    
}
