using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    [Serializable]
    public struct GridSize
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

    private MapGenerator mapGenerator;

    [SerializeField] private float sidesPerlinScale = 0.0f;
    [SerializeField] private float heightLimit = 0.0f;
    [SerializeField] private float heightPerlinScale = 0.0f;

    private GridSize gridSize = new GridSize(100, 16, 100);

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
        float[] debugSample = new float[gridSize.x * gridSize.y * gridSize.z];

        List<bool> countOfCubesInChunk = new List<bool>();
        bool[,,] doesBlockExistOnUpcomingCoordinate = new bool[100,16,100];

        for (float x = 0; x < gridSize.x; x++)
        {
            for (float y = 0; y < gridSize.y; y++)
            {
                for (float z = 0; z < gridSize.z; z++)
                {
                    float perlinValueCubes = Mathf.PerlinNoise(x * sidesPerlinScale + mapGenerator.seed, z * sidesPerlinScale + mapGenerator.seed);

                    if (perlinValueCubes > heightLimit * y)
                    {

                        float sampleXZ = Mathf.PerlinNoise(Mathf.Floor(x / 5) * sidesPerlinScale + mapGenerator.seed, Mathf.Floor(z / 5) * sidesPerlinScale + mapGenerator.seed);
                        float sampleY = Mathf.PerlinNoise(Mathf.Floor(y / 2) * heightPerlinScale + mapGenerator.seed, Mathf.Floor(y / 2) * heightPerlinScale + mapGenerator.seed);     
                        
                        //0.43423
                        //0.5343

                        //debugSample[debugSampleCounter++] = sampleXZ + sampleY;
                        //debugSample[debugSampleCounter++] = sampleY;
                        //if (sampleY != debug2)
                        //{
                        //    Debug.Log(sampleY);
                        //    debug2 = sampleY;
                        //}

                        Vector3 upcomingCubePosition = new Vector3(x + centerOfActualChunk.x, y, z + centerOfActualChunk.z);

                        if (sampleXZ + sampleY > 0.875)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.greenCube);
                            dubugGreenCubeCounter++;
                        }
                        else if (sampleXZ + sampleY > 0.75)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.blueCube);
                            debugBlueCubeCounter++;
                        }
                        else if (sampleXZ + sampleY > 0.625)
                        {
                            mapGenerator.InstantiateCube(upcomingCubePosition, mapGenerator.brownCube);
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
