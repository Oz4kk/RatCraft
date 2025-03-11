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
        mapOptimization.onIsSelectedBorderCube += FindInvisibleCubeAroundBrokenCube;
    }

    private void FindInvisibleCubeAroundBrokenCube(CubeData cubeData, Border cubeBorder)
    {
        NeighbourCubesData<Border> neighbourCubesData = SetNeighborCubeValues(cubeBorder, cubeData);
    }

    private void BorderCubeOptimizationSequence(Dictionary<Vector3, CubeData> newChunkFieldData, CubeData newCubeData, Border newChunkBorder)
    {
        NeighbourCubesData<Border> neighbourCubesData = SetNeighborCubeValues(newChunkBorder, newCubeData);

        if (!DoesNeighborChunkExist(neighbourCubesData.centerOfChunk))
        {
            return;
        }

        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighbourCubesData.centerOfChunk];
        // Return if New Cube in New Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded(newChunkFieldData, newCubeData.position, neighbourChunkField, neighbourCubesData.cubePosition, newChunkBorder))
        {
            return;
        }
        newCubeData.isCubeDataSurrounded = true;

        // Return if Neighbor Cube in Neighbor Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded(neighbourChunkField, neighbourCubesData.cubePosition, newChunkFieldData, newCubeData.position, neighbourCubesData.edgeType))
        {
            return;
        }
        neighbourChunkField[neighbourCubesData.cubePosition].cubeParameters.gameObject.SetActive(false);
    }

    private NeighbourCubesData<Border> SetNeighborCubeValues(Border newChunkBorder, CubeData newCubeData)
    {
        NeighbourCubesData<Border> neighbourCubesData = new NeighbourCubesData<Border>();
        Vector2 newCubeChunkCenter = newCubeData.chunkCenter;
        
        switch (newChunkBorder)
        {
            case Border.XNegative:
                neighbourCubesData.edgeType = Border.XPositive;
                neighbourCubesData.centerOfChunk = new Vector2(newCubeChunkCenter.x - mapGenerator.gridSize.x, newCubeChunkCenter.y); // centerOfXNegativeNeighbourChunk
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.left;
                break;
            case Border.XPositive:
                neighbourCubesData.edgeType = Border.XNegative;
                neighbourCubesData.centerOfChunk = new Vector2(newCubeChunkCenter.x + mapGenerator.gridSize.x, newCubeChunkCenter.y); // centerOfXPositiveNeighbourChunk
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.right;
                break;
            case Border.ZNegative:
                neighbourCubesData.edgeType = Border.ZPositive;
                neighbourCubesData.centerOfChunk = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y - mapGenerator.gridSize.x); // centerOfZNegativeNeighbourChunk
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.back;
                break;
            case Border.ZPositive:
                neighbourCubesData.edgeType = Border.ZNegative;
                neighbourCubesData.centerOfChunk = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y + mapGenerator.gridSize.y); // centerOfZPositiveNeighbourChunk
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.forward;
                break;
        }

        return neighbourCubesData;
    }

    private bool DoesNeighborChunkExist(Vector2 neighborChunkCenter)
    {
        if (mapGenerator.dictionaryOfCentersWithItsChunkField.ContainsKey(neighborChunkCenter))
        {
            return true;
        }

        return false;
    }

    private bool IsBorderCubeSurrounded(Dictionary<Vector3, CubeData> firstChunkFieldData, Vector3 firstCubePosition, Dictionary<Vector3, CubeData> secondChunkFieldData, Vector3 secondCubePosition, Border border)
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
