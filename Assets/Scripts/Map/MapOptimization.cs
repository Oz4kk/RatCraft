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
    internal struct NeighbourCubesData<T>
    {
        public T edgeType;
        public Vector3 cubePosition;
        public Vector2 chunkCenter;

        public NeighbourCubesData(T edgeType, Vector3 cubePosition, Vector2 chunkCenter) : this()
        {
            this.edgeType = edgeType;
            this.cubePosition = cubePosition;
            this.chunkCenter = chunkCenter;
        }
    }

    internal enum Border
    {
        Null,
        XPositive,
        XNegative,
        ZPositive,
        ZNegative
    }

    internal enum Corner
    {
        Null,
        XPositive_ZPositive,
        XPositive_ZNegative,
        XNegative_ZPositive,
        XNegative_ZNegative
    }

    public class MapOptimization : MonoBehaviour
    {
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Border> onIsBorderCube;
        internal System.Action<CubeData, Dictionary<Vector3, CubeData>, Border> onIsSelectedBorderCube;
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Vector2, Border, Corner> onIsCornerCube;
        
        internal readonly Vector3[] directions = new[]
        {
            // Vertical Y directions
            Vector3.up, Vector3.down,
            // Horizontal X directions
            Vector3.right, Vector3.left,
            // Horizontal Z directions
            Vector3.forward, Vector3.back
        };
        
        private float XNegativeCorner = 0;
        private float XPositiveCorner = 0;
        private float ZNegativeCorner = 0;
        private float ZPositiveCorner = 0;

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
            XNegativeCorner = centerOfNewChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            XPositiveCorner = centerOfNewChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            ZNegativeCorner = centerOfNewChunk.y - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            ZPositiveCorner = centerOfNewChunk.y + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;

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
                if (IsCubeAtCorner(newCubeData, ref newCubeCorner))
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
            else if ((newCubeDataPosition.x + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.XNegative;

                return true;
            }
            // Negative Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            else if ((newCubeDataPosition.z - (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.ZPositive;

                return true;
            }
            // Postive Z border of Actual Chunk
            // If actual cube position is on border of actual chunk and if border chunk exist, optimalize borders of these two chunks
            else if ((newCubeDataPosition.z + (mapGenerator.gridSize.x / 2)) % mapGenerator.gridSize.x == 0)
            {
                newChunkBorder = Border.ZNegative;

                return true;
            }

            return false;
        }


        private bool IsCubeAtCorner(CubeData newCubeData, ref Corner corner)
        {
            if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZNegativeCorner)
            {
                corner = Corner.XNegative_ZNegative;
                return true;
            }
            else if (newCubeData.position.x == XNegativeCorner && newCubeData.position.z == ZPositiveCorner)
            {
                corner = Corner.XNegative_ZPositive;
                return true;
            }
            else if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZNegativeCorner)
            {
                corner = Corner.XPositive_ZNegative;
                return true;
            }
            else if (newCubeData.position.x == XPositiveCorner && newCubeData.position.z == ZPositiveCorner)
            {
                corner = Corner.XPositive_ZPositive;
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

        private void DeactivateInvisibleCubesAroundPlacedCube(Vector3 cubePosition)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubePosition.x, cubePosition.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];

            foreach (Vector3 direction in directions)
            {
                if (chunkField.ContainsKey(cubePosition + direction))
                {
                    DeactiavateSurroundedCube(chunkField[cubePosition + direction].cubeParameters.gameObject, chunkField);
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
                if (IsCubeAtCorner(cubeData, ref cubeCorner))
                {
                    
                }
                else
                {
                    onIsSelectedBorderCube(cubeData, chunkField, cubeBorder);
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
