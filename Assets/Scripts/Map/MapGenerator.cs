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

    public Dictionary<Vector3, Dictionary<Vector3, GameObject>> dictionaryOfCentersWithItsChunkField = new Dictionary<Vector3, Dictionary<Vector3, GameObject>>();

    private Vector3 centerOfActualChunk;
    private Dictionary<Vector3, GameObject> chunkFieldOfActualChunk;

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
        ChunkGenerationSequence(centerOfActualChunk);
    }


    private void Update()
    {
        ProcessChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }

    private void SetNewPredictionValues(Vector3 middlePointOfActualChunk)
    {
        centerOfActualChunk = middlePointOfActualChunk;

        xPositivePrediction = middlePointOfActualChunk.x + gridSize.x / 2;
        xNegativePrediction = middlePointOfActualChunk.x - gridSize.x / 2;
        zPositivePrediction = middlePointOfActualChunk.z + gridSize.x / 2;
        zNegativePrediction = middlePointOfActualChunk.z - gridSize.x / 2;
    }

    private void ProcessChunkGenerationDistance()
    {
        Vector3 centerPointOfUpcomingChunk = new Vector3(centerOfActualChunk.x, 0.0f, centerOfActualChunk.z);
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
        if (!dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcomingChunk))
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
        DebugManager.Log($"Middle point of last visited chunk: {centerOfActualChunk.ToString()}");

        Vector3 centerPointOfUpcomingChunk = new Vector3(centerOfActualChunk.x, 0.0f, centerOfActualChunk.z);

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

    private void ChunkGenerationSequence(Vector3 centerOfUpcommingChunk)
    {
        DataGenerationSequence(centerOfUpcommingChunk);
        ChunkOptimalisationSequence(centerOfUpcommingChunk);
        CubeGenerationSequence(centerOfUpcommingChunk);
    }

    private void ChunkOptimalisationSequence(Vector3 centerOfUpcommingChunk)
    {
        Dictionary<Vector3, GameObject> dictionaryOfMegaChunk = new Dictionary<Vector3, GameObject>();

        Dictionary<Vector3, GameObject> actualDictionary = dictionaryOfCentersWithItsChunkField[centerOfUpcommingChunk];

        foreach (KeyValuePair<Vector3, GameObject> actualCube in actualDictionary)
        {
            dictionaryOfMegaChunk.Add(actualCube.Key, actualCube.Value);
        }

        centerOfUpcommingChunk.x += gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.z -= gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.z += gridSizeSides * 2.0f;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.x -= gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.z -= gridSizeSides * 2;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.x -= gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.z -= gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }

        centerOfUpcommingChunk.z -= gridSizeSides;
        if (dictionaryOfCentersWithItsChunkField.ContainsKey(centerOfUpcommingChunk))
        {

        }
    }

    private void DataGenerationSequence(Vector3 centerOfPredictedChunk)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(centerOfPredictedChunk);
        Dictionary<Vector3, GameObject> predictedDataChunkField = chunkGenerator.GenerateChunkData(startingChunkGenerationPosition);
        
        dictionaryOfCentersWithItsChunkField.Add(centerOfPredictedChunk, predictedDataChunkField);
    }

    private void CubeGenerationSequence(Vector3 centerOfUpcomingChunk)
    {
        Vector3 startingChunkGenerationPosition = ReturnBeginningPositionOfGeneratedChunk(centerOfUpcomingChunk);
        Dictionary<Vector3, GameObject> predictedChunkField = chunkGenerator.GeneratePreloadedChunk(centerOfUpcomingChunk);
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
