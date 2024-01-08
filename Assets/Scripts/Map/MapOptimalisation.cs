using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Profiling;

public class MapOptimalisation : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private ChunkGenerator chunkGenerator;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        chunkGenerator = GetComponent<ChunkGenerator>();

        chunkGenerator.onChunkGenerated += DeactivateInvisibleCubesInChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }

    private void DeactivateInvisibleCubesInChunk()
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in mapGenerator.mapField)
        {
            if (actualCube.Value.activeInHierarchy == false)
            {
                continue;
            }

            byte counter = 0;

            if (mapGenerator.mapField.ContainsKey(actualCube.Key + Vector3.right))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(actualCube.Key - Vector3.right))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(actualCube.Key + Vector3.up))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(actualCube.Key - Vector3.up))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(actualCube.Key + Vector3.forward))
            {
                counter++;
            }
            if (mapGenerator.mapField.ContainsKey(actualCube.Key - Vector3.forward))
            {
                counter++;
            }

            if (counter == 6)
            {
                actualCube.Value.SetActive(false);
            }
        }
    }    
    
    private void DeactivateInvisibleCubesAroundPlacedCube(Vector3 targetCubePosition)
    {
        byte counter = 0;

        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.right))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.right))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.up))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.up))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.forward))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.forward))
        {
            counter++;
        }

        if (counter == 6)
        {
            mapGenerator.mapField[targetCubePosition - Vector3.forward].SetActive(true);
        }
    }

    private void RectivateInvisibleCubesAroundBrokenCube(Vector3 targetCubePosition)
    {
        Profiler.BeginSample("Reactivate cube");
        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.right))
        {
            mapGenerator.mapField[targetCubePosition + Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.right))
        {
            mapGenerator.mapField[targetCubePosition - Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.up))
        {
            mapGenerator.mapField[targetCubePosition + Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.up))
        {
            mapGenerator.mapField[targetCubePosition - Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition + Vector3.forward))
        {
            mapGenerator.mapField[targetCubePosition + Vector3.forward].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCubePosition - Vector3.forward))
        {
            mapGenerator.mapField[targetCubePosition - Vector3.forward].SetActive(true);
        }
        Profiler.EndSample();
    }
}
