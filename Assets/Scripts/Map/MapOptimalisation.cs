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

        chunkGenerator.onChunkGenerated += ActivateVisibleCubes;
        mapGenerator.onCubeDestroyed += DectivateUnvisibleCubes;
        mapGenerator.onCubePlaced += ActivateVisibleCubes;
    }

    private void ActivateVisibleCubes()
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

    private void DectivateUnvisibleCubes(Vector3 targetCubePosition)
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in mapGenerator.mapField)
        {
            if (actualCube.Value.activeInHierarchy == true)
            {
                continue;
            }

            if (actualCube.Key == targetCubePosition + Vector3.right)
            {
                actualCube.Value.SetActive(true);
            }
            if (actualCube.Key == targetCubePosition - Vector3.right)
            {
                actualCube.Value.SetActive(true);
            }
            if (actualCube.Key == targetCubePosition + Vector3.up)
            {
                actualCube.Value.SetActive(true);
            }
            if (actualCube.Key == targetCubePosition - Vector3.up)
            {
                actualCube.Value.SetActive(true);
            }
            if (actualCube.Key == targetCubePosition + Vector3.forward)
            {
                actualCube.Value.SetActive(true);
            }
            if (actualCube.Key == targetCubePosition - Vector3.forward)
            {
                actualCube.Value.SetActive(true);
            }
        }
    }
}
