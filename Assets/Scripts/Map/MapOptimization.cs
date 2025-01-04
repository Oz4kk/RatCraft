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
        public Vector2 centerOfChunk;

        public NeighbourCubesData(T edgeType, Vector3 cubePosition, Vector2 centerOfChunk) : this()
        {
            this.edgeType = edgeType;
            this.cubePosition = cubePosition;
            this.centerOfChunk = centerOfChunk;
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
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Border, Vector2, Vector2, Vector2, Vector2> onIsBorderCube;
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Vector2, Border, Corner> onIsCornerCube;

        private float XNegativeCorner = 0;
        private float XPositiveCorner = 0;
        private float ZNegativeCorner = 0;
        private float ZPositiveCorner = 0;

        protected static MapGenerator mapGenerator;

        void Awake()
        {
            mapGenerator = GetComponent<MapGenerator>();

            mapGenerator.onDataOfNewChunkGenerated += PrecessAllCubeDataOfUpcommingChunk;
            mapGenerator.onCubeDestroyed += FindInvisibleCubeAroundBrokenCube;
            mapGenerator.onCubePlaced += DeactivateInvisibleCubesAroundPlacedCube;
        }

        private void PrecessAllCubeDataOfUpcommingChunk(Dictionary<Vector3, CubeData> actualChunkField, Vector2 centerOfNewChunk)
        {
            Vector2 centerOfXNegativeNeighbourChunk = new Vector2(centerOfNewChunk.x - mapGenerator.gridSize.x, centerOfNewChunk.y);
            Vector2 centerOfXPositiveNeighbourChunk = new Vector2(centerOfNewChunk.x + mapGenerator.gridSize.x, centerOfNewChunk.y);
            Vector2 centerOfZNegativeNeighbourChunk = new Vector2(centerOfNewChunk.x, centerOfNewChunk.y - mapGenerator.gridSize.z);
            Vector2 centerOfZPositiveNeighbourChunk = new Vector2(centerOfNewChunk.x, centerOfNewChunk.y + mapGenerator.gridSize.z);

            XNegativeCorner = centerOfNewChunk.x - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            XPositiveCorner = centerOfNewChunk.x + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;
            ZNegativeCorner = centerOfNewChunk.y - Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) + 1.0f;
            ZPositiveCorner = centerOfNewChunk.y + Mathf.Ceil((float)mapGenerator.gridSize.x / 2.0f) - 1.0f;

            foreach (KeyValuePair<Vector3, CubeData> actualCube in actualChunkField)
            {
                OptimizeDataOfNewChunk(actualCube.Value, centerOfNewChunk, actualChunkField, centerOfXNegativeNeighbourChunk, centerOfXPositiveNeighbourChunk, centerOfZNegativeNeighbourChunk, centerOfZPositiveNeighbourChunk);
            }
        }

        /// <summary>
        /// Optimization of cubes that are not on the edge of the chunk and.. 
        /// </summary>
        /// <param name="newCubeData"></param>
        /// <param name="centerOfNewChunk"></param>
        /// <param name="newChunkFieldData"></param>
        private void OptimizeDataOfNewChunk(CubeData newCubeData, Vector2 centerOfNewChunk, Dictionary<Vector3, CubeData> newChunkFieldData, Vector2 centerOfXNegativeNeighbourChunk, Vector2 centerOfXPositiveNeighbourChunk, Vector2 centerOfZNegativeNeighbourChunk, Vector2 centerOfZPositiveNeighbourChunk)
        {
            Border newChunkBorder = Border.Null;

            if (IsNewCubeAtBorder(newCubeData.position, ref newChunkBorder))
            {
                Corner newCubeCorner = Corner.Null;
                if (IsNewCubeAtCorner(newCubeData, ref newCubeCorner))
                {
                    onIsCornerCube(newChunkFieldData, newCubeData, centerOfNewChunk, newChunkBorder, newCubeCorner);
                }
                else
                {
                    onIsBorderCube(newChunkFieldData, newCubeData, newChunkBorder, centerOfXNegativeNeighbourChunk, centerOfXPositiveNeighbourChunk, centerOfZNegativeNeighbourChunk, centerOfZPositiveNeighbourChunk);
                }
            }
            else
            {
                DeactiavateSurroundedCubeData(newCubeData, newChunkFieldData);
            }
        }

        private bool IsNewCubeAtBorder(Vector3 newCubeDataPosition, ref Border newChunkBorder)
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


        private bool IsNewCubeAtCorner(CubeData newCubeData, ref Corner corner)
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
            if (!actualChunkField.ContainsKey(actualCube.position + Vector3.right))
            {
                return;
            }
            if (!actualChunkField.ContainsKey(actualCube.position - Vector3.right))
            {
                return;
            }
            if (!actualChunkField.ContainsKey(actualCube.position + Vector3.up))
            {
                return;
            }
            if (!actualChunkField.ContainsKey(actualCube.position - Vector3.up))
            {
                return;
            }
            if (!actualChunkField.ContainsKey(actualCube.position + Vector3.forward))
            {
                return;
            }
            if (!actualChunkField.ContainsKey(actualCube.position - Vector3.forward))
            {
                return;
            }

            actualCube.isCubeDataSurrounded = true;
        }

        private void DeactiavateSurroundedCube(GameObject actualCube, Dictionary<Vector3, CubeData> chunkField)
        {
            byte counter = 0;

            if (chunkField.ContainsKey(actualCube.transform.position + Vector3.right))
            {
                counter++;
            }
            if (chunkField.ContainsKey(actualCube.transform.position - Vector3.right))
            {
                counter++;
            }
            if (chunkField.ContainsKey(actualCube.transform.position + Vector3.up))
            {
                counter++;
            }
            if (chunkField.ContainsKey(actualCube.transform.position - Vector3.up))
            {
                counter++;
            }
            if (chunkField.ContainsKey(actualCube.transform.position + Vector3.forward))
            {
                counter++;
            }
            if (chunkField.ContainsKey(actualCube.transform.position - Vector3.forward))
            {
                counter++;
            }

            if (counter == 6)
            {
                actualCube.SetActive(false);
            }
        }

        private void DeactivateInvisibleCubesAroundPlacedCube(Vector3 cubePosition)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubePosition.x, cubePosition.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];
            
            if (chunkField.ContainsKey(cubePosition + Vector3.right))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition + Vector3.right].cubeParameters.gameObject, chunkField);
            }
            if (chunkField.ContainsKey(cubePosition - Vector3.right))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition - Vector3.right].cubeParameters.gameObject, chunkField);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.up))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition + Vector3.up].cubeParameters.gameObject, chunkField);
            }
            if (chunkField.ContainsKey(cubePosition - Vector3.up))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition - Vector3.up].cubeParameters.gameObject, chunkField);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.forward))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition + Vector3.forward].cubeParameters.gameObject, chunkField);
            }
            if (chunkField.ContainsKey(cubePosition - Vector3.forward))
            {
                DeactiavateSurroundedCube(chunkField[cubePosition - Vector3.forward].cubeParameters.gameObject, chunkField);
            }
        }

        private void FindInvisibleCubeAroundBrokenCube(Vector3 cubePosition)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubePosition.x, cubePosition.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];

            if (chunkField.ContainsKey(cubePosition + Vector3.right))
            {
                ExposeCube(chunkField[cubePosition + Vector3.right]);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.left))
            {
                ExposeCube(chunkField[cubePosition + Vector3.left]);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.up))
            {
                ExposeCube(chunkField[cubePosition + Vector3.up]);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.down))
            {
                ExposeCube(chunkField[cubePosition + Vector3.down]);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.forward))
            {
                ExposeCube(chunkField[cubePosition + Vector3.forward]);
            }
            if (chunkField.ContainsKey(cubePosition + Vector3.back))
            {
                ExposeCube(chunkField[cubePosition + Vector3.back]);
            }
        }
        
        private void ExposeCube(CubeData cubeData)
        {
            if (cubeData.isCubeDataSurrounded)
            {
                mapGenerator.InstantiateCube(cubeData);
            }
            else
            {
                if (cubeData.cubeParameters.gameObject.activeInHierarchy)
                {
                    return;
                }
                else
                {
                    cubeData.cubeParameters.gameObject.SetActive(true);
                }
            }
        }
    }
}
