using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CornerOptimization : MapOptimization
{
    void Awake()
    {
        onIsCornerCube += CornerCubeOptimizationSequence;
    }

    private void CornerCubeOptimizationSequence(CubeData newCubeData, Vector3 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeigbourCubes<Corner> neighborCubesDataAroundCorner = GetNeighborCubesDataAroundCorner(newCubeData.position, centerOfNewChunk, newChunkBorder, newCubeCorner);

        // If there are not instantiated each correspondent chunks around New Cube Corner, then return
        if (!AreChunksAroundCornerInstantiated(neighborCubesDataAroundCorner))
        {
            return;
        }

        // Check if New Cube at corresponding corner of New Chunk is surrrounded. If yes then set New Cube as surrounded, to prevent instatiating that New Cube in the future.
        if (IsCornerCubeSurrounded(newCubeData.position, centerOfNewChunk, centerOfNewChunk, newCubeCorner))
        {
            newCubeData.isCubeDataSurrounded = true;
        }

        // Check if Neighbor Cubes at corresponding corners around New Chunk are surrounded. If yes then deactive Neighbor Cube.
        foreach (var neighborCube in neighborCubesDataAroundCorner)
        {
            if (IsCornerCubeSurrounded(neighborCube.cubePosition, neighborCube.centerOfChunk, centerOfNewChunk, neighborCube.edgeType))
            {
                Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborCube.centerOfChunk];
                neighbourChunkField[neighborCube.cubePosition].cubeInstance.gameObject.SetActive(false);
            }
        }
    }

    private NeigbourCubes<Corner> GetNeighborCubesDataAroundCorner(Vector3 newCubeDataPosition, Vector3 centerOfNewChunk, Border newChunkBorder, Corner newCubeCorner)
    {
        NeigbourCubes<Corner> cornerCubes = new NeigbourCubes<Corner>();

        if (newChunkBorder == Border.XNegative || newChunkBorder == Border.XPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.left;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.back;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(0.0f, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.left + Vector3.back);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition2, centerOfNeighborChunk2);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.left;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.forward;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(0.0f, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.left + Vector3.forward);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition2, centerOfNeighborChunk2);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.right;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.back;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(0.0f, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.right + Vector3.back);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition2, centerOfNeighborChunk2);

            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.right;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.forward;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(0.0f, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.right + Vector3.forward);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition2, centerOfNeighborChunk2);
            }
        }
        else if (newChunkBorder == Border.ZNegative || newChunkBorder == Border.ZPositive)
        {
            if (newCubeCorner == Corner.XNegative_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.back;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(0.0f, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.left;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.back + Vector3.left);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition2, centerOfNeighborChunk2);
            }
            else if (newCubeCorner == Corner.XNegative_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.forward;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(0.0f, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.left;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.forward + Vector3.left);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(-mapGenerator.gridSize.x, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition2, centerOfNeighborChunk2);
            }
            else if (newCubeCorner == Corner.XPositive_ZNegative)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.back;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(0.0f, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZPositive, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.right;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.back + Vector3.right);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, -mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition2, centerOfNeighborChunk2);
            }
            else if (newCubeCorner == Corner.XPositive_ZPositive)
            {
                Vector3 neighborCubePosition0 = newCubeDataPosition + Vector3.forward;
                Vector3 centerOfNeighborChunk0 = centerOfNewChunk + new Vector3(0.0f, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XPositive_ZNegative, neighborCubePosition0, centerOfNeighborChunk0);

                Vector3 neighborCubePosition1 = newCubeDataPosition + Vector3.right;
                Vector3 centerOfNeighborChunk1 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, 0.0f);
                cornerCubes.Add(Corner.XNegative_ZPositive, neighborCubePosition1, centerOfNeighborChunk1);

                Vector3 neighborCubePosition2 = newCubeDataPosition + (Vector3.forward + Vector3.right);
                Vector3 centerOfNeighborChunk2 = centerOfNewChunk + new Vector3(mapGenerator.gridSize.x, 0.0f, mapGenerator.gridSize.x);
                cornerCubes.Add(Corner.XNegative_ZNegative, neighborCubePosition2, centerOfNeighborChunk2);
            }
        }

        return cornerCubes;
    }

    private bool AreChunksAroundCornerInstantiated(NeigbourCubes<Corner> neighborCubesAroundCorner)
    {
        foreach (NeighbourCubesData<Corner> actualPredictedCenterOfChunk in neighborCubesAroundCorner)
        {
            if (!mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(actualPredictedCenterOfChunk.centerOfChunk))
            {
                return false;
            }
        }

        return true;
    }

    private bool IsCornerCubeSurrounded(Vector3 currentCubePosition, Vector3 centerOfCurrentChunk, Vector3 centerOfNewChunk, Corner currentCorner)
    {
        foreach (Vector3 currentDirection in XZDirections)
        {
            if (!ProcessCubeAtCurrentDirection(currentCorner, currentDirection, currentCubePosition, centerOfCurrentChunk, centerOfNewChunk))
            {
                return false;
            }
        }

        return true;
    }

    private bool ProcessCubeAtCurrentDirection(Corner currentCorner, Vector3 direction, Vector3 currentCubePosition, Vector3 centerOfCurrentChunk, Vector3 centerOfNewChunk)
    {
        Dictionary<Vector3, CubeData> currentChunkFieldData = mapGenerator.dictionaryOfCentersWithItsDataChunkField[centerOfCurrentChunk];

        Vector3 predictedCubePosition = currentCubePosition + direction;

        if (IsDirectionMatchingCornerForCurrentChunkField(currentChunkFieldData, currentCorner, direction, predictedCubePosition))
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
        return false;
    }

    private bool IsDirectionMatchingCornerForCurrentChunkField(Dictionary<Vector3, CubeData> currentChunkFieldData, Corner currentCorner, Vector3 direction, Vector3 predictedCubePosition)
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

    private Vector3 SetPredictedChunkCenterAccordingToDirection(Vector3 direction, Vector3 centerOfCurrenChunk)
    {
        Vector3 predictedChunkCenter = centerOfCurrenChunk;

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
            predictedChunkCenter.z -= mapGenerator.gridSize.x;
        }
        else if (direction == Vector3.forward)
        {
            predictedChunkCenter.z += mapGenerator.gridSize.x;
        }

        return predictedChunkCenter;
    }

    // UGLY - Change Naming for this or second method!!
    private bool DoesCorrespondingCubeExistInCorrespondingChunk(Dictionary<Vector3, CubeData> currentChunkFieldData, Vector3 currentCubePosition, Vector3 predictedChunkCenter, Vector3 predictedCubePosition, Vector3 centerOfNewChunk)
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
