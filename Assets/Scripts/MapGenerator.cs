using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();
    public float seed;

    [SerializeField] private float chunkGenerationDistanceFromEndOfTheChunk;
    [SerializeField] private uint gridSizeSides;
    [SerializeField] private uint gridSizeHeight;
    private List<Vector3> listOfCenters = new List<Vector3>();
    private Vector3 middlePointOfLastChunk;
    private float xPositivePrediction;
    private float xNegativePrediction;
    private float zPositivePrediction;
    private float zNegativePrediction;
    private bool doesInitialChunkBeenCreated = false;

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
        doesInitialChunkBeenCreated = true;
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

    //UGLY - Change name of methode, it's not descriptive
    private void ProcessChunkGenerationDistance()
    {
        Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);
        if (player.transform.position.x > xPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.x += gridSize.x;
        }
        else if (player.transform.position.x < xNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.x -= gridSize.x;
        }
        else if (player.transform.position.z > zPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.z += gridSize.x;
        }
        else if (player.transform.position.z < zNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.z -= gridSize.x;
        }
        else
        {
            return;
        }
        IfMiddlePointDontExistGenerateChunk(centerOfActualChunk);
    }

    private void IfMiddlePointDontExistGenerateChunk(Vector3 centerOfActualChunk)
    {
        if (!listOfCenters.Contains(centerOfActualChunk))
        {
            ChunkGenerationSequence(centerOfActualChunk);
        }
    }
 
    private void SetNewActiveChunkPrediction()
    {
        DebugManager.Log($"Middle point of last visited chunk: {middlePointOfLastChunk.ToString()}");

        Vector3 centerPointOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);

        if (player.transform.position.x > xPositivePrediction)
        {
            centerPointOfActualChunk.x += gridSize.x;
        }
        else if (player.transform.position.x < xNegativePrediction)
        {
            centerPointOfActualChunk.x -= gridSize.x;
        }
        else if (player.transform.position.z > zPositivePrediction)
        {
            centerPointOfActualChunk.z += gridSize.x;

        }
        else if (player.transform.position.z < zNegativePrediction)
        {
            centerPointOfActualChunk.z -= gridSize.x;
        }
        else
        {
            return;
        }

        SetNewPredictionValues(centerPointOfActualChunk);
    }

    private void ChunkGenerationSequence(Vector3 centerOfActualChunk)
    {
        listOfCenters.Add(centerOfActualChunk);
        Vector3 newChunkSpawnPosition = ReturnNewChunkPosition(centerOfActualChunk);
        if (doesInitialChunkBeenCreated)
        {
            StartCoroutine(chunkGenerator.GenerateChunkCoroutine(newChunkSpawnPosition));
        }
        else
        {
            chunkGenerator.GenerateChunk(newChunkSpawnPosition);
        }
    }

    private Vector3 ReturnNewChunkPosition(Vector3 centerOfChunk)
    {
        return new Vector3(centerOfChunk.x - gridSize.x / 2, 0.0f, centerOfChunk.z - gridSize.x / 2);
    }

    public void InstantiateCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject cube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            CubeParameters cubeParameters = cube.GetComponent<CubeParameters>();
            mapField.Add(spawnPosition, cubeParameters);
        }

        DebugManager.Log($"Count of mapField: {mapField.Count}");
    }        
    
    // Left it as two methodes or just as one?
    public GameObject? InstantiateAndReturnCube(Vector3 spawnPosition, GameObject cubePrefab)
    {
        if (!mapField.ContainsKey(spawnPosition))
        {
            GameObject cube = Instantiate<GameObject>(cubePrefab, spawnPosition, Quaternion.identity);
            CubeParameters cubeParameters = cube.GetComponent<CubeParameters>();
            mapField.Add(spawnPosition, cubeParameters);

            return cube;
        }

        DebugManager.Log($"Count of mapField: {mapField.Count}");
        return null;
    }
}
