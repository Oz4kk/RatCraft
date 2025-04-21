using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InternalTypesForMapOptimization;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

public class BorderOptimization : MonoBehaviour
{
    private enum BorderSideAroundCorner : byte
    {
        Null,
        XPositiveZPositive_Xnegative,
        XPositiveZPositive_Znegative,
        XPositiveZNegative_Xnegative,
        XPositiveZNegative_Zpositive,
        XNegativeZPositive_Xpositive,
        XNegativeZPositive_Znegative,
        XNegativeZNegative_Xpositive,
        XNegativeZNegative_Zpositive,
    }
    
    private MapGenerator mapGenerator;
    private MapOptimization mapOptimization;
    private CornerOptimization cornerOptimization;

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
        cornerOptimization = GetComponent<CornerOptimization>();

        mapOptimization.onIsBorderCube += BorderCubeOptimizationSequence;
        mapOptimization.onIsPlacedBorderCube += BorderCubePlacementSequence;
        mapOptimization.onIsDestroyedBorderCube += FindInvisibleCubesAroundDestroyedCube;
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

    private void BorderCubePlacementSequence(CubeData newCubeData, Dictionary<Vector3, CubeData> chunkField, Border border)
    {
        NeighbourCubesValues<Border> potentionalNeighbourCubeValues = GetNeighborCubeValues(border, newCubeData);
        Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[potentionalNeighbourCubeValues.chunkCenter];

        BorderSideAroundCorner borderSideAroundCorner = BorderSideAroundCorner.Null;
        
        if (!IsBorderCubeNearCorner(newCubeData, newCubeData.chunkCenter, ref borderSideAroundCorner))
        {
            NeighbourCubesValues<Border>[] neighbourCubesValuesAroundPlacedCube = GetNeighborCubeValuesAroundSelectedCube2(newCubeData.position, newCubeData.chunkCenter, border, potentionalNeighbourCubeValues.chunkCenter);
            
            HideInvisibleCubeAroundPlacedCubeInItsChunk(chunkField, neighbourCubesValuesAroundPlacedCube, potentionalNeighbourCubeValues.chunkCenter, potentionalNeighbourCubeValues.position, neighbourChunkField);
            HideInvisibleCubeAroundPlacedCubeInNeighborChunk(neighbourChunkField, potentionalNeighbourCubeValues);
        }
        else
        {
            NeighbourCubesValues<Border>[] neighbourCubesValuesAroundPlacedCube = GetNeighborCubeValuesAroundSelectedCube3(newCubeData.position, newCubeData.chunkCenter, border, borderSideAroundCorner, potentionalNeighbourCubeValues.chunkCenter);
            NeighbourCubesValues<Corner> potentionalCornerCubeValues = GetCornerCubeValues(borderSideAroundCorner, newCubeData);
            
            HideInvisibleCubeAroundPlacedCubeInItsChunk(chunkField, neighbourCubesValuesAroundPlacedCube, potentionalNeighbourCubeValues.chunkCenter, potentionalNeighbourCubeValues.position, neighbourChunkField);
            HideInvisibleCubeAroundPlacedCubeInNeighborChunk(neighbourChunkField, potentionalNeighbourCubeValues);
            HideInvisibleCornerCubeAroundPlacedCubeInItsChunk(potentionalCornerCubeValues, chunkField);
        }
    }

    private void HideInvisibleCornerCubeAroundPlacedCubeInItsChunk(NeighbourCubesValues<Corner> potentionalCornerCubeValues, Dictionary<Vector3, CubeData> chunkField)
    {
        if (!chunkField.ContainsKey(potentionalCornerCubeValues.position))
        {
            return;
        }
        
        NeighbourCubesValues<Corner>[] cornerCubesValuesAroundCorner = cornerOptimization.GetCornerCubesValuesAroundSelectedCornerCube(potentionalCornerCubeValues.position, potentionalCornerCubeValues.chunkCenter, potentionalCornerCubeValues.edgeType);
        NeighbourCubesValues<Border>[] borderCubesValuesAroundCorner = cornerOptimization.GetBorderCubesValuesAroundSelectedCornerCube(potentionalCornerCubeValues.position, potentionalCornerCubeValues.chunkCenter, potentionalCornerCubeValues.edgeType);

        foreach (NeighbourCubesValues<Corner> neighborCornerCube in cornerCubesValuesAroundCorner)
        {
            Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborCornerCube.chunkCenter];
            
            if (!neighbourChunkField.ContainsKey(neighborCornerCube.position))
            {
                return;
            }
        }

        foreach (NeighbourCubesValues<Border> neighborBorderCube in borderCubesValuesAroundCorner)
        {
            Dictionary<Vector3, CubeData> neighbourChunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[neighborBorderCube.chunkCenter];
            
            if (!neighbourChunkField.ContainsKey(neighborBorderCube.position))
            {
                return;
            } 
        }
        
        CubeData cornerCubeData = chunkField[potentionalCornerCubeValues.position];
        cornerCubeData.cubeParameters.gameObject.SetActive(false);
    }

    private bool IsBorderCubeNearCorner(CubeData newCubeData, Vector2 chunkCenter, ref BorderSideAroundCorner borderSideAroundCorner)
    {
        float XNegativeCorner = chunkCenter.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
        float XPositiveCorner = chunkCenter.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
        float ZNegativeCorner = chunkCenter.y - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
        float ZPositiveCorner = chunkCenter.y + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            
        if (newCubeData.position.x == (XNegativeCorner + 1.0f) && newCubeData.position.z == ZNegativeCorner)
        {
            borderSideAroundCorner = BorderSideAroundCorner.XNegativeZNegative_Xpositive;
            return true;
        }
        if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == (ZNegativeCorner + 1.0f))
        {
            borderSideAroundCorner = BorderSideAroundCorner.XNegativeZNegative_Zpositive;
            return true;
        }
        if (newCubeData.position.x == (XNegativeCorner + 1.0f) && newCubeData.position.z == ZPositiveCorner)
        {
            borderSideAroundCorner = BorderSideAroundCorner.XNegativeZPositive_Xpositive;
            return true;
        }
        if (newCubeData.position.x == XNegativeCorner  && newCubeData.position.z == (ZPositiveCorner - 1.0f))
        {
            borderSideAroundCorner = BorderSideAroundCorner.XNegativeZPositive_Znegative;
            return true;
        } 
        if (newCubeData.position.x == (XPositiveCorner - 1.0f) && newCubeData.position.z == ZNegativeCorner)
        {
            borderSideAroundCorner = BorderSideAroundCorner.XPositiveZNegative_Xnegative;
            return true;
        } 
        if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == (ZNegativeCorner + 1.0f))
        {
            borderSideAroundCorner = BorderSideAroundCorner.XPositiveZNegative_Zpositive;
            return true;
        } 
        if (newCubeData.position.x == (XPositiveCorner - 1.0f) && newCubeData.position.z == ZPositiveCorner)
        {
            borderSideAroundCorner = BorderSideAroundCorner.XPositiveZPositive_Xnegative;
            return true;
        }
        if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == (ZPositiveCorner - 1.0f))
        {
            borderSideAroundCorner = BorderSideAroundCorner.XPositiveZPositive_Znegative;
            return true;
        }

        return false;
    }
    
    private void HideInvisibleCubeAroundPlacedCubeInNeighborChunk(Dictionary<Vector3, CubeData> neighbourChunkField, NeighbourCubesValues<Border> potentionalNeighbourCubeValues)
    {
        if (!neighbourChunkField.ContainsKey(potentionalNeighbourCubeValues.position))
        {
            return;
        }
        if (!IsBorderCubeSurrounded2(neighbourChunkField, potentionalNeighbourCubeValues.position, potentionalNeighbourCubeValues.edgeType))
        {
            return;
        }
        
        CubeData neighbourCubeData = neighbourChunkField[potentionalNeighbourCubeValues.position];
        neighbourCubeData.cubeParameters.gameObject.SetActive(false);
    }
    
    private bool IsBorderCubeSurrounded2(Dictionary<Vector3, CubeData> chunkFieldData, Vector3 cubePosition, Border border)
    {
        foreach (Vector3 actualDirection in mapOptimization.directions)
        {
            // Continue to the next foreach iteration if Actual Direction is the same as Current Border
            if (IsDirectionMatchingBorder(actualDirection, border))
            {
                continue;
            }
            Vector3 predictedCubePosition = cubePosition + actualDirection;
            if (!chunkFieldData.ContainsKey(predictedCubePosition))
            {
                return false;
            }
        }

        return true;
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
        
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundDestroyedCube = GetNeighborCubeValuesAroundSelectedCube2(destroyedCubeData.position, destroyedCubeData.chunkCenter, destroyedCubeBorder, potentionalNeighbourCubeValues.chunkCenter);

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
    
    private NeighbourCubesValues<Border>[] GetNeighborCubeValuesAroundSelectedCube3(Vector3 cubePosition, Vector3 chunkCenter, Border border, BorderSideAroundCorner borderSideAroundCorner, Vector3 neighborChunkCenter)
    {
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundSelectedCube = new NeighbourCubesValues<Border>[4];
        Vector3 direction;

        int arrayLength = mapOptimization.directions.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            direction = mapOptimization.directions[i];
            
            if (IsDirectionMatchingBorder(direction, border))
            {
                i--;
                arrayLength--;
                continue;
            }

            if (IsDirectionMatchingBorderSideAroundCorner(direction, borderSideAroundCorner))
            {
                i--;
                arrayLength--;
                continue;
            }
            neighbourCubesValuesAroundSelectedCube[i] = SetNeighborCubeValue(cubePosition, chunkCenter, border, neighborChunkCenter, direction);
        }

        return neighbourCubesValuesAroundSelectedCube;
    }
    
    private NeighbourCubesValues<Border>[] GetNeighborCubeValuesAroundSelectedCube2(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neighborChunkCenter)
    {
        NeighbourCubesValues<Border>[] neighbourCubesValuesAroundSelectedCube = new NeighbourCubesValues<Border>[5];
        Vector3 direction;

        int arrayLength = mapOptimization.directions.Length;
        for (int i = 0; i < arrayLength; i++)
        {
            direction = mapOptimization.directions[i];
            
            if (IsDirectionMatchingBorder(direction, border))
            {
                i--;
                arrayLength--;
                continue;
            }
            neighbourCubesValuesAroundSelectedCube[i] = SetNeighborCubeValue(cubePosition, chunkCenter, border, neighborChunkCenter, direction);
        }

        return neighbourCubesValuesAroundSelectedCube;
    }

    private NeighbourCubesValues<Border> SetNeighborCubeValue(Vector3 cubePosition, Vector3 chunkCenter, Border border, Vector3 neigbourChunkCenter, Vector3 direction)
    {
        NeighbourCubesValues<Border> neighbourCubeValue = new NeighbourCubesValues<Border>();

        neighbourCubeValue.position = cubePosition + direction;
        switch (border)
        {
            case Border.XNegative:
                if (direction == Vector3.left)
                {
                    neighbourCubeValue.edgeType = Border.XPositive;
                    neighbourCubeValue.chunkCenter = neigbourChunkCenter;
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
                    neighbourCubeValue.chunkCenter = neigbourChunkCenter;
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
                    neighbourCubeValue.chunkCenter = neigbourChunkCenter;
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
                    neighbourCubeValue.chunkCenter = neigbourChunkCenter;
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

    private NeighbourCubesValues<Corner> GetCornerCubeValues(BorderSideAroundCorner borderSideAroundCorner, CubeData newCubeData)
    {
        NeighbourCubesValues<Corner> neighbourCubesValues = new NeighbourCubesValues<Corner>();
        neighbourCubesValues.chunkCenter = newCubeData.chunkCenter;
        
        switch (borderSideAroundCorner)
        {
            case BorderSideAroundCorner.XNegativeZNegative_Xpositive:
                neighbourCubesValues.edgeType = Corner.XNegativeZNegative;
                neighbourCubesValues.position = newCubeData.position + -(Vector3.right);
                break;
            case BorderSideAroundCorner.XNegativeZNegative_Zpositive:
                neighbourCubesValues.edgeType = Corner.XNegativeZNegative;
                neighbourCubesValues.position = newCubeData.position + -(Vector3.forward);
                break;
            
            case BorderSideAroundCorner.XNegativeZPositive_Xpositive:
                neighbourCubesValues.edgeType = Corner.XNegativeZPositive;
                neighbourCubesValues.position = newCubeData.position + -(Vector3.right);
                break;
            case BorderSideAroundCorner.XNegativeZPositive_Znegative:
                neighbourCubesValues.edgeType = Corner.XNegativeZPositive;
                neighbourCubesValues.position = newCubeData.position + Vector3.forward;
                break;
            
            case BorderSideAroundCorner.XPositiveZPositive_Xnegative:
                neighbourCubesValues.edgeType = Corner.XPositiveZPositive;
                neighbourCubesValues.position = newCubeData.position + Vector3.right;
                break;
            case BorderSideAroundCorner.XPositiveZPositive_Znegative:
                neighbourCubesValues.edgeType = Corner.XPositiveZPositive;
                neighbourCubesValues.position = newCubeData.position + Vector3.forward;
                break;
            
            case BorderSideAroundCorner.XPositiveZNegative_Xnegative:
                neighbourCubesValues.edgeType = Corner.XPositiveZNegative;
                neighbourCubesValues.position = newCubeData.position + Vector3.right;
                break;
            case BorderSideAroundCorner.XPositiveZNegative_Zpositive:
                neighbourCubesValues.edgeType = Corner.XPositiveZNegative;
                neighbourCubesValues.position = newCubeData.position + -(Vector3.forward);
                break;
        }

        return neighbourCubesValues;
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
    
    private bool IsDirectionMatchingBorderSideAroundCorner(Vector3 actualDirection, BorderSideAroundCorner borderSideAroundCorner)
    {
        // XnegativeZnegative corner
        if (actualDirection == -(Vector3.right) && borderSideAroundCorner == BorderSideAroundCorner.XNegativeZNegative_Xpositive)
        {
            return true;
        }
        if (actualDirection == -(Vector3.forward) && borderSideAroundCorner == BorderSideAroundCorner.XNegativeZNegative_Zpositive)
        {
            return true;
        }
        // XnegativeZpositive corner
        if (actualDirection == -(Vector3.right) && borderSideAroundCorner == BorderSideAroundCorner.XNegativeZPositive_Xpositive)
        {
            return true;
        }
        if (actualDirection == Vector3.forward && borderSideAroundCorner == BorderSideAroundCorner.XNegativeZPositive_Znegative)
        {
            return true;
        }
        // XpositiveZpositive corner
        if (actualDirection == Vector3.right && borderSideAroundCorner == BorderSideAroundCorner.XPositiveZPositive_Xnegative)
        {
            return true;
        }
        if (actualDirection == Vector3.forward && borderSideAroundCorner == BorderSideAroundCorner.XPositiveZPositive_Znegative)
        {
            return true;
        }
        // XPositiveZnegative corner
        if (actualDirection == Vector3.right && borderSideAroundCorner == BorderSideAroundCorner.XPositiveZNegative_Xnegative)
        {
            return true;
        }
        if (actualDirection == -(Vector3.forward) && borderSideAroundCorner == BorderSideAroundCorner.XPositiveZNegative_Zpositive)
        {
            return true;
        }

        return false;
    }
}
