using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeData
{
    public GameObject cubePrefab;
    public Vector3 position;
    public bool isCubeDataSurrounded;

    public CubeData(GameObject cubePrefab, Vector3 position, bool isCubeDataSurrounded)
    {
        this.cubePrefab = cubePrefab;
        this.position = position;
        this.isCubeDataSurrounded = isCubeDataSurrounded;
    }
}

//private struct ChunkData
//{
//    public Texture mainTexture;
//    public GameObject prefab;
//    //Dalsi veci co chces nastavit...
//}

//private void ChunkDataGenerationSequence(Vector3 upcomingCubePosition, GameObject actualCubecColor, ref uint debugActualCubeCounter, ref Dictionary<Vector3, GameObject> actualChunkFieldData)
//{
//    ChunkData actualCube = new ChunkData();
//    actualCube.prefab = actualCubeColor;
//    actualCube.mainTexture = GetTextureForPosition(upcomingCubePosition);
//    debugActualCubeCounter++;
//    actualChunkFieldData.Add(upcomingCubePosition, actualCube);
//    mapGenerator.mapFieldData.Add(upcomingCubePosition, actualCube);
//}
//private Texture GetTextureForPosition(Vector3 cubePosition)
//{
//    if (cubePosition.y > grassValue)
//    {
//        return mapGenerator.grass;
//    }
//    else if (cubePosition.y > dirtValue)
//    {
//        return actualMaterial.mainTexture = mapGenerator.dirt;
//    }
//    else if (cubePosition.y > rockValue)
//    {
//        return actualMaterial.mainTexture = mapGenerator.rock;
//    }
//    else
//    {
//        return actualMaterial.mainTexture = mapGenerator.sand;
//    }
//}

////Pozdeji pri generovani objektu pro chunk...

//GenerateChunk() {

//    foreach (KeyValuePair<Vector3, ChunkData> cube in actualChunkFieldData)
//    {
//        GameObject actualCube = Instantiate(cube.Value.prefab, cube.Key, Quaternion.identity);
//        actualCube.GetComponent<Renderer>().material.mainTexture = cube.Value.mainTexture;
//        //... dalsi nastaveni objektu
//    }
//}
