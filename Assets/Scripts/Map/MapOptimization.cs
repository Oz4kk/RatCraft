using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Threading;
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

    private static readonly Border[] borders = new[]
    {
        Border.XNegative,
        Border.XPositive,
        Border.ZNegative,
        Border.ZPositive
    };

    private static readonly Vector3[] directions = new[]
    {
        // Vertical directions
        Vector3.up, Vector3.down,
        // Horizontal X directions
        Vector3.right, Vector3.left,
        // Horizontal Z directions
        Vector3.forward, Vector3.back
    };

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
        Border neighborChunkBorder = Border.Null;
        Border newChunkBorder = Border.Null;
        Corner corner = Corner.Null;

        // Negative X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if ((newCubeData.position.x - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXPositiveNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfXPositiveNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.right;
                newChunkBorder = Border.XPositive;
                neighborChunkBorder = Border.XNegative;
                // X positive - Z positive corner
                if (newCubeData.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // X positive - Z negative corner
                else if (newCubeData.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Positive X border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXNegativeNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfXNegativeNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.left;
                newChunkBorder = Border.XNegative;
                neighborChunkBorder = Border.XPositive;
                // X negative - Z positive corner
                if (newCubeData.position.z == centerOfUpcomingChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) - 1.0f)
                {

                }

                // X negative - Z negative corner
                else if (newCubeData.position.z == centerOfUpcomingChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.z / 2.0f) + 1.0f)
                {

                }

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Negative Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZPositiveNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfZPositiveNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.forward;
                newChunkBorder = Border.ZPositive;
                neighborChunkBorder = Border.ZNegative;
                // Z positive - X positive corner
                if (newCubeData.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (newCubeData.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Postive Z border of actual chunk
        // If actual cube postion is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZNegativeNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfZNegativeNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.back;
                newChunkBorder = Border.ZNegative;
                neighborChunkBorder = Border.ZPositive;

                // Z negative - X positive corner
                if (newCubeData.position.x == centerOfUpcomingChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f)
                {

                }

                // Z negative - X negative corner
                else if (newCubeData.position.x == centerOfUpcomingChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f)
                {

                }

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }

        DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
    }

    private void BorderCubeOptimizationSequence(Dictionary<Vector3, CubeData> newChunkFieldData, CubeData newCubeData, Border newChunkBorder, Dictionary<Vector3, CubeParameters> neighbourChunkField, Vector3 neighbourCubePosition, Border neighbourChynkBorder)
    {
        // Return if New Cube in New Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeData, CubeParameters>(newChunkFieldData, newCubeData.position, neighbourChunkField, neighbourCubePosition, newChunkBorder))
        {
            return;
        }
        newCubeData.isCubeDataSurrounded = true;

        // Return if Neighbor Cube in Neighbor Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeParameters, CubeData>(neighbourChunkField, neighbourCubePosition, newChunkFieldData, newCubeData.position, neighbourChynkBorder))
        {
            return;
        }
        neighbourChunkField[neighbourCubePosition].cubeInstance.gameObject.SetActive(false);
    }

    private bool IsBorderCubeSurrounded<T, A>(Dictionary<Vector3, T> firstChunkFieldData, Vector3 firstCubePosition, Dictionary<Vector3, A> secondChunkFieldData, Vector3 secondCubePosition, Border border)
    {
        foreach (Border actaualBorder in borders)
        {
            foreach (Vector3 actualDirection in directions)
            {
                // Continue to the next foreach interation if Actual Direction is the same as Current Border
                if (isDirectionSameAsBorder(actualDirection, border))
                {
                    continue;
                }

                if (actaualBorder != border)
                {
                    if (!firstChunkFieldData.ContainsKey(firstCubePosition + actualDirection))
                    {
                        return false;
                    }
                }
                else
                {
                    if (!secondChunkFieldData.ContainsKey(secondCubePosition))
                    {
                        return false;
                    }
                }
            }
        }

        return true;
    }

    private bool isDirectionSameAsBorder(Vector3 actualDirection, Border border)
    {
        if (actualDirection == Vector3.right && border == Border.XPositive)
        {
            return true;
        }
        if (actualDirection == -(Vector3.right) && border == Border.XNegative)
        {
            return true;
        }
        if (actualDirection == Vector3.forward && border == Border.ZPositive)
        {
            return true;
        }
        if (actualDirection == -(Vector3.forward) && border == Border.ZNegative)
        {
            return true;
        }

        return false;
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
