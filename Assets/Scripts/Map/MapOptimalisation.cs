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

        chunkGenerator.onChunkGenerated += DeactivateInvisibleCubesInNewChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }

    private void DeactivateInvisibleCubesInNewChunk(Dictionary<Vector3, GameObject> actualChunkField)
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in actualChunkField)
        {
            DeactiavateSurroundedCube(actualCube.Value);
        }
    }

    private void DeactiavateSurroundedCube(GameObject actualCube)
    {
        byte counter = 0;

        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.right))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.right))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.up))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.up))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.forward))
        {
            counter++;
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.forward))
        {
            counter++;
        }

        if (counter == 6)
        {
            actualCube.SetActive(false);
        }
    }

    private void DeactivateInvisibleCubesAroundPlacedCube(GameObject targetCube)
    {
        //Make for cycle in which i will proceed through all these values and check if new cube hide another cubes around that cube
        
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.right]);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.right]);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.up]);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.up]);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.forward]);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.forward]);
        }
    }

    private void RectivateInvisibleCubesAroundBrokenCube(GameObject targetCube)
    {
        Profiler.BeginSample("Reactivate cube");
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.right))
        {
            mapGenerator.mapField[targetCube.transform.position + Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.right))
        {
            mapGenerator.mapField[targetCube.transform.position - Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.up))
        {
            mapGenerator.mapField[targetCube.transform.position + Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.up))
        {
            mapGenerator.mapField[targetCube.transform.position - Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.forward))
        {
            mapGenerator.mapField[targetCube.transform.position + Vector3.forward].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.forward))
        {
            mapGenerator.mapField[targetCube.transform.position - Vector3.forward].SetActive(true);
        }
        Profiler.EndSample();
    }
}
