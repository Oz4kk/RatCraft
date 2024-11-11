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

    private static readonly Vector3[] directions = new[]
    {
        // Vertical directions
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
        Border neighbourChunkBorder = Border.Null;
        Vector3 neighbourCubePosition = Vector3.zero;
        Vector3 neighbourChunkCenter = Vector3.zero;

        SetNeighborChunkValues(newChunkBorder, newCubeData.position, ref neighbourChunkBorder, ref neighbourCubePosition, ref neighbourChunkCenter, centerOfXNegativeNeighbourChunk, centerOfXPositiveNeighbourChunk, centerOfZNegativeNeighbourChunk, centerOfZPositiveNeighbourChunk);

        if (!DoesNeighborChunkExist(neighbourChunkCenter))
        {
            return;
        }

        Dictionary<Vector3, CubeParameters> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighbourChunkCenter];
        // Return if New Cube in New Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeData, CubeParameters>(newChunkFieldData, newCubeData.position, neighbourChunkField, neighbourCubePosition, newChunkBorder))
        {
            return;
        }
        newCubeData.isCubeDataSurrounded = true;

        // Return if Neighbor Cube in Neighbor Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded<CubeParameters, CubeData>(neighbourChunkField, neighbourCubePosition, newChunkFieldData, newCubeData.position, neighbourChunkBorder))
        {
            return;
        }
        neighbourChunkField[neighbourCubePosition].cubeInstance.gameObject.SetActive(false);
    }

    private void SetNeighborChunkValues(Border newChunkBorder, Vector3 newCubeDataPosition, ref Border neighborChunkBorder, ref Vector3 neighborCubePosition, ref Vector3 neighbourChunkCenter, Vector3 centerOfXNegativeNeighbourChunk, Vector3 centerOfXPositiveNeighbourChunk, Vector3 centerOfZNegativeNeighbourChunk, Vector3 centerOfZPositiveNeighbourChunk)
    {
        switch (newChunkBorder)
        {
            case Border.XNegative:
                neighborChunkBorder = Border.XPositive;
                neighbourChunkCenter = centerOfXNegativeNeighbourChunk;
                neighborCubePosition = newCubeDataPosition + Vector3.left;
                break;
            case Border.XPositive:
                neighborChunkBorder = Border.XNegative;
                neighbourChunkCenter = centerOfXPositiveNeighbourChunk;
                neighborCubePosition = newCubeDataPosition + Vector3.right;
                break;
            case Border.ZNegative:
                neighborChunkBorder = Border.ZPositive;
                neighbourChunkCenter = centerOfZNegativeNeighbourChunk;
                neighborCubePosition = newCubeDataPosition + Vector3.back;
                break;
            case Border.ZPositive:
                neighborChunkBorder = Border.ZNegative;
                neighbourChunkCenter = centerOfZPositiveNeighbourChunk;
                neighborCubePosition = newCubeDataPosition + Vector3.forward;
                break;
        }
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
}
