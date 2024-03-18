using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MapGenerator : MonoBehaviour
{
    public Action<Dictionary<Vector3, CubeData>, Vector3> onDataOfNewChunkGenerated;

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

    [HideInInspector] public GridSize gridSize = new GridSize(100, 16, 100);

    // TO-DO: Delete this after finishing optimalisation
    public Dictionary<Vector3, GameObject> mapField = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, CubeData> mapFieldData = new Dictionary<Vector3, CubeData>();

    public float seed;

    [SerializeField] private float chunkGenerationDistanceFromEndOfTheChunk;
    [SerializeField] private uint gridSizeSides;
    [SerializeField] private uint gridSizeHeight;

    public Dictionary<Vector3, Dictionary<Vector3, CubeData>> dictionaryOfCentersWithItsDataChunkField = new Dictionary<Vector3, Dictionary<Vector3, CubeData>>();
    public Dictionary<Vector3, Dictionary<Vector3, CubeData>> dictionaryOfCentersWithItsChunkField = new Dictionary<Vector3, Dictionary<Vector3, CubeData>>();

    private Vector3 middlePointOfLastChunk;
    private float xPositivePrediction;
    private float xNegativePrediction;
    private float zPositivePrediction;
    private float zNegativePrediction;

    private float grassValue = 10.0f;
    private float dirtValue = 7.0f;
    private float rockValue = 2.0f;

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
        player = playerSpawn.spawnedPlayer;
        SetNewPredictionValues(new Vector3(player.transform.position.x, 0.0f, player.transform.position.z));
        ChunkGenerationSequence(middlePointOfLastChunk);
    }


    private void Update()
    {
        ProcessChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }

    private void SetNewPredictionValues(Vector3 middlePointOfActualChunk)
    {
        middlePointOfLastChunk = middlePointOfActualChunk;

        xPositivePrediction = middlePointOfActualChunk.x + gridSize.x / 2;
        xNegativePrediction = middlePointOfActualChunk.x - gridSize.x / 2;
        zPositivePrediction = middlePointOfActualChunk.z + gridSize.x / 2;
        zNegativePrediction = middlePointOfActualChunk.z - gridSize.x / 2;
    }

    private void ProcessChunkGenerationDistance()
    {
        Vector3 centerPointOfUpcomingChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);
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
            centerPointOfUpcomingChunk.z += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerPointOfUpcomingChunk.z -= gridSize.x;
        }
        else
        {
            return;
        }
        IfCenterDontExistGenerateChunk(centerPointOfUpcomingChunk);
    }

    private void IfCenterDontExistGenerateChunk(Vector3 centerOfUpcomingChunk)
    {
        if (!dictionaryOfCentersWithItsDataChunkField.ContainsKey(centerOfUpcomingChunk))
        {
            ChunkGenerationSequence(centerOfUpcomingChunk);
        }
    }

    public void DeleteCube(CubeParameters actualCube)
    {
        mapField.Remove(actualCube.gameObject.transform.position);
        Destroy(actualCube.gameObject);

        onCubeDestroyed.Invoke(actualCube.gameObject);
    }

    private void SetNewActiveChunkPrediction()
    {
        Debug.Log($"Middle point of last visited chunk: {middlePointOfLastChunk.ToString()}");

        Vector3 centerOfUpcomingChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);

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
            centerOfUpcomingChunk.z += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction)
        {
            centerOfUpcomingChunk.z -= gridSize.x;
        }
        else
        {
            return;
        }

        SetNewPredictionValues(centerOfUpcomingChunk);
    }

    private void ChunkGenerationSequence(Vector3 centerOfUpcommingChunk)
    {
        GenerateDataOfUpcommingChunk(centerOfUpcommingChunk);
        onDataOfNewChunkGenerated(dictionaryOfCentersWithItsDataChunkField[centerOfUpcommingChunk], centerOfUpcommingChunk);
        GeneratePreloadedChunk(centerOfUpcommingChunk);
    }

    public void GeneratePreloadedChunk(Vector3 centerOfUpcommingChunk)
    {
        Dictionary<Vector3, CubeData> chunkField = dictionaryOfCentersWithItsDataChunkField[centerOfUpcommingChunk];

        foreach (KeyValuePair<Vector3, CubeData> actualCube in chunkField)
        {
            if (actualCube.Value.isCubeDataSurrounded == true)
            {
                continue;
            }
            GameObject cube = InstantiateAndReturnCube(actualCube.Key, actualCube.Value.cubePrefab);
            ChooseTexture(cube);

            // TO-DO: Here I ended, fullfill new dictionary with Instantiated cube chunks
            //dictionaryOfCentersWithItsChunkField.Add();
        }
    }

    /// <summary>
    /// Creates Dictionary fullfilled with data for upcomming chunk and adds it to the global dictionaryOfCentersWithItsChunkField dictionary
    /// </summary>
    /// <param name="centerOfPredictedChunk"></param>
    private void GenerateDataOfUpcommingChunk(Vector3 centerOfPredictedChunk)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(centerOfPredictedChunk);
        Dictionary<Vector3, CubeData> predictedDataChunkField = chunkGenerator.GenerateChunkData(startingChunkGenerationPosition);

        dictionaryOfCentersWithItsDataChunkField.Add(centerOfPredictedChunk, predictedDataChunkField);
    }

    private Vector3 ReturnBeginningPositionOfGeneratedChunk(Vector3 centerOfChunk)
    {
        return new Vector3(centerOfChunk.x - gridSize.x / 2, 0.0f, centerOfChunk.z - gridSize.x / 2);
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

    public void ChooseTexture(GameObject actualCube)
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

    public Dictionary<Vector3, GameObject> GetChunkBorderCubes()
    {
        Dictionary<Vector3, GameObject> borderField = new Dictionary<Vector3, GameObject>();

        return borderField;
    }

    public Vector3 ChooseNeighbourSide()
    {
        return Vector3.right;
    }
}
