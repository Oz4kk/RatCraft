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
            Border border = GetBorderOfCube(newCubeData.position);
            Corner corner = IsCubeAtCorner(newCubeData.position, centerOfNewChunk);
            if (border != Border.Null)
            {
                if (corner != Corner.Null)
                {
                    onIsCornerCube(newChunkFieldData, newCubeData, centerOfNewChunk, border, corner);
                    return;
                }
                onIsBorderCube(newChunkFieldData, newCubeData, border);
            } 
            else
            {
                DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
            }
        }

        private Border GetBorderOfCube(Vector3 cubeDataPosition)
        {
            Border border;
            // Negative X border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((cubeDataPosition.x - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                border = Border.XPositive;
                return border;
            }
            // Positive X border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((cubeDataPosition.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                border = Border.XNegative;
                return border;
            }
            // Negative Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((cubeDataPosition.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                border = Border.ZPositive;
                return border;
            }
            // Postive Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            if ((cubeDataPosition.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                border = Border.ZNegative;
                return border;
            }
            border = Border.Null;
            return border;
        }
        
        private Corner IsCubeAtCorner(Vector3 newCubeDataPosition, Vector2 chunkCenter)
        {
            float XNegativeCorner = chunkCenter.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            float XPositiveCorner = chunkCenter.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            float ZNegativeCorner = chunkCenter.y - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            float ZPositiveCorner = chunkCenter.y + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;

            Corner corner;
            // XNegativeZNegative corner
            if (newCubeDataPosition.x == XNegativeCorner && newCubeDataPosition.z == ZNegativeCorner)
            {
                corner = Corner.XNegativeZNegative;
                return corner;
            } 
            // XNegativeZPositive corner
            if (newCubeDataPosition.x == XNegativeCorner && newCubeDataPosition.z == ZPositiveCorner)
            {
                corner = Corner.XNegativeZPositive;
                return corner;
            } 
            // XPositiveZNegative corner
            if (newCubeDataPosition.x == XPositiveCorner && newCubeDataPosition.z == ZNegativeCorner)
            {
                corner = Corner.XPositiveZNegative;
                return corner;
            } 
            // XPositiveZPositive corner
            if (newCubeDataPosition.x == XPositiveCorner && newCubeDataPosition.z == ZPositiveCorner)
            {
                corner = Corner.XPositiveZPositive;
                return corner;
            }
            corner = Corner.Null;
            return corner;
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
            
            Border border = GetBorderOfCube(cubeData.position);
            Corner corner = IsCubeAtCorner(cubeData.position, chunkCenter);
            if (border != Border.Null)
            {
                if (corner != Corner.Null)
                {
                    onIsPlacedCornerCube(cubeData, corner);
                    return;
                }
                onIsPlacedBorderCube(cubeData, chunkField, border);
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
            
            Border border = GetBorderOfCube(cubeData.position);
            Corner corner = IsCubeAtCorner(cubeData.position, chunkCenter);
            if (border != Border.Null)
            {
                if (corner != Corner.Null)
                {
                    onIsDestroyedCornerCube(cubeData, chunkField, border, corner);
                    return;
                }
                onIsDestroyedBorderCube(cubeData, chunkField, border);
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
    }
}
