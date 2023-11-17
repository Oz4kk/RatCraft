using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    public GameObject greenCube;
    public GameObject blueCube;
    public GameObject brownCube;
    public GameObject pinkCube;

    private PlayerSpawn playerSpawn;
    private GameObject player;
    private ChunkGenerator chunkGenerator;

    public Dictionary<Vector3, CubeParameters> mapField = new Dictionary<Vector3, CubeParameters>();
    public float seed;

    private List<Vector3> listOfCenters = new List<Vector3>();
    private Vector3 middlePointOfCurrentChunk;
    private Vector3 playerLocation;

    private void Awake()
    {
        playerSpawn = GetComponent<PlayerSpawn>();
        chunkGenerator = GetComponent<ChunkGenerator>();
    }

    private void Start()
    {
        player = playerSpawn.spawnedPlayer;
        playerLocation = player.transform.position;
        middlePointOfCurrentChunk = new Vector3(playerLocation.x, 0.0f, playerLocation.z);
        listOfCenters.Add(middlePointOfCurrentChunk);

        Vector3 initialSpawnPosition = new Vector3(playerLocation.x-50.0f, playerLocation.y, playerLocation.z-50.0f);
        chunkGenerator.GenerateChunk(initialSpawnPosition);
    }

    private void Update()
    {
        playerLocation = player.transform.position;
        nextChunkPredicter(chunkGenerator.gridSize.x, chunkGenerator.gridSize.z);
    }

    private void nextChunkPredicter(float x, float z)
    {
        //Debug.Log(middlePointOfCurrentChunk.ToString());

        float xPositivePrediction = middlePointOfCurrentChunk.x + 50.0f;
        float xNegativePrediction = middlePointOfCurrentChunk.x - 50.0f;
        float zPositivePrediction = middlePointOfCurrentChunk.z + 50.0f;
        float zNegativePrediction = middlePointOfCurrentChunk.z - 50.0f;

        if (playerLocation.x > xPositivePrediction && !listOfCenters.Contains(new Vector3(middlePointOfCurrentChunk.x + 100.0f, 0.0f, middlePointOfCurrentChunk.z)))
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x + 100.0f, 0.0f, middlePointOfCurrentChunk.z);
            listOfCenters.Add(middlePointOfCurrentChunk);
            Vector3 newChunkSpawnPosition = new Vector3(middlePointOfCurrentChunk.x - 50.0f, 0.0f, middlePointOfCurrentChunk.z - 50.0f);
            chunkGenerator.GenerateChunk(newChunkSpawnPosition);
        }
        else if (playerLocation.x < xNegativePrediction && !listOfCenters.Contains(new Vector3(middlePointOfCurrentChunk.x - 100.0f, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z)))
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x - 100.0f, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z);
            listOfCenters.Add(middlePointOfCurrentChunk);
            Vector3 newChunkSpawnPosition = new Vector3(middlePointOfCurrentChunk.x - 50.0f, 0.0f, middlePointOfCurrentChunk.z - 50.0f);
            chunkGenerator.GenerateChunk(newChunkSpawnPosition);
        }
        else if(playerLocation.z > zPositivePrediction && !listOfCenters.Contains(new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z + 100.0f)))
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z + 100.0f);
            listOfCenters.Add(middlePointOfCurrentChunk);
            Vector3 newChunkSpawnPosition = new Vector3(middlePointOfCurrentChunk.x - 50.0f, 0.0f, middlePointOfCurrentChunk.z - 50.0f);
            chunkGenerator.GenerateChunk(newChunkSpawnPosition);
        }
        else if(playerLocation.z < zNegativePrediction && !listOfCenters.Contains(new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z - 100.0f)))
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z - 100.0f);
            listOfCenters.Add(middlePointOfCurrentChunk);
            Vector3 newChunkSpawnPosition = new Vector3(middlePointOfCurrentChunk.x - 50.0f, 0.0f, middlePointOfCurrentChunk.z - 50.0f);
            chunkGenerator.GenerateChunk(newChunkSpawnPosition);
        }
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
}
