using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    private MapGenerator mapGenerator;

    [SerializeField] private float sidesPerlinScale = 0.0f;
    [SerializeField] private float heightLimit = 0.0f;
    [SerializeField] private float heightPerlinScale = 0.0f;

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    public void GenerateChunk(Vector3 centerOfActualChunk)
    {
        uint debugBlueCubeCounter = 0;
        uint dubugGreenCubeCounter = 0;
        uint debugBrownCubeCounter = 0;
        uint debugPinkCubeCounter = 0;
        float[] debugSample = new float[mapGenerator.gridSize.x * mapGenerator.gridSize.y * mapGenerator.gridSize.z];

        List<bool> countOfCubesInChunk = new List<bool>();
        bool[,,] doesBlockExistOnUpcomingCoordinate = new bool[100, 16, 100];

        int debugSampleCounter = 0;

        for (int x = (int)centerOfActualChunk.x; x < mapGenerator.gridSize.x + (int)centerOfActualChunk.x; x++)
        {
            for (int y = 0; y < mapGenerator.gridSize.y; y++)
            {
                for (int z = (int)centerOfActualChunk.z; z < mapGenerator.gridSize.z + (int)centerOfActualChunk.z; z++)
                {
                    float perlinValueCubes = Mathf.PerlinNoise(x * sidesPerlinScale + mapGenerator.seed, z * sidesPerlinScale + mapGenerator.seed);

                    if (perlinValueCubes > heightLimit * y)
                    {

                        float sampleXZ = Mathf.PerlinNoise(Mathf.Floor(x / 5) * sidesPerlinScale + mapGenerator.seed, Mathf.Floor(z / 5) * sidesPerlinScale + mapGenerator.seed);
                        float sampleY = Mathf.PerlinNoise(Mathf.Floor(y / 2) * heightPerlinScale + mapGenerator.seed, Mathf.Floor(x / 2) * heightPerlinScale + mapGenerator.seed);

                        debugSample[debugSampleCounter++] = (sampleXZ + sampleY) / 2;

                        Vector3 upcomingCubePosition = new Vector3(x, y, z);

                        float resultSample = (sampleXZ + sampleY) / 2;

                        if (resultSample > 0.5)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.greenCube);
                            dubugGreenCubeCounter++;
                        }
                        else if (resultSample > 0.375)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.brownCube);
                            debugBlueCubeCounter++;
                        }
                        else if (resultSample > 0.25)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.blueCube);
                            debugBrownCubeCounter++;
                        }
                        else
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.pinkCube);
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
}
