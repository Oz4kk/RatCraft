using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;

public class MapGenerator : MonoBehaviour
{
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

    public Dictionary<Vector3, GameObject> mapField = new Dictionary<Vector3, GameObject>();
    public Dictionary<Vector3, GameObject> mapFieldData = new Dictionary<Vector3, GameObject>();

    public float seed;

    [SerializeField] private float chunkGenerationDistanceFromEndOfTheChunk;
    [SerializeField] private uint gridSizeSides;
    [SerializeField] private uint gridSizeHeight;
    private Dictionary<Vector3, Dictionary<Vector3, GameObject>> listOfDataCenters;
    private Dictionary<Vector3, Dictionary<Vector3, GameObject>> listOfGeneratedCenters;
    //private List<Vector3> listOfCentersData = new List<Vector3>();
    //private List<Vector3> listOfGeneratedCenters = new List<Vector3>();
    private Vector3 middlePointOfLastChunk;
    private float xPositivePrediction;
    private float xNegativePrediction;
    private float zPositivePrediction;
    private float zNegativePrediction;

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
        DataGenerationOfCubesAroundInitialChunk();
    }


    private void Update()
    {
        ProcessChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }
    private void DataGenerationOfCubesAroundInitialChunk()
    {
        Vector3 middlePointOfPredictedChunk = middlePointOfLastChunk;

        middlePointOfPredictedChunk.x = middlePointOfPredictedChunk.x + (float)gridSizeSides;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z + (float)gridSizeSides;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z - (float)gridSizeSides * 2.0f;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk = middlePointOfLastChunk;
        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z + (float)gridSizeSides;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z - (float)gridSizeSides * 2.0f;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk = middlePointOfLastChunk;
        middlePointOfPredictedChunk.x = middlePointOfPredictedChunk.x - (float)gridSizeSides;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z + (float)gridSizeSides;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        middlePointOfPredictedChunk.z = middlePointOfPredictedChunk.z - (float)gridSizeSides * 2.0f;
        ChunkDataGenerationSequence(middlePointOfPredictedChunk);

        foreach (var item in listOfDataCenters)
        {
            //Debug.Log(item.GetStringOfVector3());
        }
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
        IfMiddlePointDontExistGenerateChunk(centerPointOfUpcomingChunk);
    }

    private void IfMiddlePointDontExistGenerateChunk(Vector3 centerOfUpcomingChunk)
    {
        if (!listOfGeneratedCenters.ContainsKey(centerOfUpcomingChunk))
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
        DebugManager.Log($"Middle point of last visited chunk: {middlePointOfLastChunk.ToString()}");

        Vector3 centerPointOfUpcomingChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);

        if (player.transform.position.x > xPositivePrediction)
        {
            centerPointOfUpcomingChunk.x += gridSize.x;
        }
        else if (player.transform.position.x < xNegativePrediction)
        {
            centerPointOfUpcomingChunk.x -= gridSize.x;
        }
        else if (player.transform.position.z > zPositivePrediction)
        {
            centerPointOfUpcomingChunk.z += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction)
        {
            centerPointOfUpcomingChunk.z -= gridSize.x;
        }
        else
        {
            return;
        }

        SetNewPredictionValues(centerPointOfUpcomingChunk);
    }

    private void ChunkGenerationSequence(Vector3 centerOfUpcomingChunk)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(centerOfUpcomingChunk);
        Dictionary<Vector3, GameObject> predictedChunkField = GeneratePreloadedChunk(centerOfUpcomingChunk);

        listOfGeneratedCenters.Add(centerOfUpcomingChunk, predictedChunkField);
    }    
    
    private void ChunkDataGenerationSequence(Vector3 centerOfPredictedChunk)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(centerOfPredictedChunk);
        Dictionary<Vector3, GameObject> predictedChunkFieldData = chunkGenerator.GenerateChunkData(startingChunkGenerationPosition);

        listOfDataCenters.Add(centerOfPredictedChunk, predictedChunkFieldData);
    }

    private Vector3 ReturnBeginningPositionOfGeneratedChunk(Vector3 centerOfChunk)
    {
        return new Vector3(centerOfChunk.x - gridSize.x / 2, 0.0f, centerOfChunk.z - gridSize.x / 2);
    }

    private Dictionary<Vector3, GameObject> GeneratePreloadedChunk(Vector3 centerOfUpcommingChunk)
    {
        foreach (var item in listOfDataCenters[centerOfUpcommingChunk])
        {
            Instantiate(item.Value);
        }

        return listOfDataCenters[centerOfUpcommingChunk];
    }

    public GameObject? InstantiateAndReturnCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject actualCube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            actualCube.transform.parent = transform;
            mapField.Add(spawnPosition, actualCube);

            return actualCube;
        }

        DebugManager.Log($"Count of mapField: {mapField.Count}");
        return null;
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
