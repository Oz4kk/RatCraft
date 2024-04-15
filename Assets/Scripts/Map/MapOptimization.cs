using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;

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
            if (actualCube.Value.position == new Vector3(-13.0f, 4.0f, -2.0f))
            {
                string debug = "ahoj";
                debug = "aed";
            }

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

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfXPositiveNeighbourChunk, newCubeData.position + Vector3.right, border);
                BorderCubesOptimizationsOfNeighbourChunk(newCubeData, centerOfXPositiveNeighbourChunk, newCubeData.position + Vector3.right, border);
            }
        }
        // Postive X border of actual chunk
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

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfXNegativeNeighbourChunk, newCubeData.position + Vector3.left, border);
                BorderCubesOptimizationsOfNeighbourChunk(newCubeData, centerOfXNegativeNeighbourChunk, newCubeData.position + Vector3.left, border);
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

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfZPositiveNeighbourChunk, newCubeData.position + Vector3.forward, border);
                BorderCubesOptimizationsOfNeighbourChunk(newCubeData, centerOfZPositiveNeighbourChunk, newCubeData.position + Vector3.forward, border);
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

                BorderCubesOptimizationsOfNewChunk(newCubeData, newChunkFieldData, centerOfZNegativeNeighbourChunk, newCubeData.position + Vector3.back, border);
                BorderCubesOptimizationsOfNeighbourChunk(newCubeData, centerOfZNegativeNeighbourChunk, newCubeData.position + Vector3.back, border);
            }
        }

        DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
    }

    private void BorderCubesOptimizationsOfNeighbourChunk(CubeData newCubeData, Vector3 centerOfNeigbourChunk, Vector3 neighbourCubePosition, Border border)
    {
        Dictionary<Vector3, CubeParameters> neighbourChunk = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeigbourChunk];

        // Return if cube height is at 0.0f
        if (neighbourCubePosition.y == 0.0f)
        {
            return;
        }

        // Return if it's highest cube in current chunk
        if (!neighbourChunk.ContainsKey(neighbourCubePosition + Vector3.up))
        {
            return;
        }

        // Return if it's corner cube at X border
        if (border == Border.XPositive || border == Border.XNegative)
        {
            if (neighbourCubePosition.z + (mapGenerator.gridSize.z / 2) % mapGenerator.gridSize.z == 0)
            {
                return;
            }
            else if (neighbourCubePosition.z - (mapGenerator.gridSize.z / 2) % mapGenerator.gridSize.z == 0)
            {
                return;
            }
        }
        // Return if it's corner cube at Z border
        else if (border == Border.ZPositive || border == Border.ZNegative)
        {
            if (neighbourCubePosition.x + (mapGenerator.gridSize.x / 2) % mapGenerator.gridSize.x == 0)
            {
                return;
            }
            else if (neighbourCubePosition.x - (mapGenerator.gridSize.x / 2) % mapGenerator.gridSize.x == 0)
            {
                return;
            }
        }

        if (neighbourChunk[neighbourCubePosition].isCubeInstantiated == false)
        {
            return;
        }

        neighbourChunk[neighbourCubePosition].cubeInstance.SetActive(false);
    }
    
    private void BorderCubesOptimizationsOfNewChunk(CubeData newCubeData, Dictionary<Vector3, CubeData> actualChunkField, Vector3 centerOfNeigbourChunk, Vector3 neighbourCubePosition, Border border)
    {
        Dictionary<Vector3, CubeParameters> neighbourChunk = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfNeigbourChunk];

        // Return if cube height is at 0.0f
        if (newCubeData.position.y == 0.0f)
        {
            return;
        }

        // Return if it's highest cube in current chunk
        if (!actualChunkField.ContainsKey(newCubeData.position + Vector3.up))
        {
            return;
        }

        // Return if neighbour chunk don't contains cube in the same position but incremented according to direction
        if (!neighbourChunk.ContainsKey(neighbourCubePosition))
        {
            return;
        }

        // Return if it's corner cube at X border
        if (border == Border.XPositive || border == Border.XNegative)
        {
            if (newCubeData.position.z + (mapGenerator.gridSize.z / 2) % mapGenerator.gridSize.z == 0)
            {
                return;
            }
            else if (newCubeData.position.z - (mapGenerator.gridSize.z / 2) % mapGenerator.gridSize.z == 0)
            {
                return;
            }
        }
        // Return if it's corner cube at Z border
        else if (border == Border.ZPositive || border == Border.ZNegative)
        {
            if (newCubeData.position.x + (mapGenerator.gridSize.x / 2) % mapGenerator.gridSize.x == 0)
            {
                return;
            }
            else if (newCubeData.position.x - (mapGenerator.gridSize.x / 2) % mapGenerator.gridSize.x == 0)
            {
                return;
            }
        }

        newCubeData.isCubeDataSurrounded = true;
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
