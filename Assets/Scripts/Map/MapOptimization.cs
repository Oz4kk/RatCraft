using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Security.Cryptography;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Profiling;
using static UnityEngine.UI.GridLayoutGroup;

namespace InternalTypesForMapOptimization
{
    internal struct NeighbourCubesValues<T>
    {
        public T edgeType;
        public Vector3 position;
        public Vector2 chunkCenter;

        public NeighbourCubesValues(T edgeType, Vector3 position, Vector2 chunkCenter) : this()
        {
            this.edgeType = edgeType;
            this.position = position;
            this.chunkCenter = chunkCenter;
        }
    }
    
    internal enum Border : byte
    {
        Null,
        XPositive,
        XNegative,
        ZPositive,
        ZNegative
    }

    internal enum Corner : byte
    {
        Null,
        XPositiveZPositive,
        XPositiveZNegative,
        XNegativeZPositive,
        XNegativeZNegative
    }

    public class MapOptimization : MonoBehaviour
    {
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Border> onIsBorderCube;
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Vector2, Border, Corner> onIsCornerCube;
        
        internal System.Action<CubeData, Dictionary<Vector3, CubeData>, Border> onIsDestroyedBorderCube;
        internal System.Action<CubeData, Dictionary<Vector3, CubeData>, Border, Corner> onIsDestroyedCornerCube;
        
        internal System.Action<CubeData, Dictionary<Vector3, CubeData>, Border> onIsPlacedBorderCube;
        internal System.Action<CubeData, Corner> onIsPlacedCornerCube;
        
        internal readonly Vector3[] directions = new[]
        {
            // Vertical Y directions
            Vector3.up, Vector3.down,
            // Horizontal X directions
            Vector3.right, Vector3.left,
            // Horizontal Z directions
            Vector3.forward, Vector3.back
        };
        
        protected static MapGenerator mapGenerator;

        void Awake()
        {
            mapGenerator = GetComponent<MapGenerator>();

            mapGenerator.onDataOfNewChunkGenerated += PrecessAllCubeDataOfUpcommingChunk;
            mapGenerator.onCubeDestroyed += FindInvisibleCubesAroundBrokenCube;
            mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
        }

        private void PrecessAllCubeDataOfUpcommingChunk(Dictionary<Vector3, CubeData> actualChunkField, Vector2 centerOfNewChunk)
        {
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
        private void OptimizeDataOfNewChunk(CubeData newCubeData, Vector2 centerOfNewChunk, Dictionary<Vector3, CubeData> newChunkFieldData)
        {
            Border newChunkBorder = Border.Null;
            if (IsCubeAtBorder(newCubeData.position, ref newChunkBorder))
            {
                Corner newCubeCorner = Corner.Null;
                if (IsCubeAtCorner(newCubeData, centerOfNewChunk, ref newCubeCorner))
                {
                    onIsCornerCube(newChunkFieldData, newCubeData, centerOfNewChunk, newChunkBorder, newCubeCorner);
                }
                else
                {
                    onIsBorderCube(newChunkFieldData, newCubeData, newChunkBorder);
                }
            }
            else
            {
                DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
            }
        }

        private bool IsCubeAtBorder(Vector3 newCubeDataPosition, ref Border newChunkBorder)
        {
            // Negative X border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((newCubeDataPosition.x - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.XPositive;

                return true;
            }
            // Positive X border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((newCubeDataPosition.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.XNegative;

                return true;
            }
            // Negative Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((newCubeDataPosition.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.ZPositive;

                return true;
            }
            // Postive Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((newCubeDataPosition.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.ZNegative;

                return true;
            }

            return false;
        }
        
        private bool IsCubeAtCorner(CubeData newCubeData, Vector2 chunkCenter, ref Corner corner)
        {
            float XNegativeCorner = chunkCenter.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            float XPositiveCorner = chunkCenter.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            float ZNegativeCorner = chunkCenter.y - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            float ZPositiveCorner = chunkCenter.y + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            
            if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZNegativeCorner)
            {
                corner = Corner.XNegativeZNegative;
                return true;
            } 
            if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZPositiveCorner)
            {
                corner = Corner.XNegativeZPositive;
                return true;
            } 
            if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZNegativeCorner)
            {
                corner = Corner.XPositiveZNegative;
                return true;
            } 
            if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZPositiveCorner)
            {
                corner = Corner.XPositiveZPositive;
                return true;
            }

            return false;
        }

        private void DeactiavateSurroundedCubeData(CubeData actualCube, Dictionary<Vector3, CubeData> actualChunkField)
        {
            foreach (Vector3 direction in directions)
            {
                if (!actualChunkField.ContainsKey(actualCube.position + direction))
                {
                    return;
                }
            }

            actualCube.isCubeDataSurrounded = true;
        }

        private void DeactiavateSurroundedCube(GameObject actualCube, Dictionary<Vector3, CubeData> chunkField)
        {

            foreach (Vector3 direction in directions)
            {
                if (!chunkField.ContainsKey(actualCube.transform.position + direction))
                {
                    return;
                }
            }
            
            actualCube.SetActive(false);
        }

        private void DeactivateInvisibleCubesAroundPlacedCube(CubeData cubeData)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubeData.position.x, cubeData.position.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];
            
            Border cubeBorder = Border.Null;
            if (IsCubeAtBorder(cubeData.position, ref cubeBorder))
            {
                Corner cubeCorner = Corner.Null;
                if (IsCubeAtCorner(cubeData, chunkCenter, ref cubeCorner))
                {
                    onIsPlacedCornerCube(cubeData, cubeCorner);
                }
                else
                {
                    onIsPlacedBorderCube(cubeData, chunkField, cubeBorder);
                }
            }
            else
            {
                foreach (Vector3 direction in directions)
                {
                    if (chunkField.ContainsKey(cubeData.position + direction))
                    {
                        DeactiavateSurroundedCube(chunkField[cubeData.position + direction].cubeParameters.gameObject, chunkField);
                    }
                }
            }
        }

        private void FindInvisibleCubesAroundBrokenCube(CubeData cubeData)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubeData.position.x, cubeData.position.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];
            
            Border cubeBorder = Border.Null;
            if (IsCubeAtBorder(cubeData.position, ref cubeBorder))
            {
                Corner cubeCorner = Corner.Null;
                if (IsCubeAtCorner(cubeData, chunkCenter, ref cubeCorner))
                {
                    onIsDestroyedCornerCube(cubeData, chunkField, cubeBorder, cubeCorner);
                }
                else
                {
                    onIsDestroyedBorderCube(cubeData, chunkField, cubeBorder);
                }
            }
            else
            {
                foreach (Vector3 direction in directions)
                {
                    if (chunkField.ContainsKey(cubeData.position + direction))
                    {
                        ExposeCube(chunkField[cubeData.position + direction]);
                    }
                }
            }
        }
        
        public void ExposeCube(CubeData cubeData)
        {
            if (cubeData.isCubeDataSurrounded)
            {
                mapGenerator.InstantiatePredeterminedCubeSequence(cubeData);
            }
            else
            {
                if (cubeData.cubeParameters.gameObject.activeInHierarchy)
                {
                    return;
                }
                
                cubeData.cubeParameters.gameObject.SetActive(true);
            }
        }
    }
}
