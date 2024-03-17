using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

        mapGenerator.onDataOfNewChunkGenerated += PrecessAllCubeDataOfUpcommingChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }
    
    private void PrecessAllCubeDataOfUpcommingChunk(Dictionary<Vector3, CubeData> actualChunkField, Vector3 centerOfUpcomingChunk)
    {
        foreach (KeyValuePair<Vector3, CubeData> actualCube in actualChunkField)
        {
            OptimaliseDataOfNewChunk(actualCube.Value, centerOfUpcomingChunk, actualChunkField);
        }
    }

    /// <summary>
    /// Optimalise cubes that are not on the edge of the chunk and 
    /// </summary>
    /// <param name="actualCube"></param>
    /// <param name="centerOfUpcomingChunk"></param>
    /// <param name="actualChunkField"></param>
    private void OptimaliseDataOfNewChunk(CubeData actualCube, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> actualChunkField)
    {
        DeactiavateSurroundedCubeData(actualCube, actualChunkField);

        Vector3 centerOfNeighbourVector = new Vector3(centerOfUpcomingChunk.x + mapGenerator.gridSize.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z);

        // Positive X border of actual chunk
        centerOfNeighbourVector.x += mapGenerator.gridSize.x;
        if (actualCube.position.z == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfUpcomingChunk))
            {
                centerOfNeighbourVector.x += mapGenerator.gridSize.x;
                Dictionary<Vector3, CubeData> XPositiveCenterCubeData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfNeighbourVector];

            }
        }
        // Negative X border of actual chunk
        centerOfNeighbourVector.x -= mapGenerator.gridSize.x * 2.0f;
        if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfUpcomingChunk))
            {
                centerOfNeighbourVector.x += mapGenerator.gridSize.x;
                Dictionary<Vector3, CubeData> XNegativeCenterCubeData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfNeighbourVector];

            }
        }
        // Positive Z border of actual chunk
        centerOfNeighbourVector.x += mapGenerator.gridSize.x;
        centerOfNeighbourVector.z += mapGenerator.gridSize.z;
        if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfUpcomingChunk))
            {
                centerOfNeighbourVector.z += mapGenerator.gridSize.x;
                Dictionary<Vector3, CubeData> ZPositiveCenterCubeData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfNeighbourVector];

            }
        }
        // Negative Z border of actual chunk
        centerOfNeighbourVector.z += mapGenerator.gridSize.z * 2.0f;
        if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfUpcomingChunk))
            {
                centerOfNeighbourVector.z += mapGenerator.gridSize.x;
                Dictionary<Vector3, CubeData> ZNegativeCenterCubeData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfNeighbourVector];

            }
        }
    }
    private void DeactiavateSurroundedCubeData(CubeData actualCube, Dictionary<Vector3, CubeData> actualChunkField)
    {
        if (!actualChunkField.ContainsKey(actualCube.position + Vector3.right))
        {
            return;
        }
        if (!actualChunkField.ContainsKey(actualCube.position - Vector3.right))
        {
            return;
        }
        if (!actualChunkField.ContainsKey(actualCube.position + Vector3.up))
        {
            return;
        }
        if (!actualChunkField.ContainsKey(actualCube.position - Vector3.up))
        {
            return;
        }
        if (!actualChunkField.ContainsKey(actualCube.position + Vector3.forward))
        {
            return;
        }
        if (!actualChunkField.ContainsKey(actualCube.position - Vector3.forward))
        {
            return;
        }

        actualCube.isCubeDataSurrounded = true;
    }

    private void DeactiavateSurroundedCube(GameObject actualCube, Dictionary<Vector3, GameObject> actualChunkField)
    {
        byte counter = 0;

        if (actualChunkField.ContainsKey(actualCube.transform.position + Vector3.right))
        {
            counter++;
        }
        if (actualChunkField.ContainsKey(actualCube.transform.position - Vector3.right))
        {
            counter++;
        }
        if (actualChunkField.ContainsKey(actualCube.transform.position + Vector3.up))
        {
            counter++;
        }
        if (actualChunkField.ContainsKey(actualCube.transform.position - Vector3.up))
        {
            counter++;
        }
        if (actualChunkField.ContainsKey(actualCube.transform.position + Vector3.forward))
        {
            counter++;
        }
        if (actualChunkField.ContainsKey(actualCube.transform.position - Vector3.forward))
        {
            counter++;
        }

        if (counter == 6)
        {
            actualCube.SetActive(false);
        }
    }

    private void DeactivateInvisibleCubesAroundPlacedCube(GameObject actualCube)
    {        
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position + Vector3.right], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.right))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position - Vector3.right], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position + Vector3.up], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.up))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position - Vector3.up], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position + Vector3.forward], mapGenerator.mapField);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.forward))
        {
            DeactiavateSurroundedCube(mapGenerator.mapField[actualCube.transform.position - Vector3.forward], mapGenerator.mapField);
        }
    }

    private void RectivateInvisibleCubesAroundBrokenCube(GameObject actualCube)
    {
        Profiler.BeginSample("Reactivate cube");
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.right))
        {
            mapGenerator.mapField[actualCube.transform.position + Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.right))
        {
            mapGenerator.mapField[actualCube.transform.position - Vector3.right].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.up))
        {
            mapGenerator.mapField[actualCube.transform.position + Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.up))
        {
            mapGenerator.mapField[actualCube.transform.position - Vector3.up].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position + Vector3.forward))
        {
            mapGenerator.mapField[actualCube.transform.position + Vector3.forward].SetActive(true);
        }
        if (mapGenerator.mapField.ContainsKey(actualCube.transform.position - Vector3.forward))
        {
            mapGenerator.mapField[actualCube.transform.position - Vector3.forward].SetActive(true);
        }
        Profiler.EndSample();
    }
}
