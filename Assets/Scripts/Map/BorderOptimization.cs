using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InternalTypesForMapOptimization;
using Unity.Collections;
using UnityEngine.UI;

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

    private void Awake()
    {
        mapGenerator = GetComponent<MapGenerator>();
        mapOptimization = GetComponent<MapOptimization>(); 

        mapOptimization.onIsBorderCube += BorderCubeOptimizationSequence;
        mapOptimization.onIsSelectedBorderCube += FindInvisibleCubeAroundBrokenCube;
    }

    private void FindInvisibleCubeAroundBrokenCube(CubeData selectedCubeData, Dictionary<Vector3, CubeData> chunkField, Border cubeBorder)
    {
        NeighbourCubesData<Border> potentionalNeighborCubeValues = SetNeighborCubeValues(cubeBorder, selectedCubeData);
        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[potentionalNeighborCubeValues.chunkCenter];
        
        NeighbourCubesData<Border>[] neighbourCubesValuesAroundSelectedCube = GetNeighborCubeValuesAroundSelectedCube(selectedCubeData.position, selectedCubeData.chunkCenter, cubeBorder, potentionalNeighborCubeValues.chunkCenter);

        ExposeVisibleCubesAroundSelectedCube(chunkField, neighbourCubesValuesAroundSelectedCube, potentionalNeighborCubeValues.chunkCenter);
        if (!neighbourChunkField.ContainsKey(potentionalNeighborCubeValues.cubePosition))
        {
            return;
        }
        
        CubeData neighbourCubeData = neighbourChunkField[potentionalNeighborCubeValues.cubePosition];
        mapOptimization.ExposeCube(neighbourCubeData);
    }

    private void ExposeVisibleCubesAroundSelectedCube(Dictionary<Vector3, CubeData> chunkField, NeighbourCubesData<Border>[] neighbourCubesValuesAroundSelectedCube, Vector2 neighborChunkCenter)
    {
        foreach (NeighbourCubesData<Border> neighbourCubeValue in neighbourCubesValuesAroundSelectedCube)
        {
            if (neighbourCubeValue.chunkCenter == neighborChunkCenter)
            {
                continue;
            } 
            if (!chunkField.ContainsKey(neighbourCubeValue.cubePosition))
            {
                continue;
            }

            CubeData cubeData = chunkField[neighbourCubeValue.cubePosition];
            
            mapOptimization.ExposeCube(cubeData);
        }
    }

    private NeighbourCubesData<Border>[] GetNeighborCubeValuesAroundSelectedCube(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neighborChunkCenter)
    {
        NeighbourCubesData<Border>[] neighbourCubesValuesAroundSelectedCube = new NeighbourCubesData<Border>[6];
        
        for (int i = 0; i < mapOptimization.directions.Length; i++)
        {
            Vector3 direction = mapOptimization.directions[i];
            
            neighbourCubesValuesAroundSelectedCube[i] = SetNeighborCubeValue(cubePosition, chunkCenter, border, neighborChunkCenter, direction);
        }

        return neighbourCubesValuesAroundSelectedCube;
    }

    private NeighbourCubesData<Border> SetNeighborCubeValue(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neigborChunkCenter, Vector3 direction)
    {
        NeighbourCubesData<Border> neighbourCubeValue = new NeighbourCubesData<Border>();

        neighbourCubeValue.cubePosition = cubePosition + direction;
        switch (border)
        {
            case Border.XNegative:
                if (direction == Vector3.left)
                {
                    neighbourCubeValue.edgeType = Border.XPositive;
                    neighbourCubeValue.chunkCenter = neigborChunkCenter;
                }
                else if (direction == Vector3.right)
                {
                    neighbourCubeValue.edgeType = Border.Null;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                else
                {
                    neighbourCubeValue.edgeType = Border.XNegative;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                break;            
            case Border.XPositive:
                if (direction == Vector3.right)
                {
                    neighbourCubeValue.edgeType = Border.XNegative;
                    neighbourCubeValue.chunkCenter = neigborChunkCenter;
                }
                else if (direction == Vector3.left)
                {
                    neighbourCubeValue.edgeType = Border.Null;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                else
                {
                    neighbourCubeValue.edgeType = Border.XPositive;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                break;            
            case Border.ZNegative: 
                if (direction == Vector3.back)
                {
                    neighbourCubeValue.edgeType = Border.ZPositive;
                    neighbourCubeValue.chunkCenter = neigborChunkCenter;
                }
                else if (direction == Vector3.forward)
                {
                    neighbourCubeValue.edgeType = Border.Null;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                else
                {
                    neighbourCubeValue.edgeType = Border.ZNegative;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                break;            
            case Border.ZPositive: 
                if (direction == Vector3.forward)
                {
                    neighbourCubeValue.edgeType = Border.ZNegative;
                    neighbourCubeValue.chunkCenter = neigborChunkCenter;
                }
                else if (direction == Vector3.back)
                {
                    neighbourCubeValue.edgeType = Border.Null;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                else
                {
                    neighbourCubeValue.edgeType = Border.ZPositive;
                    neighbourCubeValue.chunkCenter = chunkCenter;
                }
                break;
        }

        return neighbourCubeValue;
    }

    private void BorderCubeOptimizationSequence(Dictionary<Vector3, CubeData> newChunkFieldData, CubeData newCubeData, Border newChunkBorder)
    {
        NeighbourCubesData<Border> neighbourCubesData = SetNeighborCubeValues(newChunkBorder, newCubeData);

        if (!DoesNeighborChunkExist(neighbourCubesData.chunkCenter))
        {
            return;
        }

        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighbourCubesData.chunkCenter];
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
                neighbourCubesData.chunkCenter = new Vector2(newCubeChunkCenter.x - mapGenerator.gridSize.x, newCubeChunkCenter.y); 
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.left;
                break;
            case Border.XPositive:
                neighbourCubesData.edgeType = Border.XNegative;
                neighbourCubesData.chunkCenter = new Vector2(newCubeChunkCenter.x + mapGenerator.gridSize.x, newCubeChunkCenter.y); 
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.right;
                break;
            case Border.ZNegative:
                neighbourCubesData.edgeType = Border.ZPositive;
                neighbourCubesData.chunkCenter = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y - mapGenerator.gridSize.x); 
                neighbourCubesData.cubePosition = newCubeData.position + Vector3.back;
                break;
            case Border.ZPositive:
                neighbourCubesData.edgeType = Border.ZNegative;
                neighbourCubesData.chunkCenter = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y + mapGenerator.gridSize.x); 
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
            foreach (Vector3 actualDirection in mapOptimization.directions)
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
