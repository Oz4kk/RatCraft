using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkGenerator : MonoBehaviour
{
    public Action<Dictionary<Vector3, GameObject>> onChunkGenerated;

    private MapGenerator mapGenerator;

    [SerializeField] private float sidesPerlinScale = 0.0f;
    [SerializeField] private float heightLimit = 0.0f;
    [SerializeField] private float heightPerlinScale = 0.0f;

    private float grassValue = 10.0f;
    private float dirtValue = 7.0f;
    private float rockValue = 2.0f;

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    public IEnumerator GenerateChunkCoroutine(Vector3 centerOfActualChunk)
    {
        uint debugBlueCubeCounter = 0;
        uint dubugGreenCubeCounter = 0;
        uint debugBrownCubeCounter = 0;
        uint debugPinkCubeCounter = 0;

        int xCycleCounter = (int)centerOfActualChunk.x;

        Dictionary<Vector3, GameObject> actualChunkField = new Dictionary<Vector3, GameObject>();

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

                        Vector3 upcomingCubePosition = new Vector3(x, y, z);

                        float resultSample = (sampleXZ + sampleY) / 2;

                        if (resultSample > 0.5)
                        {
                            ChunkGenerationSequence(upcomingCubePosition, mapGenerator.greenCube, ref dubugGreenCubeCounter, ref actualChunkField);
                        }
                        else if (resultSample > 0.375)
                        {
                            ChunkGenerationSequence(upcomingCubePosition, mapGenerator.brownCube, ref debugBrownCubeCounter, ref actualChunkField);
                        }
                        else if (resultSample > 0.25)
                        {
                            ChunkGenerationSequence(upcomingCubePosition, mapGenerator.blueCube, ref debugBlueCubeCounter, ref actualChunkField);
                        }
                        else
                        {
                            ChunkGenerationSequence(upcomingCubePosition, mapGenerator.pinkCube, ref debugPinkCubeCounter, ref actualChunkField);
                        }
                    }
                }
                if (x > xCycleCounter + 5)
                {
                    yield return new WaitForEndOfFrame();

                    xCycleCounter = x;
                }
            }
        }
        DebugManager.Log($"Count of green cubes - {dubugGreenCubeCounter}");
        DebugManager.Log($"Count of blue cubes - {debugBlueCubeCounter}");
        DebugManager.Log($"Count of brown cubes - {debugBrownCubeCounter}");
        DebugManager.Log($"Count of pink cubes - {debugPinkCubeCounter}");

        onChunkGenerated?.Invoke(actualChunkField);
    }    
    
    public void GenerateChunkData(Vector3 centerOfActualChunk)
    {
        uint debugBlueCubeCounter = 0;
        uint dubugGreenCubeCounter = 0;
        uint debugBrownCubeCounter = 0;
        uint debugPinkCubeCounter = 0;

        Dictionary<Vector3, GameObject> actualChunkFieldData = new Dictionary<Vector3, GameObject>();

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

                        Vector3 upcomingCubePosition = new Vector3(x, y, z);

                        float resultSample = (sampleXZ + sampleY) / 2;

                        if (resultSample > 0.5)
                        {
                            ChunkDataGenerationSequence(upcomingCubePosition, mapGenerator.greenCube, ref dubugGreenCubeCounter, ref actualChunkFieldData);
                        }
                        else if (resultSample > 0.375)
                        {
                            ChunkDataGenerationSequence(upcomingCubePosition, mapGenerator.brownCube, ref debugBrownCubeCounter, ref actualChunkFieldData);
                        }
                        else if (resultSample > 0.25)
                        {
                            ChunkDataGenerationSequence(upcomingCubePosition, mapGenerator.blueCube, ref debugBlueCubeCounter, ref actualChunkFieldData);
                        }
                        else
                        {
                            ChunkDataGenerationSequence(upcomingCubePosition, mapGenerator.pinkCube, ref debugPinkCubeCounter, ref actualChunkFieldData);
                        }
                    }
                }
            }
        }
        DebugManager.Log($"Count of green cubes - {dubugGreenCubeCounter}");
        DebugManager.Log($"Count of blue cubes - {debugBlueCubeCounter}");
        DebugManager.Log($"Count of brown cubes - {debugBrownCubeCounter}");
        DebugManager.Log($"Count of pink cubes - {debugPinkCubeCounter}");
    }

    private void ChunkGenerationSequence(Vector3 upcomingCubePosition, GameObject actualCubecColor, ref uint debugActualCubeCounter, ref Dictionary<Vector3, GameObject> actualChunkField)
    {
        GameObject actualCube = mapGenerator.InstantiateAndReturnCube(upcomingCubePosition, actualCubecColor);
        actualChunkField.Add(actualCube.transform.position, actualCube);
        debugActualCubeCounter++;
        ChooseTexture(actualCube);
    }    
    
    private void ChunkDataGenerationSequence(Vector3 upcomingCubePosition, GameObject actualCubecColor, ref uint debugActualCubeCounter, ref Dictionary<Vector3, GameObject> actualChunkFieldData)
    {
        GameObject actualCube = actualCubecColor;
        actualCube.transform.position = upcomingCubePosition;
        ChooseTexture(actualCube);
        debugActualCubeCounter++;

        mapGenerator.mapFieldData.Add(actualCube.transform.position, actualCube);
    }

    private void ChooseTexture(GameObject actualCube)
    {
        Material actualMaterial = actualCube.GetComponent<Renderer>().material;
        if (actualCube.transform.position.y > grassValue)
        {
            actualMaterial.mainTexture = mapGenerator.grass;
        }
        else if (actualCube.transform.position.y > dirtValue)
        {
            actualMaterial.mainTexture = mapGenerator.dirt;
        }
        else if (actualCube.transform.position.y > rockValue)
        {
            actualMaterial.mainTexture = mapGenerator.rock;
        }
        else
        {
            actualMaterial.mainTexture = mapGenerator.sand;
        }
    }
}
