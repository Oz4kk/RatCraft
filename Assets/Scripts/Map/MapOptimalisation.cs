using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

public class MapOptimalisation : MonoBehaviour
{
    enum Border
    {
        Null,
        XPositive,
        XNegative,
        ZPositive,
        ZNegative
    }

    enum Corner
    {
        Null,
        XPositive_ZPositive,
        XPositive_ZNegative,
        XNegative_ZPositive,
        XNegative_ZNegative
    }

    private MapGenerator mapGenerator;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();

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
        Vector3 centerOfNeighbourVector = new Vector3(centerOfUpcomingChunk.x + mapGenerator.gridSize.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z);

        // Positive X border of actual chunk
        centerOfNeighbourVector.x += mapGenerator.gridSize.x;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
            {

            }

            if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
            {

            }
        }

        // Negative X border of actual chunk
        centerOfNeighbourVector.x -= mapGenerator.gridSize.x * 4.0f;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
            {

            }

            if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
            {

            }
        }

        // Positive Z border of actual chunk
        centerOfNeighbourVector = centerOfUpcomingChunk;
        centerOfNeighbourVector.z += mapGenerator.gridSize.z;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
            {

            }

            if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
            {

            }
        }

        // Negative Z border of actual chunk
        centerOfNeighbourVector.z += mapGenerator.gridSize.z * 4.0f;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
            {

            }

            if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
            {

            }
        }

        else
        {
            DeactiavateSurroundedCubeData(actualCube, actualChunkField);
        }
    }

    private void ChooseCorner(Border border1, Border border2, ref Corner corner)
    {

    }

    private void CornerCubeOptimalization(CubeData actualCube, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> actualChunkField)
    {

    }

    private void BorderCubesOptimalization(CubeData actualCube, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> actualChunkField)
    {
        Vector3 centerOfNeighbourVector = new Vector3(centerOfUpcomingChunk.x + mapGenerator.gridSize.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z);

        // Positive X border of actual chunk
        centerOfNeighbourVector.x += mapGenerator.gridSize.x;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.z == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfNeighbourVector))
            {
                centerOfNeighbourVector.x += mapGenerator.gridSize.x;
                Dictionary<Vector3, GameObject> XPositiveCenterCubeData = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeighbourVector];

                if (XPositiveCenterCubeData.ContainsKey(actualCube.position - Vector3.right))
                {

                }
            }
        }

        // Negative X border of actual chunk
        centerOfNeighbourVector.x -= mapGenerator.gridSize.x * 4.0f;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfNeighbourVector))
            {
                centerOfNeighbourVector.x += mapGenerator.gridSize.x;
                Dictionary<Vector3, GameObject> XNegativeCenterCubeData = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeighbourVector];

                if (XNegativeCenterCubeData.ContainsKey(actualCube.position - Vector3.left))
                {

                }
            }
        }

        // Positive Z border of actual chunk
        centerOfNeighbourVector = centerOfUpcomingChunk;
        centerOfNeighbourVector.z += mapGenerator.gridSize.z;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfNeighbourVector))
            {
                centerOfNeighbourVector.z += mapGenerator.gridSize.x;
                Dictionary<Vector3, GameObject> ZPositiveCenterCubeData = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeighbourVector];

                if (ZPositiveCenterCubeData.ContainsKey(actualCube.position - Vector3.forward))
                {

                }
            }
        }

        // Negative Z border of actual chunk
        centerOfNeighbourVector.z += mapGenerator.gridSize.z * 4.0f;
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfNeighbourVector))
            {
                centerOfNeighbourVector.z += mapGenerator.gridSize.x;
                Dictionary<Vector3, GameObject> ZNegativeCenterCubeData = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeighbourVector];

                if (ZNegativeCenterCubeData.ContainsKey(actualCube.position - Vector3.back))
                {

                }
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
