using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.UI.GridLayoutGroup;

public class MapOptimization : MonoBehaviour
{
    enum Corner
    {
        Null,
        XPositive_ZPositive,
        XPositive_ZNegative,
        XNegative_ZPositive,
        XNegative_ZNegative
    }

    enum Border
    {
        Null,
        XPositive,
        XNegative,
        ZPositive,
        ZNegative
    }

    private static readonly Corner[] corners = new[]
    {
        Corner.XNegative_ZNegative,
        Corner.XNegative_ZPositive,
        Corner.XNegative_ZNegative,
        Corner.XNegative_ZPositive
    };

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

    private float XNegativeCorner = 0;
    private float XPositiveCorner = 0;
    private float ZNegativeCorner = 0;
    private float ZPositiveCorner = 0;

    private MapGenerator mapGenerator;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();

        mapGenerator.onDataOfNewChunkGenerated += PrecessAllCubeDataOfUpcommingChunk;
        mapGenerator.onCubeDestroyed += RectivateInvisibleCubesAroundBrokenCube;
        mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
    }

    private void PrecessAllCubeDataOfUpcommingChunk(Dictionary<Vector3, CubeData> actualChunkField, Vector3 centerOfNewChunk)
    {
        centerOfXNegativeNeighbourChunk = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, centerOfNewChunk.y, centerOfNewChunk.z);
        centerOfXPositiveNeighbourChunk = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, centerOfNewChunk.y, centerOfNewChunk.z);
        centerOfZNegativeNeighbourChunk = new Vector3(centerOfNewChunk.x, centerOfNewChunk.y, centerOfNewChunk.z - mapGenerator.gridSize.z);
        centerOfZPositiveNeighbourChunk = new Vector3(centerOfNewChunk.x, centerOfNewChunk.y, centerOfNewChunk.z + mapGenerator.gridSize.z);

        XNegativeCorner = centerOfNewChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
        XPositiveCorner = centerOfNewChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
        ZNegativeCorner = centerOfNewChunk.z - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
        ZPositiveCorner = centerOfNewChunk.z + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;

        foreach (KeyValuePair<Vector3, CubeData> actualCube in actualChunkField)
        {
            OptimizeDataOfNewChunk(actualCube.Value, centerOfNewChunk, actualChunkField);
        }
    }

    /// <summary>
    /// Optimization of cubes that are not on the edge of the chunk and.. 
    /// </summary>
    /// <param name="newCubeData"></param>
    /// <param name="centerOfNewChunk"></param>
    /// <param name="newChunkFieldData"></param>
    private void OptimizeDataOfNewChunk(CubeData newCubeData, Vector3 centerOfNewChunk, Dictionary<Vector3, CubeData> newChunkFieldData)
    {
        Border neighborChunkBorder = Border.Null;
        Border newChunkBorder = Border.Null;

        if (newCubeData.position == new Vector3(-13, 3, -12))
        {
            string ahoj;
            ahoj = "ahoj";
        }

        // Negative X border of Actual Chunk
        // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        if ((newCubeData.position.x - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXPositiveNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfXPositiveNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.right;
                Corner newChunkCorner1 = Corner.XPositive_ZPositive;
                Corner newChunkCorner2 = Corner.XPositive_ZNegative;

                newChunkBorder = Border.XPositive;
                neighborChunkBorder = Border.XNegative;

                CornerCubeOptimizationSequence(newCubeData, centerOfNewChunk, neighborCubePosition, newChunkBorder, newChunkCorner1, newChunkCorner2);

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Positive X border of Actual Chunk
        // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfXNegativeNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfXNegativeNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.left;
                Corner newChunkCorner1 = Corner.XNegative_ZNegative;
                Corner newChunkCorner2 = Corner.XNegative_ZPositive;

                newChunkBorder = Border.XNegative;
                neighborChunkBorder = Border.XPositive;

                CornerCubeOptimizationSequence(newCubeData, centerOfNewChunk, neighborCubePosition, newChunkBorder, newChunkCorner1, newChunkCorner2);

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Negative Z border of Actual Chunk
        // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZPositiveNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfZPositiveNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.forward;
                Corner newChunkCorner1 = Corner.XNegative_ZPositive;
                Corner newChunkCorner2 = Corner.XNegative_ZPositive;

                newChunkBorder = Border.ZPositive;
                neighborChunkBorder = Border.ZNegative;

                CornerCubeOptimizationSequence(newCubeData, centerOfNewChunk, neighborCubePosition, newChunkBorder, newChunkCorner1, newChunkCorner2);

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }
        // Postive Z border of Actual Chunk
        // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
        else if ((newCubeData.position.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
        {
            if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfZNegativeNeighbourChunk))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[centerOfZNegativeNeighbourChunk];
                Vector3 neighborCubePosition = newCubeData.position + Vector3.back;
                Corner newChunkCorner1 = Corner.XNegative_ZNegative;
                Corner newChunkCorner2 = Corner.XPositive_ZNegative;

                newChunkBorder = Border.ZNegative;
                neighborChunkBorder = Border.ZPositive;

                CornerCubeOptimizationSequence(newCubeData, centerOfNewChunk, neighborCubePosition, newChunkBorder, newChunkCorner1, newChunkCorner2);

                BorderCubeOptimizationSequence(newChunkFieldData, newCubeData, newChunkBorder, neighbourChunkField, neighborCubePosition, neighborChunkBorder);

                return;
            }
        }

        DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
    }

    private void CornerCubeOptimizationSequence(CubeData newCubeData, Vector3 centerOfNewChunk, Vector3 neighborCubePosition, Border newChunkBorder, Corner newChunkCorner1, Corner newChunkCorner2)
    {
        Corner newCubeCorner = GetCornerOfNewCube(newCubeData);
        // If New Cube isn't at corner then return
        if (newCubeCorner == Corner.Null)
        {
            return;
        }

        // If there are not instantiated each correspondent chunks around New Cube Corner, then return
        if (!AreChunksAroundCornerInstantiated(centerOfNewChunk, newCubeCorner, newChunkBorder))
        {
            return;
        }

        Dictionary<Vector3, Corner> neighborCubesAroundCorner = GetNeighborCubesPositionsAroundCorner(newChunkBorder, newCubeCorner, newCubeData.position);

        Vector3? centerOfNewChunk2 = IsCornerCubeSurrounded(newCubeData.position, newCubeCorner, centerOfNewChunk);
        if (centerOfNewChunk2 != null)
        {
            newCubeData.isCubeDataSurrounded = true;
        }
        foreach (KeyValuePair<Vector3, Corner> neighborCube in neighborCubesAroundCorner)
        {
            Vector3? centerOfChunkAroundPredicted = IsCornerCubeSurrounded(newCubeData.position, newCubeCorner, centerOfNewChunk);
            if (centerOfChunkAroundPredicted != null)
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[(Vector3)centerOfChunkAroundPredicted];
                neighbourChunkField[neighborCube.Key].cubeInstance.gameObject.SetActive(false);
            }
        }
    }

    private Vector3? IsCornerCubeSurrounded(Vector3 currentCubePosition, Corner currentCorner, Vector3 centerOfNewChunk)
    {
        Vector3? centerOfCurrentChunk = new Vector3();

        foreach (Vector3 direction in directions)
        {
            centerOfCurrentChunk = IsCubeSurroundedAccordingToCorner(currentCorner, direction, currentCubePosition, centerOfNewChunk);
            if (centerOfCurrentChunk == null)
            {
                return null;
            }
        }

        return centerOfCurrentChunk;
    }

    private Dictionary<Vector3, Corner> GetNeighborCubesPositionsAroundCorner(Border newChunkBorder, Corner newCubeCorner, Vector3 newCubeDataPosition)
    {
        Dictionary<Vector3, Corner> neighborCubesPositions = new Dictionary<Vector3, Corner>();

        if (newChunkBorder == Border.XNegative || newChunkBorder == Border.XPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.left;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XPositive_ZNegative);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.back;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XNegative_ZPositive);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.left + Vector3.back);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XPositive_ZPositive);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.left;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XPositive_ZPositive);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.forward;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XNegative_ZNegative);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.left + Vector3.forward);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XPositive_ZNegative);
            }
            if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.right;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XNegative_ZNegative);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.back;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XNegative_ZPositive);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.right + Vector3.back);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XNegative_ZPositive);

            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.right;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XNegative_ZPositive);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.forward;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XPositive_ZNegative);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.right + Vector3.forward);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XNegative_ZNegative);

            }
        }
        else if (newChunkBorder == Border.ZNegative || newChunkBorder == Border.ZPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.back;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XNegative_ZPositive);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.left;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XPositive_ZNegative);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.back + Vector3.left);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XPositive_ZPositive);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.forward;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XNegative_ZNegative);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.left;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XPositive_ZPositive);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.forward + Vector3.left);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XPositive_ZNegative);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.back;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XPositive_ZPositive);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.right;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XNegative_ZNegative);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.back + Vector3.right);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XNegative_ZPositive);
            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.forward;
                neighborCubesPositions.Add(neighborCubePosition0, Corner.XPositive_ZNegative);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.right;
                neighborCubesPositions.Add(neighborCubePosition1, Corner.XNegative_ZPositive);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.forward + Vector3.right);
                neighborCubesPositions.Add(neighborCubePosition2, Corner.XNegative_ZNegative);
            }
        }

        return neighborCubesPositions;
    }

    private Vector3? IsCubeSurroundedAccordingToCorner(Corner currentCorner, Vector3 direction, Vector3 currentCubePosition, Vector3 centerOfNewChunk)
    {
        Dictionary<Vector3, CubeParameters> neighborChunkField;
        Dictionary<Vector3, CubeData> newChunkFieldData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfNewChunk];

        Vector3 predictedCubePosition;
        Vector3 predictedChunkCenter = centerOfNewChunk;

        KeyValuePair<Vector3, Corner> neighborCubePositionWithItsCorner;

        if (currentCorner == Corner.XNegative_ZNegative)
        {
            if (direction == Vector3.right)
            {
                predictedCubePosition = currentCubePosition + Vector3.right;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.forward)
            {
                predictedCubePosition = currentCubePosition + Vector3.forward;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.left)
            {
                predictedCubePosition = currentCubePosition + Vector3.left;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.x -= mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                if (neighborChunkField.ContainsKey(predictedCubePosition))
                {
                    return predictedChunkCenter;
                }
            }
            else if (direction == Vector3.back)
            {
                predictedCubePosition = currentCubePosition + Vector3.back;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.z -= mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                if (neighborChunkField.ContainsKey(predictedCubePosition))
                {
                    return predictedChunkCenter;
                }
            }
        }
        else if (currentCorner == Corner.XNegative_ZPositive)
        {
            if (direction == Vector3.left)
            {
                predictedCubePosition = currentCubePosition + Vector3.left;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.back)
            {
                predictedCubePosition = currentCubePosition + Vector3.back;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.left)
            {
                predictedCubePosition = currentCubePosition + Vector3.left;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];
                predictedChunkCenter.x -= mapGenerator.gridSize.x;

                return predictedChunkCenter;
            }
            else if (direction == Vector3.forward)
            {
                predictedCubePosition = currentCubePosition + Vector3.forward;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.z += mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                return predictedChunkCenter;
            }
        }
        else if (currentCorner == Corner.XPositive_ZNegative)
        {
            if (direction == Vector3.left)
            {
                predictedCubePosition = currentCubePosition + Vector3.left;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.forward)
            {
                predictedCubePosition = currentCubePosition + Vector3.forward;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.right)
            {
                predictedCubePosition = currentCubePosition + Vector3.right;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.x += mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                return predictedChunkCenter;
            }
            else if (direction == Vector3.back)
            {
                predictedCubePosition = currentCubePosition + Vector3.back;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.z -= mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                return predictedChunkCenter;
            }
        }
        else if (currentCorner == Corner.XPositive_ZPositive)
        {
            if (direction == Vector3.left)
            {
                predictedCubePosition = currentCubePosition + Vector3.left;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.back)
            {
                predictedCubePosition = currentCubePosition + Vector3.back;

                if (newChunkFieldData.ContainsKey(predictedCubePosition))
                {
                    return centerOfNewChunk;
                }
            }
            else if (direction == Vector3.right)
            {
                predictedCubePosition = currentCubePosition + Vector3.right;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.x += mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                return predictedChunkCenter;
            }
            else if (direction == Vector3.forward)
            {
                predictedCubePosition = currentCubePosition + Vector3.forward;
                neighborCubePositionWithItsCorner = new KeyValuePair<Vector3, Corner>(predictedCubePosition, Corner.XPositive_ZNegative);

                predictedChunkCenter.z += mapGenerator.gridSize.x;
                neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];

                return predictedChunkCenter;
            }
        }
        return null;
    }

    private Corner GetCornerOfNewCube(CubeData newCubeData)
    {
        if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZNegativeCorner)
        {
            Corner corner = Corner.XNegative_ZNegative;
            return corner;
        }
        else if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZPositiveCorner)
        {
            Corner corner = Corner.XNegative_ZPositive;
            return corner;
        }
        else if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZNegativeCorner)
        {
            Corner corner = Corner.XPositive_ZNegative;
            return corner;
        }
        else if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZPositiveCorner)
        {
            Corner corner = Corner.XPositive_ZPositive;
            return corner;
        }

        return Corner.Null;
    }

    private bool IsCornerMatchingBorder(Border actualBorder, Corner actualCorner)
    {
        if (actualBorder == Border.XNegative)
        {
            if (actualCorner == Corner.XNegative_ZNegative || actualCorner == Corner.XNegative_ZPositive)
            {
                return true;
            }
        }
        else if (actualBorder == Border.XPositive)
        {
            if (actualCorner == Corner.XPositive_ZNegative || actualCorner == Corner.XPositive_ZPositive)
            {
                return true;
            }
        }
        else if (actualBorder == Border.ZNegative)
        {
            if (actualCorner == Corner.XNegative_ZNegative || actualCorner == Corner.XPositive_ZNegative)
            {
                return true;
            }
        }
        else if (actualBorder == Border.ZPositive)
        {
            if (actualCorner == Corner.XNegative_ZPositive || actualCorner == Corner.XPositive_ZPositive)
            {
                return true;
            }
        }

        return false;
    }

    private Vector3[] GetChunkcCentersAccordingToCornerWhichIsDirectionedByBorder(Vector3 centerOfNewChunk, Corner newChunkCorner, Border border)
    {
        Vector3 predictedCenterOfChunk0 = centerOfNewChunk;
        Vector3 predictedCenterOfChunk1 = centerOfNewChunk;
        Vector3 predictedCenterOfChunk2 = centerOfNewChunk;

        if (border == Border.XNegative || border == Border.XPositive)
        {
            if (newChunkCorner == Corner.XNegative_ZNegative)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XNegative_ZPositive)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XPositive_ZNegative)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XPositive_ZPositive)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
            }
        }
        else if (border == Border.ZNegative || border == Border.ZPositive)
        {
            if (newChunkCorner == Corner.XNegative_ZNegative)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XNegative_ZPositive)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x - mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XPositive_ZNegative)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z - mapGenerator.gridSize.x);
            }
            else if (newChunkCorner == Corner.XPositive_ZPositive)
            {
                predictedCenterOfChunk0 = new Vector3(centerOfNewChunk.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
                predictedCenterOfChunk1 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z);
                predictedCenterOfChunk2 = new Vector3(centerOfNewChunk.x + mapGenerator.gridSize.x, 0.0f, centerOfNewChunk.z + mapGenerator.gridSize.x);
            }
        }

        Vector3[] fieldOfCentersAroundBorder = new Vector3[]
        {
            predictedCenterOfChunk0,
            predictedCenterOfChunk1,
            predictedCenterOfChunk2
        };

        return fieldOfCentersAroundBorder;
    }

    private bool AreChunksAroundCornerInstantiated(Vector3 centerOfNewChunk, Corner newCubeCorner, Border newChunkBorder)
    {
        Vector3[] fieldOfCentersAroundBorder = GetChunkcCentersAccordingToCornerWhichIsDirectionedByBorder(centerOfNewChunk, newCubeCorner, newChunkBorder);

        foreach (Vector3 actualPredictedCenterOfChunk in fieldOfCentersAroundBorder)
        {
            if (!mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(actualPredictedCenterOfChunk))
            {
                return false;
            }
        }

        return true;
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
                if (IsDirectionMatchingBorder(actualDirection, border))
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

    private bool IsDirectionMatchingBorder(Vector3 actualDirection, Border border)
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
