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

        mapGenerator.onMegaChunkFullFilled += DeactivateInvisibleCubesInNewChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }

    private void DeactivateInvisibleCubesInNewChunk(Dictionary<Vector3, GameObject> actualChunkField)
    {
        foreach (KeyValuePair<Vector3, GameObject> actualCube in actualChunkField)
        {
            DeactiavateSurroundedCube(actualCube.Value, mapGenerator.mapField);
        }
    }

    private void DeactiavateSurroundedCube(GameObject actualCube, Dictionary<Vector3, GameObject> targetFieldOfCubes)
    {
        byte counter = 0;

        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position + Vector3.right))
        {
            counter++;
        }
        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position - Vector3.right))
        {
            counter++;
        }
        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position + Vector3.up))
        {
            counter++;
        }
        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position - Vector3.up))
        {
            counter++;
        }
        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position + Vector3.forward))
        {
            counter++;
        }
        if (targetFieldOfCubes.ContainsKey(actualCube.transform.position - Vector3.forward))
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
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.right], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.right], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.up], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.up], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position + Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position + Vector3.forward], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(targetCube.transform.position - Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[targetCube.transform.position - Vector3.forward], mapGenerator.mapField);
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
