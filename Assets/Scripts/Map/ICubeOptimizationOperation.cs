using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using InternalTypesForMapOptimization;

internal interface ICubeOptimizationOperation
{ 
        void HandleCornerCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Vector2 chunkCenter, Border border, Corner corner);
        void HandleBorderCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData, Border border);
        void HandleNeighborCube(Dictionary<Vector3, CubeData> chunkField, CubeData cubeData);
}
