using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOptimalisation : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private ChunkGenerator chunkGenerator;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        chunkGenerator = GetComponent<ChunkGenerator>();

        chunkGenerator.onChunkGenerated += RefreshUnvisibleCubes;
        mapGenerator.onCubeDestroyed += RefreshVisibleCubes;
        mapGenerator.onCubePlaced += RefreshUnvisibleCubes;
    }

    private void RefreshUnvisibleCubes()
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in mapGenerator.mapField)
        {
            byte counter = 0;
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x + 1.0f, actualCube.Key.y, actualCube.Key.z)))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x - 1.0f, actualCube.Key.y, actualCube.Key.z)))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x, actualCube.Key.y + 1.0f, actualCube.Key.z)))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x, actualCube.Key.y - 1.0f, actualCube.Key.z)))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x, actualCube.Key.y, actualCube.Key.z + 1.0f)))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(new Vector3(actualCube.Key.x, actualCube.Key.y, actualCube.Key.z - 1.0f)))
            {
                counter++;
            }

            if (counter == 6 && !mapGenerator.unloadedMapField.ContainsKey(actualCube.Key))
            {
                actualCube.Value.SetActive(false);
            }
        }
    }

    private void RefreshVisibleCubes(Vector3 actualCubePosition)
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in mapGenerator.mapField)
        {
            if (actualCube.Key == new Vector3(actualCubePosition.x + 1.0f, actualCubePosition.y, actualCubePosition.z))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
            if (actualCube.Key == new Vector3(actualCubePosition.x - 1.0f, actualCubePosition.y, actualCubePosition.z))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
            if (actualCube.Key == new Vector3(actualCubePosition.x, actualCubePosition.y + 1.0f, actualCubePosition.z))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
            if (actualCube.Key == new Vector3(actualCubePosition.x, actualCubePosition.y - 1.0f, actualCubePosition.z))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
            if (actualCube.Key == new Vector3(actualCubePosition.x, actualCubePosition.y, actualCubePosition.z + 1.0f))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
            if (actualCube.Key == new Vector3(actualCubePosition.x, actualCubePosition.y, actualCubePosition.z - 1.0f))
            {
                Debug.Log(actualCube.Value.GetComponent<CubeParameters>().cubeType);
                ProcessRefreshOfUnvisibleCubes(actualCube.Value);
            }
        }
    }

    private void ProcessRefreshOfUnvisibleCubes(GameObject actualCube)
    {
        actualCube.SetActive(true);
    }
}
