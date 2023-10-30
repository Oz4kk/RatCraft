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

    [SerializeField] private float sidesPerlinScale = 0.0f;    
    [SerializeField] private float yPerlinScale = 0.1f;
    [SerializeField] private float heightLimit = 0.0f;
    [SerializeField] private float seed;

    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();

    private void Start()
    {
        GenerateMap();
    }

    private void GenerateMap()
    {
        uint debugBlueCubeCounter = 0;
        uint dubugGreenCubeCounter = 0;
        uint debugBrownCubeCounter = 0;
        uint debugPinkCubeCounter = 0;
        float[] debugSample = new float[gridSize.x * gridSize.y * gridSize.z];
        int debugSampleCounter = 0;


        for (float x = 0; x < gridSize.x; x++)
        {
            for (float y = 0; y < gridSize.y; y++)
            {
                for (float z = 0; z < gridSize.z; z++)
                {
                    float perlinValueCubes = Mathf.PerlinNoise(x * sidesPerlinScale + seed, z * sidesPerlinScale + seed);
                    
                    if (perlinValueCubes > heightLimit * y)
                    {
                        float sample = Mathf.PerlinNoise(Mathf.Floor(x/5) * sidesPerlinScale + seed, Mathf.Floor(z/5) * sidesPerlinScale + seed);
                        float sampleY = Mathf.PerlinNoise(Mathf.Floor(y/2) * yPerlinScale + seed, Mathf.Floor(y/2) * yPerlinScale + seed);

                        debugSample[debugSampleCounter++] = sample + sampleY;

                        Vector3 upcomingCubePosition = new Vector3(x, y, z);

                        if (sample + sampleY > 0.875)
                        {
                            InstantiateCube(upcomingCubePosition, greenCube);
                            dubugGreenCubeCounter++;
                        }
                        else if (sample + sampleY > 0.75)
                        {
                            InstantiateCube(upcomingCubePosition, blueCube);
                            debugBlueCubeCounter++;
                        }
                        else if (sample + sampleY > 0.675)
                        {
                            InstantiateCube(upcomingCubePosition, brownCube);
                            debugBrownCubeCounter++;
                        }
                        else
                        {
                            InstantiateCube(upcomingCubePosition, pinkCube);
                            debugPinkCubeCounter++;
                        }
                    }
                }
            }
        }
        DebugManager.Log($"DebugSample Max - {Mathf.Max(debugSample)}");
        DebugManager.Log($"DebugSample Min - {Mathf.Min(debugSample)}");
        DebugManager.Log($"Count of green cubes - {dubugGreenCubeCounter}");
        DebugManager.Log($"Count of blue cubes - {debugBlueCubeCounter}");
        DebugManager.Log($"Count of brown cubes - {debugBrownCubeCounter}");
        DebugManager.Log($"Count of pink cubes - {debugPinkCubeCounter}");
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
