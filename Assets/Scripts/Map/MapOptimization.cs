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
        
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Border> onIsDestroyedBorderCube;
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData,  Border, Corner> onIsDestroyedCornerCube;
        
        internal System.Action<Dictionary<Vector3, CubeData>, CubeData, Border> onIsPlacedBorderCube;
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
        
        private ICubeOptimizationOperation optimizationOperation;
        private NewChunkOptimization newChunkOptimization;
        private DeactivateInvisibleCubesOptimization deactivateInvisibleCubesOptimization;
        private FindInvisibleCubesAroundBrokenCubeOptimization findInvisibleCubesAroundBrokenCubeOptimization;

        private Action<CubeData> onExposeCube;
        private Action<CubeData> onDeactiavateSurroundedCube;
        
        private class NewChunkOptimization : ICubeOptimizationOperation
        {
            private MapOptimization mapOptimization;

            public NewChunkOptimization(MapOptimization mapOptimization)
            {
                this.mapOptimization = mapOptimization;
            }

            public void HandleCornerCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Vector2 chunkCenter, Border border, Corner corner)
            {
                mapOptimization.onIsCornerCube(chunkField, cubeData, chunkCenter, border, corner);
            }

            public void HandleBorderCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Border border)
            {
                mapOptimization.onIsBorderCube(chunkField, cubeData, border);
            }

            public void HandleNeighborCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData)
            {
                mapOptimization.DeactiavateSurroundedCubeData(chunkField, cubeData);
            }
        }

        private class DeactivateInvisibleCubesOptimization : ICubeOptimizationOperation
        {
            private MapOptimization mapOptimization;
        
            public DeactivateInvisibleCubesOptimization(MapOptimization mapOptimization)
            {
                this.mapOptimization = mapOptimization;
            }
        
            public void HandleCornerCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Vector2 chunkCenter, Border border, Corner corner)
            {
                mapOptimization.onIsPlacedCornerCube(cubeData, corner);
            }
        
            public void HandleBorderCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Border border)
            {
                mapOptimization.onIsPlacedBorderCube(chunkField, cubeData, border);
            }
        
            public void HandleNeighborCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData)
            {
                mapOptimization.OptimizeNeighbors(chunkField, cubeData, mapOptimization.onDeactiavateSurroundedCube);
            }
        }
        
        private class FindInvisibleCubesAroundBrokenCubeOptimization : ICubeOptimizationOperation
        {
            private MapOptimization mapOptimization;
        
            public FindInvisibleCubesAroundBrokenCubeOptimization(MapOptimization mapOptimization)
            {
                this.mapOptimization = mapOptimization;
            }
        
            public void HandleCornerCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Vector2 chunkCenter, Border border, Corner corner)
            {
                mapOptimization.onIsDestroyedCornerCube(chunkField, cubeData, border, corner);
            }
        
            public void HandleBorderCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Border border)
            {
                mapOptimization.onIsDestroyedBorderCube(chunkField, cubeData, border);
            }
        
            public void HandleNeighborCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData)
            {
                mapOptimization.OptimizeNeighbors(chunkField, cubeData, mapOptimization.onExposeCube);
            }
        }

        private void ProcessCubeOptimization(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Vector2 centerOfNewChunk, ICubeOptimizationOperation optimizationOperation)
        {
            Border border = GetBorderOfCube(cubeData.position);
            Corner corner = IsCubeAtCorner(cubeData.position, centerOfNewChunk);
            if (border != Border.Null)
            {
                if (corner != Corner.Null)
                {
                    optimizationOperation.HandleCornerCube(chunkField, cubeData, centerOfNewChunk, border, corner);
                    return;
                }
                optimizationOperation.HandleBorderCube(chunkField, cubeData, border);
            } 
            else
            {
                optimizationOperation.HandleNeighborCube(chunkField, cubeData);
            }
        }
        
        void Awake()
        {
            mapGenerator = GetComponent<MapGenerator>();
            newChunkOptimization = new NewChunkOptimization(this);
            deactivateInvisibleCubesOptimization = new DeactivateInvisibleCubesOptimization(this);
            findInvisibleCubesAroundBrokenCubeOptimization = new FindInvisibleCubesAroundBrokenCubeOptimization(this);
                
            onExposeCube += ExposeCube;
            onDeactiavateSurroundedCube += DeactiavateSurroundedCube;
                
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
                ProcessCubeOptimization(actualChunkField, actualCube.Value, centerOfNewChunk, newChunkOptimization);
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

        private void DeactiavateSurroundedCubeData(Dictionary<Vector3, CubeData> actualChunkField, CubeData actualCube)
        {
            List<CubeData> neigbors = GetNeighborCubesInEachDirection(actualCube, actualChunkField);

            if (neigbors.Count != directions.Length)
            {
                return;
            }
            actualCube.isCubeDataSurrounded = true;
        }

        private void DeactiavateSurroundedCube(CubeData actualCube)
        {
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[actualCube.chunkCenter];
            List<CubeData> neighbors = GetNeighborCubesInEachDirection(actualCube, chunkField);
            
            if (neighbors.Count != directions.Length)
            {
                return;
            }
            actualCube.cubeParameters.gameObject.SetActive(false);
        }

        private void DeactivateInvisibleCubesAroundPlacedCube(CubeData cubeData)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubeData.position.x, cubeData.position.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];

            ProcessCubeOptimization(chunkField, cubeData, chunkCenter, deactivateInvisibleCubesOptimization);
        }

        private void FindInvisibleCubesAroundBrokenCube(CubeData cubeData)
        {
            Vector2 chunkCenter = mapGenerator.GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(cubeData.position.x, cubeData.position.z));
            Dictionary<Vector3, CubeData> chunkField = mapGenerator.dictionaryOfCentersWithItsChunkField[chunkCenter];
            
            ProcessCubeOptimization(chunkField, cubeData, chunkCenter, findInvisibleCubesAroundBrokenCubeOptimization);
        }

        private void OptimizeNeighbors(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Action<CubeData> optimizationOperation)
        {
            List<CubeData> neighbors = GetNeighborCubesInEachDirection(cubeData, chunkField);

            foreach (CubeData actualCubeData in neighbors)
            {
                optimizationOperation(actualCubeData);
            }
        }

        private List<CubeData> GetNeighborCubesInEachDirection(CubeData cubeData, Dictionary<Vector3, CubeData> chunkField)
        {
            List<CubeData> neighbors = new List<CubeData>();
            
            foreach (Vector3 direction in directions)
            {
                Vector3 actualPosition = cubeData.position + direction;
                if (chunkField.ContainsKey(cubeData.position + direction))
                {
                    neighbors.Add(chunkField[actualPosition]);
                }
            }
            
            return neighbors;
        }
    }
}
