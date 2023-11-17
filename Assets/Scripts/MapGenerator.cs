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

    private Action onActualMiddlePointChanged;

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
        playerLocation = player.transform.position;
        middlePointOfCurrentChunk = new Vector3(playerLocation.x, 0.0f, playerLocation.z);
        listOfCenters.Add(middlePointOfCurrentChunk);
        onActualMiddlePointChanged += SetNewPredictionValues;
        onActualMiddlePointChanged.Invoke();

        Vector3 initialSpawnPosition = ReturnNewChunkPosition(middlePointOfCurrentChunk);
        chunkGenerator.GenerateChunk(initialSpawnPosition);
    }

    private void Update()
    {
        playerLocation = player.transform.position;
        NextChunkPrediction();
    }

    private void SetNewPredictionValues()
    {
        xPositivePrediction = middlePointOfCurrentChunk.x + 50.0f;
        xNegativePrediction = middlePointOfCurrentChunk.x - 50.0f;
        zPositivePrediction = middlePointOfCurrentChunk.z + 50.0f;
        zNegativePrediction = middlePointOfCurrentChunk.z - 50.0f;
    }

    private void NextChunkPrediction()
    {
        Debug.Log(middlePointOfCurrentChunk.ToString());

        if (playerLocation.x > xPositivePrediction)
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x + 100.0f, 0.0f, middlePointOfCurrentChunk.z);
            if (!listOfCenters.Contains(middlePointOfCurrentChunk))
            {
                ChunkGenerationSequence();
            }
            onActualMiddlePointChanged?.Invoke();
        }
        else if (playerLocation.x < xNegativePrediction)
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x - 100.0f, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z);
            if (!listOfCenters.Contains(middlePointOfCurrentChunk))
            {
                ChunkGenerationSequence();
            }
            onActualMiddlePointChanged?.Invoke();
        }
        else if(playerLocation.z > zPositivePrediction)
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z + 100.0f);
            if (!listOfCenters.Contains(middlePointOfCurrentChunk))
            {
                ChunkGenerationSequence();
            }
            onActualMiddlePointChanged?.Invoke();
        }
        else if(playerLocation.z < zNegativePrediction)
        {
            middlePointOfCurrentChunk = new Vector3(middlePointOfCurrentChunk.x, middlePointOfCurrentChunk.y, middlePointOfCurrentChunk.z - 100.0f);
            if (!listOfCenters.Contains(middlePointOfCurrentChunk))
            {
                ChunkGenerationSequence();
            }
            onActualMiddlePointChanged?.Invoke();
        }
    }

    private void ChunkGenerationSequence()
    {
        listOfCenters.Add(middlePointOfCurrentChunk);
        Vector3 newChunkSpawnPosition = ReturnNewChunkPosition(middlePointOfCurrentChunk);
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
}
