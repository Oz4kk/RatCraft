using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InternalTypesForMapOptimization;
using UnityEngine.Profiling;

public class CornerOptimization : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private MapOptimization mapOptimization;

    private static readonly Vector3[] directions = new[]
    {
        // Vertical Y directions
        Vector3.up, Vector3.down,
        // Horizontal X directions
        Vector3.right, Vector3.left,
        // Horizontal Z directions
        Vector3.forward, Vector3.back
    };

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapOptimization = GetComponent<MapOptimization>();

        mapOptimization.onIsCornerCube += CornerCubeOptimizationSequence;
    }

    private void CornerCubeOptimizationSequence(CubeData newCubeData, Vector2 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeighbourCubesData<Corner>[] neighbourCubesDataAroundCorner = GetNeighborCubesDataAroundCorner(newCubeData.position, centerOfNewChunk, newChunkBorder, newCubeCorner);

        // If there are not instantiated each correspondent chunks around New Cube Corner, then return
        if (!AreChunksAroundCornerInstantiated(neighbourCubesDataAroundCorner))
        {
            return;
        }

        // Check if New Cube at corresponding corner of New Chunk is surrrounded. If yes then set New Cube as surrounded, to prevent instatiating that New Cube in the future.
        if (IsCornerCubeSurrounded(newCubeData.position, centerOfNewChunk, centerOfNewChunk, newCubeCorner))
        {
            newCubeData.isCubeDataSurrounded = true;
        }

        // Check if Neighbor Cubes at corresponding corners around New Chunk are surrounded. If yes then deactive Neighbor Cube.
        foreach (NeighbourCubesData<Corner> neighborCube in neighbourCubesDataAroundCorner)
        {
            if (IsCornerCubeSurrounded(neighborCube.cubePosition, neighborCube.centerOfChunk, centerOfNewChunk, neighborCube.edgeType))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborCube.centerOfChunk];
                neighbourChunkField[neighborCube.cubePosition].cubeInstance.gameObject.SetActive(false);
            }
        }
    }

    private NeighbourCubesData<Corner>[] GetNeighborCubesDataAroundCorner(Vector3 newCubeDataPosition, Vector2 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeighbourCubesData<Corner> neighbourCubesData0 = new NeighbourCubesData<Corner>();
        NeighbourCubesData<Corner> neighbourCubesData1 = new NeighbourCubesData<Corner>();
        NeighbourCubesData<Corner> neighbourCubesData2 = new NeighbourCubesData<Corner>();

        if (newChunkBorder == Border.XNegative || newChunkBorder == Border.XPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                neighbourCubesData0.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.left;

                neighbourCubesData1.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.back;

                neighbourCubesData2.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.left + Vector3.back);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                neighbourCubesData0.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.left;

                neighbourCubesData1.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.forward;

                neighbourCubesData2.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.left + Vector3.forward);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                neighbourCubesData0.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.right;

                neighbourCubesData1.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.back;

                neighbourCubesData2.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.right + Vector3.back);

            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                neighbourCubesData0.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.right;

                neighbourCubesData1.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.forward;

                neighbourCubesData2.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.right + Vector3.forward);
            }
        }
        else if (newChunkBorder == Border.ZNegative || newChunkBorder == Border.ZPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                neighbourCubesData0.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.back;

                neighbourCubesData1.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.left;

                neighbourCubesData2.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.back + Vector3.left);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                neighbourCubesData0.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.forward;

                neighbourCubesData1.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.left;

                neighbourCubesData2.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.forward + Vector3.left);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                neighbourCubesData0.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.back;

                neighbourCubesData1.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.right;

                neighbourCubesData2.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.back + Vector3.right);
            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                neighbourCubesData0.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesData0.centerOfChunk = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesData0.cubePosition = newCubeDataPosition + Vector3.forward;

                neighbourCubesData1.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesData1.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesData1.cubePosition = newCubeDataPosition + Vector3.right;

                neighbourCubesData2.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesData2.centerOfChunk = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesData2.cubePosition = newCubeDataPosition + (Vector3.forward + Vector3.right);
            }
        }

        NeighbourCubesData<Corner>[] neighbourCubesData = new NeighbourCubesData<Corner>[3] {neighbourCubesData0, neighbourCubesData1, neighbourCubesData2 };

        return neighbourCubesData;
    }

    private bool AreChunksAroundCornerInstantiated(NeighbourCubesData<Corner>[] neighbourCubesDataAroundCorner)
    {
        foreach (NeighbourCubesData<Corner> actualPredictedCenterOfChunk in neighbourCubesDataAroundCorner)
        {
            if (!mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(actualPredictedCenterOfChunk.centerOfChunk))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCornerCubeSurrounded(Vector3 currentCubePosition, Vector2 centerOfCurrentChunk, Vector2 centerOfNewChunk, Corner currentCorner)
    {
        foreach (Vector3 currentDirection in directions)
        {
            if (!ProcessCubeAtCurrentDirection(currentCorner, currentDirection, currentCubePosition, centerOfCurrentChunk, centerOfNewChunk))
            {
                return false;
            }
        }

        return true;
    }

    private bool ProcessCubeAtCurrentDirection(Corner currentCorner, Vector3 direction, Vector3 currentCubePosition, Vector2 centerOfCurrentChunk, Vector2 centerOfNewChunk)
    {
        Dictionary<Vector3, CubeData> currentChunkFieldData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfCurrentChunk];
        Vector3 predictedCubePosition = currentCubePosition + direction;

        if (direction == Vector3.up || direction == Vector3.down)
        {
            if (DoesCubeExistInChunk(currentChunkFieldData, predictedCubePosition))
            {
                return true;
            }
        }
        else
        {
            if (IsDirectionMatchingCornerForCurrentChunkField(currentCorner, direction))
            {
                if (DoesCubeExistInChunk(currentChunkFieldData, predictedCubePosition))
                {
                    return true;
                }
            }
            else
            {
                Vector3 predictedChunkCenter = SetPredictedChunkCenterAccordingToDirection(direction, centerOfCurrentChunk);

                if (DoesCorrespondingCubeExistInCorrespondingChunk(currentChunkFieldData, currentCubePosition, predictedChunkCenter, predictedCubePosition, centerOfNewChunk))
                {
                    return true;
                }
            }
        }
        return false;
    }

    private bool IsDirectionMatchingCornerForCurrentChunkField(Corner currentCorner, Vector3 direction)
    {
        switch (currentCorner)
        {
            case Corner.XNegative_ZNegative:
                if (direction == Vector3.right || direction == Vector3.forward)
                {
                    return true;
                }
                break;

            case Corner.XNegative_ZPositive:
                if (direction == Vector3.right || direction == Vector3.back)
                {
                    return true;
                }
                break;

            case Corner.XPositive_ZNegative:
                if (direction == Vector3.left || direction == Vector3.forward)
                {
                    return true;
                }
                break;

            case Corner.XPositive_ZPositive:
                if (direction == Vector3.left || direction == Vector3.back)
                {
                    return true;
                }
                break;
        }

        return false;
    }

    private bool DoesCubeExistInChunk<T>(Dictionary<Vector3, T> chunkField, Vector3 cubePosition)
    {
        if (chunkField.ContainsKey(cubePosition))
        {
            return true;
        }
        return false;
    }

    private Vector3 SetPredictedChunkCenterAccordingToDirection(Vector3 direction, Vector2 centerOfCurrenChunk)
    {
        Vector2 predictedChunkCenter = centerOfCurrenChunk;

        if (direction == Vector3.left)
        {
            predictedChunkCenter.x -= mapGenerator.gridSize.x;
        }
        else if (direction == Vector3.right)
        {
            predictedChunkCenter.x += mapGenerator.gridSize.x;
        }
        else if (direction == Vector3.back)
        {
            predictedChunkCenter.y -= mapGenerator.gridSize.x;
        }
        else if (direction == Vector3.forward)
        {
            predictedChunkCenter.y += mapGenerator.gridSize.x;
        }

        return predictedChunkCenter;
    }

    // UGLY - Change Naming for this or second method!!
    private bool DoesCorrespondingCubeExistInCorrespondingChunk(Dictionary<Vector3, CubeData> currentChunkFieldData, Vector3 currentCubePosition, Vector2 predictedChunkCenter, Vector3 predictedCubePosition, Vector2 centerOfNewChunk)
    {
        if (predictedChunkCenter == centerOfNewChunk)
        {
            if (DoesCubeExistInChunk(currentChunkFieldData, currentCubePosition))
            {
                return true;
            }
        }
        else
        {
            Dictionary<Vector3, CubeParameters> neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];
            if (DoesCubeExistInChunk(neighborChunkField, predictedCubePosition))
            {
                return true;
            }
        }

        return false;
    }
}
