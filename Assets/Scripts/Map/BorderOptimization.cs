using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InternalTypesForMapOptimization;

public class BorderOptimization : MonoBehaviour
{
    private MapGenerator mapGenerator;
    private MapOptimization mapOptimization;

    private static readonly Border[] borders = new[]
    {
        Border.XNegative,
        Border.XPositive,
        Border.ZNegative,
        Border.ZPositive
    };

    private static readonly Vector3[] xzDirections = new[]
    {
        // Vertical Y direction
        Vector3.up, Vector3.down,
        // Horizontal X directions
        Vector3.right, Vector3.left,
        // Horizontal Z directions
        Vector3.forward, Vector3.back
    };

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapOptimization = GetComponent<MapOptimization>(); 

        mapOptimization.onIsBorderCube += BorderCubeOptimizationSequence;
    }

    private void BorderCubeOptimizationSequence(Dictionary<Vector3, CubeData> newChunkFieldData, CubeData newCubeData, Border newChunkBorder, Vector3 centerOfXNegativeNeighbourChunk, Vector3 centerOfXPositiveNeighbourChunk, Vector3 centerOfZNegativeNeighbourChunk, Vector3 centerOfZPositiveNeighbourChunk)
    {
        NeighbourCubesData<Border> neighbourCubesData = SetNeighborChunkValues(newChunkBorder, newCubeData.position, centerOfXNegativeNeighbourChunk, centerOfXPositiveNeighbourChunk, centerOfZNegativeNeighbourChunk, centerOfZPositiveNeighbourChunk);

        if (!DoesNeighborChunkExist(neighbourCubesData.centerOfChunk))
        {
            return;
        }

        Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighbourCubesData.centerOfChunk];
        // Return if New Cube in New Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeData, CubeParameters>(newChunkFieldData, newCubeData.position, neighbourChunkField, neighbourCubesData.cubePosition, newChunkBorder))
        {
            return;
        }
        newCubeData.isCubeDataSurrounded = true;

        // Return if Neighbor Cube in Neighbor Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeParameters, CubeData>(neighbourChunkField, neighbourCubesData.cubePosition, newChunkFieldData, newCubeData.position, neighbourCubesData.edgeType))
        {
            return;
        }
        neighbourChunkField[neighbourCubesData.cubePosition].cubeInstance.gameObject.SetActive(false);
    }

    private NeighbourCubesData<Border> SetNeighborChunkValues(Border newChunkBorder, Vector3 newCubeDataPosition, Vector3 centerOfXNegativeNeighbourChunk, Vector3 centerOfXPositiveNeighbourChunk, Vector3 centerOfZNegativeNeighbourChunk, Vector3 centerOfZPositiveNeighbourChunk)
    {
        NeighbourCubesData<Border> neighbourCubesData = new NeighbourCubesData<Border>();

        switch (newChunkBorder)
        {
            case Border.XNegative:
                neighbourCubesData.edgeType = Border.XPositive;
                neighbourCubesData.centerOfChunk = centerOfXNegativeNeighbourChunk;
                neighbourCubesData.cubePosition = newCubeDataPosition + Vector3.left;
                break;
            case Border.XPositive:
                neighbourCubesData.edgeType = Border.XNegative;
                neighbourCubesData.centerOfChunk = centerOfXPositiveNeighbourChunk;
                neighbourCubesData.cubePosition = newCubeDataPosition + Vector3.right;
                break;
            case Border.ZNegative:
                neighbourCubesData.edgeType = Border.ZPositive;
                neighbourCubesData.centerOfChunk = centerOfZNegativeNeighbourChunk;
                neighbourCubesData.cubePosition = newCubeDataPosition + Vector3.back;
                break;
            case Border.ZPositive:
                neighbourCubesData.edgeType = Border.ZNegative;
                neighbourCubesData.centerOfChunk = centerOfZPositiveNeighbourChunk;
                neighbourCubesData.cubePosition = newCubeDataPosition + Vector3.forward;
                break;
        }

        return neighbourCubesData;
    }

    private bool DoesNeighborChunkExist(Vector3 neighborChunkCenter)
    {
        if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(neighborChunkCenter))
        {
            return true;
        }

        return false;
    }

    private bool IsBorderCubeSurrounded<T, A>(Dictionary<Vector3, T> firstChunkFieldData, Vector3 firstCubePosition, Dictionary<Vector3, A> secondChunkFieldData, Vector3 secondCubePosition, Border border)
    {
        foreach (Border actaualBorder in borders)
        {
            foreach (Vector3 actualDirection in xzDirections)
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
}
