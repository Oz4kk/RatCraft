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

    private Vector3 centerOfXPositiveNeighbourChunk = new Vector3();
    private Vector3 centerOfXNegativeNeighbourChunk = new Vector3();
    private Vector3 centerOfZPositiveNeighbourChunk = new Vector3();
    private Vector3 centerOfZNegativeNeighbourChunk = new Vector3();

    private MapGenerator mapGenerator;
    private bool isItFirstChunk = false;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();

        mapGenerator.onDataOfNewChunkGenerated += PrecessAllCubeDataOfUpcommingChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }

    private void PrecessAllCubeDataOfUpcommingChunk(Dictionary<Vector3, CubeData> actualChunkField, Vector3 centerOfUpcomingChunk)
    {
        centerOfXPositiveNeighbourChunk = new Vector3(centerOfUpcomingChunk.x + mapGenerator.gridSize.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z);
        centerOfXNegativeNeighbourChunk = new Vector3(centerOfUpcomingChunk.x - mapGenerator.gridSize.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z);
        centerOfZPositiveNeighbourChunk = new Vector3(centerOfUpcomingChunk.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z + mapGenerator.gridSize.z);
        centerOfZNegativeNeighbourChunk = new Vector3(centerOfUpcomingChunk.x, centerOfUpcomingChunk.y, centerOfUpcomingChunk.z - mapGenerator.gridSize.z);

        foreach (KeyValuePair<Vector3, CubeData> actualCube in actualChunkField)
        {
            if (actualCube.Value.position == new Vector3(-1.0f, 3.0f, 13.0f))
            {
                string debug = "ahoj";
                debug = "aed";
            }

            OptimaliseDataOfNewChunk(actualCube.Value, centerOfUpcomingChunk, actualChunkField);
        }
    }

    /// <summary>
    /// Optimization of cubes that are not on the edge of the chunk and.. 
    /// </summary>
    /// <param name="actualCube"></param>
    /// <param name="centerOfUpcomingChunk"></param>
    /// <param name="actualChunkField"></param>
    private void OptimaliseDataOfNewChunk(CubeData actualCube, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> actualChunkField)
    {
        Border border = Border.Null;
        Corner corner = Corner.Null;

        // Positive X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXPositiveNeighbourChunk))
            {
                border = Border.XPositive;
                // X positive - Z positive corner
                if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // X positive - Z negative corner
                else if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizations(actualCube, centerOfUpcomingChunk, actualChunkField, centerOfXPositiveNeighbourChunk, actualCube.position + Vector3.right, border);
            }
        }
        // Negative X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXNegativeNeighbourChunk))
            {
                border = Border.XNegative;
                // X negative - Z positive corner
                if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) - 1.0f)
                {

                }

                // X negative - Z negative corner
                else if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizations(actualCube, centerOfUpcomingChunk, actualChunkField, centerOfXNegativeNeighbourChunk, actualCube.position + Vector3.left, border);
            }
        }
        // Positive Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if (actualCube.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZPositiveNeighbourChunk))
            {
                border = Border.ZPositive;
                // Z positive - X positive corner
                if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizations(actualCube, centerOfUpcomingChunk, actualChunkField, centerOfZPositiveNeighbourChunk, actualCube.position + Vector3.forward, border);
            }
        }
        // Negative Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if (actualCube.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZNegativeNeighbourChunk))
            {
                border = Border.ZNegative;
                // Z negative - X positive corner
                if (actualCube.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (actualCube.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizations(actualCube, centerOfUpcomingChunk, actualChunkField, centerOfZNegativeNeighbourChunk, actualCube.position + Vector3.back, border);
            }
        }

        DeactiavateSurroundedCubeData(actualCube, actualChunkField);
    }

    private void ChooseCorner(Border border1, Border border2, ref Corner corner)
    {

    }

    private void BorderCubesOptimizations(CubeData actualCube, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> actualChunkField, Vector3 centerOfNeigbourChunk, Vector3 neighbourCubePosition, Border border)
    {
        Dictionary<Vector3, CubeParameters> neighbourChunk = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeigbourChunk];

        if (!neighbourChunk.ContainsKey(neighbourCubePosition))
        {
            return;
        }

        if (neighbourCubePosition.y == 0.0f)
        {
            return;
        }

        if (!neighbourChunk.ContainsKey(neighbourCubePosition + Vector3.up))
        {
            return;
        }

        if (border == Border.XPositive || border == Border.XNegative)
        {
            if (true)
            {

            }
            else if (true)
            {

            }
        }
        else if (border == Border.ZPositive || border == Border.ZNegative)
        {
            if (true)
            {

            }
            else if (true)
            {

            }
        }

        neighbourChunk[neighbourCubePosition].gameObject.SetActive(false);
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
