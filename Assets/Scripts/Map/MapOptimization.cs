using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.UI.GridLayoutGroup;

public class MapOptimization : MonoBehaviour
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
            OptimizeDataOfNewChunk(actualCube.Value, centerOfUpcomingChunk, actualChunkField);
        }
    }

    /// <summary>
    /// Optimization of cubes that are not on the edge of the chunk and.. 
    /// </summary>
    /// <param name="newCubeData"></param>
    /// <param name="centerOfUpcomingChunk"></param>
    /// <param name="newChunkFieldData"></param>
    private void OptimizeDataOfNewChunk(CubeData newCubeData, Vector3 centerOfUpcomingChunk, Dictionary<Vector3, CubeData> newChunkFieldData)
    {
        Border border = Border.Null;
        Corner corner = Corner.Null;

        // Debug
        if (newCubeData.position == new Vector3(-13, 3, 1))
        {
            string ahoj;
            ahoj = "ahoj";
        }

        // Negative X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if ((newCubeData.position.x - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXPositiveNeighbourChunk))
            {
                border = Border.XPositive;
                // X positive - Z positive corner
                if (newCubeData.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // X positive - Z negative corner
                else if (newCubeData.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfXPositiveNeighbourChunk, newCubeData.position + Vector3.right, border, Corner.Null);
                BorderCubesOptimizationsOfNeighbourChunk(newChunkFieldData, newCubeData, centerOfXPositiveNeighbourChunk, newCubeData.position + Vector3.right, border, Corner.Null);
                return;
            }
        }
        // Positive X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXNegativeNeighbourChunk))
            {
                border = Border.XNegative;
                // X negative - Z positive corner
                if (newCubeData.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) - 1.0f)
                {

                }

                // X negative - Z negative corner
                else if (newCubeData.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfXNegativeNeighbourChunk, newCubeData.position + Vector3.left, border, Corner.Null);
                BorderCubesOptimizationsOfNeighbourChunk(newChunkFieldData, newCubeData, centerOfXNegativeNeighbourChunk, newCubeData.position + Vector3.left, border, Corner.Null);
                return;
            }
        }
        // Negative Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZPositiveNeighbourChunk))
            {
                border = Border.ZPositive;
                // Z positive - X positive corner
                if (newCubeData.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (newCubeData.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfZPositiveNeighbourChunk, newCubeData.position + Vector3.forward, border, Corner.Null);
                BorderCubesOptimizationsOfNeighbourChunk(newChunkFieldData, newCubeData, centerOfZPositiveNeighbourChunk, newCubeData.position + Vector3.forward, border, Corner.Null);
                return;
            }
        }
        // Postive Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZNegativeNeighbourChunk))
            {
                border = Border.ZNegative;
                // Z negative - X positive corner
                if (newCubeData.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (newCubeData.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfZNegativeNeighbourChunk, newCubeData.position + Vector3.back, border, Corner.Null);
                BorderCubesOptimizationsOfNeighbourChunk(newChunkFieldData, newCubeData, centerOfZNegativeNeighbourChunk, newCubeData.position + Vector3.back, border, Corner.Null);
                return;
            }
        }

        DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
    }

    private void BorderCubesOptimizationsOfNewChunk(CubeData newCubeData, Dictionary<Vector3, CubeData> newChunkFieldData, Vector3 centerOfNeigbourChunk, Vector3 neighbourCubePosition, Border border, Corner corner)
    {
        Dictionary<Vector3, CubeParameters> neighbourChunk = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeigbourChunk];

        if (corner != Corner.Null)
        {
            if (true)
            {

            }
            else if (true)
            {

            }
            else if (true)
            {

            }
            else if (true)
            {

            }
            return;
        }

        if (border != Border.XNegative)
        {
            if (!newChunkFieldData.ContainsKey(newCubeData.position - Vector3.right))
            {
                return;
            }
        }
        else
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition))
            {
                return;
            }
        }
        if (border != Border.XPositive)
        {
            if (!newChunkFieldData.ContainsKey(newCubeData.position + Vector3.right))
            {
                return;
            }
        }
        else
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition))
            {
                return;
            }
        }
        if (border != Border.ZNegative)
        {
            if (!newChunkFieldData.ContainsKey(newCubeData.position - Vector3.forward))
            {
                return;
            }
        }
        else
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition))
            {
                return;
            }
        }
        if (border != Border.ZPositive)
        {
            if (!newChunkFieldData.ContainsKey(newCubeData.position + Vector3.forward))
            {
                return;
            }
        }

        if (!newChunkFieldData.ContainsKey(newCubeData.position - Vector3.up))
        {
            return;
        }
        if (!newChunkFieldData.ContainsKey(newCubeData.position + Vector3.up))
        {
            return;
        }


        newCubeData.isCubeDataSurrounded = true;
    }

    private void BorderCubesOptimizationsOfNeighbourChunk(Dictionary<Vector3, CubeData> newChunkFieldData, CubeData newCubeData, Vector3 centerOfNeigbourChunk, Vector3 neighbourCubePosition, Border border, Corner corner)
    {
        Dictionary<Vector3, CubeParameters> neighbourChunk = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeigbourChunk];

        if (corner != Corner.Null)
        {
            if (true)
            {

            }
            else if (true)
            {

            }
            else if (true)
            {

            }
            else if (true)
            {

            }
            return;
        }

        if (border != Border.XNegative)
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition + Vector3.right))
            {
                return;
            }
            else
            {
                if (!newChunkFieldData.ContainsKey(newCubeData.position))
                {
                    return;
                }
            }

        }
        if (border != Border.XPositive)
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition - Vector3.right))
            {
                return;
            }
            else
            {
                if (!newChunkFieldData.ContainsKey(newCubeData.position))
                {
                    return;
                }
            }
        }
        if (border != Border.ZNegative)
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition + Vector3.forward))
            {
                return;
            }
            else
            {
                if (!newChunkFieldData.ContainsKey(newCubeData.position))
                {
                    return;
                }
            }
        }
        if (border != Border.ZPositive)
        {
            if (!neighbourChunk.ContainsKey(neighbourCubePosition - Vector3.forward))
            {
                return;
            }
            else
            {
                if (!newChunkFieldData.ContainsKey(newCubeData.position))
                {
                    return;
                }
            }
        }

        if (!neighbourChunk.ContainsKey(neighbourCubePosition - Vector3.up))
        {
            return;
        }
        if (!neighbourChunk.ContainsKey(neighbourCubePosition + Vector3.up))
        {
            return;
        }

        neighbourChunk[neighbourCubePosition].cubeInstance.gameObject.SetActive(false);
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
