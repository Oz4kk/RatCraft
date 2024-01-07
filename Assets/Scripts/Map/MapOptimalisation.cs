using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapOptimalisation : MonoBehaviour
{
    public Action onCubeInstantiated;

    private MapGenerator mapGenerator;
    private ChunkGenerator chunkGenerator;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
    }

    private void Start()
    {
        mapGenerator.onCubeDestroyed += RefreshVisibleCubes;
        chunkGenerator.onChunkGenerated += RefreshUnvisibleCubes;
    }

    public void RefreshUnvisibleCubes()
    {
        List<Vector3> listOfPositions = new List<Vector3>();

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
                mapGenerator.unloadedMapField.Add(actualCube.Key, actualCube.Value);
                Destroy(actualCube.Value);
                listOfPositions.Add(actualCube.Key);
            }
        }

        Debug.Log(mapGenerator.unloadedMapField.Count);
        foreach (var item in mapGenerator.unloadedMapField)
        {
            Debug.Log(item.Value.GetComponent<CubeParameters>().cubeType);
        }

        foreach (Vector3 actualPosition in listOfPositions)
        {
            if (mapGenerator.mapField.ContainsKey(actualPosition))
            {
                mapGenerator.mapField.Remove(actualPosition);
            }
        }
    }

    public void RefreshVisibleCubes(Vector3 actualCubePosition)
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in mapGenerator.unloadedMapField)
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
        Instantiate(actualCube, actualCube.transform.position, Quaternion.identity);
    }
}
