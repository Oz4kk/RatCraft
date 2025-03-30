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
        mapOptimization.onIsPlacedBorderCube += BorderCubePlacementSequence;
        mapOptimization.onIsDestroyedBorderCube += FindInvisibleCubesAroundDestroyedCube;
    }

    private void BorderCubePlacementSequence(CubeData newCubeData, Dictionary<Vector3, CubeData> chunkField, Border border)
    {
        NeighbourCubesValues<Border> potentionalNeighbourCubeValues = GetNeighborCubeValues(border, newCubeData);
        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[potentionalNeighbourCubeValues.chunkCenter];
        
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundPlacedCube = GetNeighborCubeValuesAroundSelectedCube(newCubeData.position, newCubeData.chunkCenter, border, potentionalNeighbourCubeValues.chunkCenter);
        
        HideInvisibleCubeAroundPlacedCubeInItsChunk(chunkField, neighbourCubesValuesAroundPlacedCube, potentionalNeighbourCubeValues.chunkCenter, potentionalNeighbourCubeValues.position, neighbourChunkField);
        if (!neighbourChunkField.ContainsKey(potentionalNeighbourCubeValues.position))
        {
            return;
        }

        if (!IsBorderCubeSurrounded(chunkField, newCubeData.position, neighbourChunkField, potentionalNeighbourCubeValues.position, border))
        {
            return;
        }
        
        CubeData neighbourCubeData = neighbourChunkField[potentionalNeighbourCubeValues.position];
        neighbourCubeData.cubeParameters.gameObject.SetActive(false);
    }

    private void HideInvisibleCubeAroundPlacedCubeInItsChunk(Dictionary<Vector3, CubeData> chunkField, NeighbourCubesValues<Border>[] neighbourCubesValuesAroundSelectedCube, Vector2 neighborChunkCenter, Vector3 neighbourCubePosition, Dictionary<Vector3, CubeData> neighbourChunkField)
    {
        foreach (NeighbourCubesValues<Border> neighbourCubeValue in neighbourCubesValuesAroundSelectedCube)
        {
            if (neighbourCubeValue.chunkCenter == neighborChunkCenter)
            {
                continue;
            } 
            if (!chunkField.ContainsKey(neighbourCubeValue.position))
            {
                continue;
            }
            if (!IsBorderCubeSurrounded(chunkField, neighbourCubeValue.position, neighbourChunkField, neighbourCubePosition, neighbourCubeValue.edgeType))
            {
                continue;
            }

            chunkField[neighbourCubeValue.position].cubeParameters.gameObject.SetActive(false);
        }
    }

    private void FindInvisibleCubesAroundDestroyedCube(CubeData destroyedCubeData, Dictionary<Vector3, CubeData> destroyedCubeChunkField, Border destroyedCubeBorder)
    {
        NeighbourCubesValues<Border> potentionalNeighbourCubeValues = GetNeighborCubeValues(destroyedCubeBorder, destroyedCubeData);
        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[potentionalNeighbourCubeValues.chunkCenter];
        
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundDestroyedCube = GetNeighborCubeValuesAroundSelectedCube(destroyedCubeData.position, destroyedCubeData.chunkCenter, destroyedCubeBorder, potentionalNeighbourCubeValues.chunkCenter);

        ExposeVisibleCubesAroundSelectedCubeInItsChunk(destroyedCubeChunkField, neighbourCubesValuesAroundDestroyedCube, potentionalNeighbourCubeValues.chunkCenter);
        if (!neighbourChunkField.ContainsKey(potentionalNeighbourCubeValues.position))
        {
            return;
        }
        
        CubeData neighbourCubeData = neighbourChunkField[potentionalNeighbourCubeValues.position];
        mapOptimization.ExposeCube(neighbourCubeData);
    }

    private void ExposeVisibleCubesAroundSelectedCubeInItsChunk(Dictionary<Vector3, CubeData> chunkField, NeighbourCubesValues<Border>[] neighbourCubesValuesAroundSelectedCube, Vector2 neighborChunkCenter)
    {
        foreach (NeighbourCubesValues<Border> neighbourCubeValue in neighbourCubesValuesAroundSelectedCube)
        {
            if (neighbourCubeValue.chunkCenter == neighborChunkCenter)
            {
                continue;
            } 
            if (!chunkField.ContainsKey(neighbourCubeValue.position))
            {
                continue;
            }

            CubeData cubeData = chunkField[neighbourCubeValue.position];
            
            mapOptimization.ExposeCube(cubeData);
        }
    }

    internal NeighbourCubesValues<Border>[] GetNeighborCubeValuesAroundSelectedCube(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neighborChunkCenter)
    {
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundSelectedCube = new NeighbourCubesValues<Border>[6];
        
        for (int i = 0; i < mapOptimization.directions.Length; i++)
        {
            Vector3 direction = mapOptimization.directions[i];
            
            neighbourCubesValuesAroundSelectedCube[i] = SetNeighborCubeValue(cubePosition, chunkCenter, border, neighborChunkCenter, direction);
        }

        return neighbourCubesValuesAroundSelectedCube;
    }

    private NeighbourCubesValues<Border> SetNeighborCubeValue(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neigborChunkCenter, Vector3 direction)
    {
        NeighbourCubesValues<Border> neighbourCubeValue = new NeighbourCubesValues<Border>();

        neighbourCubeValue.position = cubePosition + direction;
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
        NeighbourCubesValues<Border> neighbourCubeValues = GetNeighborCubeValues(newChunkBorder, newCubeData);

        if (!DoesNeighborChunkExist(neighbourCubeValues.chunkCenter))
        {
            return;
        }

        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighbourCubeValues.chunkCenter];
        // Return if New Cube in New Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded(newChunkFieldData, newCubeData.position, neighbourChunkField, neighbourCubeValues.position, newChunkBorder))
        {
            return;
        }
        newCubeData.isCubeDataSurrounded = true;

        // Return if Neighbor Cube in Neighbor Chunk isn't surrounded with cubes from each sides
        if (!IsBorderCubeSurrounded(neighbourChunkField, neighbourCubeValues.position, newChunkFieldData, newCubeData.position, neighbourCubeValues.edgeType))
        {
            return;
        }
        neighbourChunkField[neighbourCubeValues.position].cubeParameters.gameObject.SetActive(false);
    }

    private NeighbourCubesValues<Border> GetNeighborCubeValues(Border newChunkBorder, CubeData newCubeData)
    {
        NeighbourCubesValues<Border> neighbourCubesValues = new NeighbourCubesValues<Border>();
        Vector2 newCubeChunkCenter = newCubeData.chunkCenter;
        
        switch (newChunkBorder)
        {
            case Border.XNegative:
                neighbourCubesValues.edgeType = Border.XPositive;
                neighbourCubesValues.chunkCenter = new Vector2(newCubeChunkCenter.x - mapGenerator.gridSize.x, newCubeChunkCenter.y); 
                neighbourCubesValues.position = newCubeData.position + Vector3.left;
                break;
            case Border.XPositive:
                neighbourCubesValues.edgeType = Border.XNegative;
                neighbourCubesValues.chunkCenter = new Vector2(newCubeChunkCenter.x + mapGenerator.gridSize.x, newCubeChunkCenter.y); 
                neighbourCubesValues.position = newCubeData.position + Vector3.right;
                break;
            case Border.ZNegative:
                neighbourCubesValues.edgeType = Border.ZPositive;
                neighbourCubesValues.chunkCenter = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y - mapGenerator.gridSize.x); 
                neighbourCubesValues.position = newCubeData.position + Vector3.back;
                break;
            case Border.ZPositive:
                neighbourCubesValues.edgeType = Border.ZNegative;
                neighbourCubesValues.chunkCenter = new Vector2(newCubeChunkCenter.x, newCubeChunkCenter.y + mapGenerator.gridSize.x); 
                neighbourCubesValues.position = newCubeData.position + Vector3.forward;
                break;
        }

        return neighbourCubesValues;
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
