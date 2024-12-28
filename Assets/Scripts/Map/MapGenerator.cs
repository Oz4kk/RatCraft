using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MapGenerator : MonoBehaviour
{
    public Action<Dictionary<Vector3, CubeData>, Vector2> onDataOfNewChunkGenerated;

    [Serializable]
    public struct GridSize
    {
        public int x;
        public int y;
        public int z;

        public GridSize(int x, int y, int z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public Action<GameObject> onCubeDestroyed;
    public Action<GameObject> onCubePlaced;

    public GameObject greenCube;
    public GameObject blueCube;
    public GameObject brownCube;
    public GameObject pinkCube;

    public Texture grass;
    public Texture dirt;
    public Texture rock;
    public Texture sand;

    private PlayerSpawn playerSpawn;
    private GameObject player;
    private ChunkGenerator chunkGenerator;

    [HideInInspector] public GridSize gridSize = new GridSize(0, 0, 0);

    // TO-DO: Delete this after finishing optimalisation
    public Dictionary<Vector3, GameObject> mapField = new Dictionary<Vector3, GameObject>();

    public float seed;

    [SerializeField] private float chunkGenerationDistanceFromEndOfTheChunk;
    [SerializeField] private uint gridSizeSides;
    [SerializeField] private uint gridSizeHeight;
    
    // Zmenit druhe dictionary tykajici se samotneho chunku na klasu
    public Dictionary<Vector2, Dictionary<Vector3, CubeData>> dictionaryOfCentersWithItsChunkField = new Dictionary<Vector2, Dictionary<Vector3, CubeData>>();

    private Vector2 middlePointOfLastChunk;
    
    private float xPositivePrediction;
    private float xNegativePrediction;
    private float zPositivePrediction;
    private float zNegativePrediction;

    private float grassValue = 10.0f;
    private float dirtValue = 7.0f;
    private float rockValue = 2.0f;
    
    private float greatestDistanceFromCenterOfTheChunk;

    private void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
        chunkGenerator = GetComponent<ChunkGenerator>();

        gridSize.y = (int)gridSizeHeight;
        gridSize.x = (int)gridSizeSides;
        gridSize.z = (int)gridSizeSides;
    }

    private void Start()
    {
        greatestDistanceFromCenterOfTheChunk = (gridSizeSides - 1) / Mathf.Sqrt(2);
        
        player = playerSpawn.spawnedPlayer;
        SetNewPredictionValues(new Vector2(player.transform.position.x, player.transform.position.z));
        ChunkGenerationSequence(middlePointOfLastChunk);
    }

    private void Update()
    {
        ProcessChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }
    
    public void DeleteCube(CubeParameters actualCube)
    {
        Vector2 centerOfActualChunk = GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(new Vector2(actualCube.position.x, actualCube.position.z));
        dictionaryOfCentersWithItsChunkField[centerOfActualChunk].Remove(actualCube.position);
        
        Debug.Log("1st - " + centerOfActualChunk + "" + actualCube.position);
        
        //mapField.Remove(actualCube.gameObject.transform.position);
        Destroy(actualCube.gameObject);

        onCubeDestroyed.Invoke(actualCube.gameObject);
    }
    
    public GameObject? InstantiateAndReturnCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject actualCube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            mapField.Add(spawnPosition, actualCube);

            return actualCube;
        }

        DebugManager.Log($"Count of mapField: {mapField.Count}");
        return null;
    }

    
    public Vector2 GetNearestDistanceBetweenPlacedCubePositionAndChunkCenters(Vector2 cubePositionWithoutHeight)
    {
        Vector2 nearestChunkCenter = Vector2.zero;
        
        foreach (KeyValuePair<Vector2, Dictionary<Vector3, CubeData>> chunkField in dictionaryOfCentersWithItsChunkField)
        {
            float distance = Vector2.Distance(chunkField.Key, cubePositionWithoutHeight);
            Debug.Log("chunkField = " + chunkField.Key + ", distance = " + distance);
            
            if (distance <= greatestDistanceFromCenterOfTheChunk)
            {
                nearestChunkCenter = chunkField.Key;
            }
        }
        
        return nearestChunkCenter;
    }
    
    private void ChooseTexture(GameObject actualCube)
    {
        Material actualMaterial = actualCube.GetComponent<Renderer>().material;
        if (actualCube.transform.position.y > grassValue)
        {
            actualMaterial.mainTexture = grass;
        }
        else if (actualCube.transform.position.y > dirtValue)
        {
            actualMaterial.mainTexture = dirt;
        }
        else if (actualCube.transform.position.y > rockValue)
        {
            actualMaterial.mainTexture = rock;
        }
        else
        {
            actualMaterial.mainTexture = sand;
        }
    }

    private void GeneratePreloadedChunk(Dictionary<Vector3, CubeData> newChunkFiedlData, Vector2 centerOfUpcommingChunk)
    {
        foreach (KeyValuePair<Vector3, CubeData> newCubeData in newChunkFiedlData)
        {
            if (newCubeData.Value.isCubeDataSurrounded)
            {
                continue;
            }
            GameObject cubeInstance = InstantiateAndReturnCube(newCubeData.Key, newCubeData.Value.cubePrefab);
            CubeParameters cubeParameters = cubeInstance.GetComponent<CubeParameters>();
            ChooseTexture(cubeInstance);
            
            newCubeData.Value.cubeParameters = cubeParameters;
        }
        dictionaryOfCentersWithItsChunkField.Add(centerOfUpcommingChunk, newChunkFiedlData);
    }

    private void SetNewPredictionValues(Vector2 middlePointOfActualChunk)
    {
        middlePointOfLastChunk = middlePointOfActualChunk;

        xPositivePrediction = middlePointOfActualChunk.x + gridSize.x / 2;
        xNegativePrediction = middlePointOfActualChunk.x - gridSize.x / 2;
        zPositivePrediction = middlePointOfActualChunk.y + gridSize.x / 2;
        zNegativePrediction = middlePointOfActualChunk.y - gridSize.x / 2;
    }

    private void ProcessChunkGenerationDistance()
    {
        Vector2 centerPointOfUpcomingChunk = new Vector2(middlePointOfLastChunk.x, middlePointOfLastChunk.y);
        if (player.transform.position.x > xPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerPointOfUpcomingChunk.x += gridSize.x;
        }
        else if (player.transform.position.x < xNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerPointOfUpcomingChunk.x -= gridSize.x;
        }
        else if (player.transform.position.z > zPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerPointOfUpcomingChunk.y += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerPointOfUpcomingChunk.y -= gridSize.x;
        }
        else
        {
            return;
        }
        IfCenterDontExistGenerateChunk(centerPointOfUpcomingChunk);
    }

    private void IfCenterDontExistGenerateChunk(Vector2 centerOfUpcomingChunk)
    {
        if (!dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcomingChunk))
        {
            ChunkGenerationSequence(centerOfUpcomingChunk);
        }
    }


    private void SetNewActiveChunkPrediction()
    {
        //Debug.Log($"Middle point of last visited chunk: {middlePointOfLastChunk.ToString()}");

        Vector2 centerOfUpcomingChunk = new Vector2(middlePointOfLastChunk.x, middlePointOfLastChunk.y);

        if (player.transform.position.x > xPositivePrediction)
        {
            centerOfUpcomingChunk.x += gridSize.x;
        }
        else if (player.transform.position.x < xNegativePrediction)
        {
            centerOfUpcomingChunk.x -= gridSize.x;
        }
        else if (player.transform.position.z > zPositivePrediction)
        {
            centerOfUpcomingChunk.y += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction)
        {
            centerOfUpcomingChunk.y -= gridSize.x;
        }
        else
        {
            return;
        }

        SetNewPredictionValues(centerOfUpcomingChunk);
    }

    //Ugly naming
    private void ChunkGenerationSequence(Vector2 centerOfUpcommingChunk)
    {
        Dictionary<Vector3, CubeData> newChunkFieldData = GenerateDataOfUpcommingChunk(centerOfUpcommingChunk);
        // Vzdy se ptat sa subscription // Pokud mam referenci tak delegat je zbytecny // nedelat oboustranny reference
        onDataOfNewChunkGenerated(newChunkFieldData, centerOfUpcommingChunk);
        GeneratePreloadedChunk(newChunkFieldData, centerOfUpcommingChunk);
    }


    /// <summary>
    /// Creates Dictionary fullfilled with data for upcoming chunk and adds it to the global dictionaryOfCentersWithItsChunkField dictionary
    /// </summary>
    /// <param name="newChunkCenter"></param>
    private Dictionary<Vector3, CubeData> GenerateDataOfUpcommingChunk(Vector2 newChunkCenter)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(newChunkCenter);
        Dictionary<Vector3, CubeData> newChunkFieldData = chunkGenerator.GenerateChunkData(newChunkCenter, startingChunkGenerationPosition);

        return newChunkFieldData;
    }

    private Vector3 ReturnBeginningPositionOfGeneratedChunk(Vector2 centerOfChunk)
    {
        return new Vector3(centerOfChunk.x - gridSize.x / 2, 0.0f, centerOfChunk.y - gridSize.x / 2);
    }
}
