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

    public GridSize gridSize = new GridSize(100, 16, 100);
    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();
    public float seed;

    [SerializeField] private float chunkGenerationDistanceFromEndOfTheChunk;
    private List<Vector3> listOfCenters = new List<Vector3>();
    private Vector3 middlePointOfLastChunk;
    private float xPositivePrediction;
    private float xNegativePrediction;
    private float zPositivePrediction;
    private float zNegativePrediction;

    private void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
        chunkGenerator = GetComponent<ChunkGenerator>();
    }

    private void Start()
    {
        player = playerSpawn.spawnedPlayer;
        SetNewPredictionValues(new Vector3(player.transform.position.x, 0.0f, player.transform.position.z));
        ChunkGenerationSequence(middlePointOfLastChunk);
    }

    private void Update()
    {
        PlayerReachedChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }

    private void SetNewPredictionValues(Vector3 middlePointOfActualChunk)
    {
        middlePointOfLastChunk = middlePointOfActualChunk;

        xPositivePrediction = middlePointOfActualChunk.x + 50.0f;
        xNegativePrediction = middlePointOfActualChunk.x - 50.0f;
        zPositivePrediction = middlePointOfActualChunk.z + 50.0f;
        zNegativePrediction = middlePointOfActualChunk.z - 50.0f;
    }

    //UGLY - Change name of methode, it's not descriptive
    private void PlayerReachedChunkGenerationDistance()
    {
        Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z);
        if (player.transform.position.x > xPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.x += 100.0f;
        }
        else if (player.transform.position.x < xNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.x -= 100.0f;
        }
        else if (player.transform.position.z > zPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.z += 100.0f;
        }
        else if (player.transform.position.z < zNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            centerOfActualChunk.z -= 100.0f;
        }
        else
        {
            return;
        }
        GenerateChunkIfMiddlePointDontExist(centerOfActualChunk);
    }

    private void GenerateChunkIfMiddlePointDontExist(Vector3 centerOfActualChunk)
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
            centerPointOfActualChunk.x += 100.0f;
        }
        else if (player.transform.position.x < xNegativePrediction)
        {
            centerPointOfActualChunk.x -= 100.0f;
        }
        else if (player.transform.position.z > zPositivePrediction)
        {
            centerPointOfActualChunk.z += 100.0f;

        }
        else if (player.transform.position.z < zNegativePrediction)
        {
            centerPointOfActualChunk.z -= 100.0f;
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
        chunkGenerator.GenerateChunk(newChunkSpawnPosition);
    }

    private Vector3 ReturnNewChunkPosition(Vector3 centerOfChunk)
    {
        return new Vector3(centerOfChunk.x - 50.0f, 0.0f, centerOfChunk.z - 50.0f);
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
