using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InternalTypesForMapOptimization;
using UnityEngine.Profiling;

public class CornerOptimization : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private MapOptimization mapOptimization;

    void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapOptimization = GetComponent<MapOptimization>();

        mapOptimization.onIsCornerCube += CornerCubeOptimizationSequence;
    }

    private void CornerCubeOptimizationSequence(Dictionary<Vector3, CubeData> newChunkField, CubeData newCubeData, Vector2 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeighbourCubesValues<Corner>[] neighbourCubesValuesAroundCorner = GetNeighborCubesDataAroundCorner(newCubeData.position, centerOfNewChunk, newChunkBorder, newCubeCorner);

        // If there are not instantiated each correspondent chunks around New Cube Corner, then return
        if (!AreChunksAroundCornerInstantiated(neighbourCubesValuesAroundCorner))
        {
            return;
        }

        // Check if New Cube at corresponding corner of New Chunk is surrounded. If yes then set New Cube as surrounded, to prevent instatiating that New Cube in the future.
        if (IsCornerCubeSurrounded(newCubeData.position, centerOfNewChunk, newChunkField, centerOfNewChunk, newCubeCorner))
        {
            newCubeData.isCubeDataSurrounded = true;
        }
        // Check if Neighbor Cubes at corresponding corners around New Chunk are surrounded. If yes then deactive Neighbor Cube.
        foreach (NeighbourCubesValues<Corner> neighborCube in neighbourCubesValuesAroundCorner)
        {
            Dictionary<Vector3, CubeData> currentChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborCube.chunkCenter];
            
            if (IsCornerCubeSurrounded(neighborCube.position, neighborCube.chunkCenter, currentChunkField, centerOfNewChunk, neighborCube.edgeType))
            {
                Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborCube.chunkCenter];
                neighbourChunkField[neighborCube.position].cubeParameters.gameObject.SetActive(false);
            }
        }
    }

    private NeighbourCubesValues<Corner>[] GetNeighborCubesDataAroundCorner(Vector3 newCubeDataPosition, Vector2 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeighbourCubesValues<Corner> neighbourCubesValues0 = new NeighbourCubesValues<Corner>();
        NeighbourCubesValues<Corner> neighbourCubesValues1 = new NeighbourCubesValues<Corner>();
        NeighbourCubesValues<Corner> neighbourCubesValues2 = new NeighbourCubesValues<Corner>();

        if (newChunkBorder == Border.XNegative || newChunkBorder == Border.XPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                neighbourCubesValues0.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.left;

                neighbourCubesValues1.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.back;

                neighbourCubesValues2.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.left + Vector3.back);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                neighbourCubesValues0.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.left;

                neighbourCubesValues1.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.forward;

                neighbourCubesValues2.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.left + Vector3.forward);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                neighbourCubesValues0.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.right;

                neighbourCubesValues1.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.back;

                neighbourCubesValues2.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.right + Vector3.back);

            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                neighbourCubesValues0.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.right;

                neighbourCubesValues1.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.forward;

                neighbourCubesValues2.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.right + Vector3.forward);
            }
        }
        else if (newChunkBorder == Border.ZNegative || newChunkBorder == Border.ZPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                neighbourCubesValues0.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.back;

                neighbourCubesValues1.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.left;

                neighbourCubesValues2.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.back + Vector3.left);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                neighbourCubesValues0.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.forward;

                neighbourCubesValues1.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.left;

                neighbourCubesValues2.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(-mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.forward + Vector3.left);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                neighbourCubesValues0.edgeType = Corner.XPositive_ZPositive;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(0.0f, -mapGenerator.gridSize.x);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.back;

                neighbourCubesValues1.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.right;

                neighbourCubesValues2.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, -mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.back + Vector3.right);
            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                neighbourCubesValues0.edgeType = Corner.XPositive_ZNegative;
                neighbourCubesValues0.chunkCenter = centerOfNewChunk + new Vector2(0.0f, mapGenerator.gridSize.x);
                neighbourCubesValues0.position = newCubeDataPosition + Vector3.forward;

                neighbourCubesValues1.edgeType = Corner.XNegative_ZPositive;
                neighbourCubesValues1.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, 0.0f);
                neighbourCubesValues1.position = newCubeDataPosition + Vector3.right;

                neighbourCubesValues2.edgeType = Corner.XNegative_ZNegative;
                neighbourCubesValues2.chunkCenter = centerOfNewChunk + new Vector2(mapGenerator.gridSize.x, mapGenerator.gridSize.x);
                neighbourCubesValues2.position = newCubeDataPosition + (Vector3.forward + Vector3.right);
            }
        }

        NeighbourCubesValues<Corner>[] neighbourCubesValues = new NeighbourCubesValues<Corner>[3] {neighbourCubesValues0, neighbourCubesValues1, neighbourCubesValues2 };

        return neighbourCubesValues;
    }

    private bool AreChunksAroundCornerInstantiated(NeighbourCubesValues<Corner>[] neighbourCubesValuesAroundCorner)
    {
        foreach (NeighbourCubesValues<Corner> actualPredictedCenterOfChunk in neighbourCubesValuesAroundCorner)
        {
            if (!mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(actualPredictedCenterOfChunk.chunkCenter))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCornerCubeSurrounded<T>(Vector3 currentCubePosition, Vector2 centerOfCurrentChunk, Dictionary<Vector3, T> currentChunkField, Vector2 centerOfNewChunk, Corner currentCorner)
    {
        foreach (Vector3 currentDirection in mapOptimization.directions)
        {
            if (!ProcessCubeAtCurrentDirection(currentCorner, currentDirection, currentCubePosition, centerOfCurrentChunk, currentChunkField, centerOfNewChunk))
            {
                return false;
            }
        }

        return true;
    }

    private bool ProcessCubeAtCurrentDirection<T>(Corner currentCorner, Vector3 direction, Vector3 currentCubePosition, Vector2 centerOfCurrentChunk, Dictionary<Vector3, T> currentChunkField, Vector2 centerOfNewChunk)
    {
        Vector3 predictedCubePosition = currentCubePosition + direction;
        
        if (direction == Vector3.up || direction == Vector3.down)
        {
            if (DoesCubeExistInChunk(currentChunkField, predictedCubePosition))
            {
                return true;
            }
        }
        else
        {
            if (IsDirectionMatchingCornerForCurrentChunkField(currentCorner, direction))
            {
                if (DoesCubeExistInChunk(currentChunkField, predictedCubePosition))
                {
                    return true;
                }
            }
            else
            {
                Vector2 predictedChunkCenter = SetPredictedChunkCenterAccordingToDirection(direction, centerOfCurrentChunk);

                if (DoesCorrespondingCubeExistInCorrespondingChunk(currentChunkField, currentCubePosition, predictedChunkCenter, predictedCubePosition, centerOfNewChunk))
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

    private Vector2 SetPredictedChunkCenterAccordingToDirection(Vector3 direction, Vector2 centerOfCurrenChunk)
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
    private bool DoesCorrespondingCubeExistInCorrespondingChunk<T>(Dictionary<Vector3, T> currentChunkFieldData, Vector3 currentCubePosition, Vector2 predictedChunkCenter, Vector3 predictedCubePosition, Vector2 centerOfNewChunk)
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
            Dictionary<Vector3, CubeData> neighborChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[predictedChunkCenter];
            if (DoesCubeExistInChunk(neighborChunkField, predictedCubePosition))
            {
                return true;
            }
        }

        return false;
    }
}
