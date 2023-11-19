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

    [SerializeField] float chunkGenerationDistanceFromEndOfTheChunk;
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
        Vector3 centerOfActualChunk = new Vector3(player.transform.position.x, 0.0f, player.transform.position.z);
        ChunkGenerationSequence(centerOfActualChunk);
        SetNewPredictionValues(centerOfActualChunk);

        Vector3 initialSpawnPosition = ReturnNewChunkPosition(centerOfActualChunk);
        chunkGenerator.GenerateChunk(initialSpawnPosition);
    }

    private void Update()
    {
        PlayerReachedChunkGenerationDistance();
        SetNewActiveChunkPrediction();
    }

    private void SetNewPredictionValues(Vector3 middlePointOfActualChunk)
    {
        xPositivePrediction = middlePointOfActualChunk.x + 50.0f;
        xNegativePrediction = middlePointOfActualChunk.x - 50.0f;
        zPositivePrediction = middlePointOfActualChunk.z + 50.0f;
        zNegativePrediction = middlePointOfActualChunk.z - 50.0f;
    }

    private void PlayerReachedChunkGenerationDistance()
    {
        if (player.transform.position.x > xPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x + 100.0f, 0.0f, middlePointOfLastChunk.z);
            if (!listOfCenters.Contains(centerOfActualChunk))
            {
                ChunkGenerationSequence(centerOfActualChunk);
            }
        }
        else if (player.transform.position.x < xNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x - 100.0f, 0.0f, middlePointOfLastChunk.z);
            if (!listOfCenters.Contains(centerOfActualChunk))
            {
                ChunkGenerationSequence(centerOfActualChunk);
            }
        }
        else if (player.transform.position.z > zPositivePrediction - chunkGenerationDistanceFromEndOfTheChunk)
        {
            Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z + 100.0f);
            if (!listOfCenters.Contains(centerOfActualChunk))
            {
                ChunkGenerationSequence(centerOfActualChunk);
            }
        }
        else if (player.transform.position.z < zNegativePrediction + chunkGenerationDistanceFromEndOfTheChunk)
        {
            Vector3 centerOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z - 100.0f);
            if (!listOfCenters.Contains(centerOfActualChunk))
            {
                ChunkGenerationSequence(centerOfActualChunk);
            }
        }
    }

    private void SetNewActiveChunkPrediction()
    {
        DebugManager.Log($"Middle point of last visited chunk: {middlePointOfLastChunk.ToString()}");

        if (player.transform.position.x > xPositivePrediction)
        {
            Vector3 middlePointOfActualChunk = new Vector3(middlePointOfLastChunk.x + 100.0f, 0.0f, middlePointOfLastChunk.z);
            middlePointOfLastChunk = middlePointOfActualChunk;
            SetNewPredictionValues(middlePointOfActualChunk);
        }
        else if (player.transform.position.x < xNegativePrediction)
        {
            Vector3 middlePointOfActualChunk = new Vector3(middlePointOfLastChunk.x - 100.0f, 0.0f, middlePointOfLastChunk.z);
            middlePointOfLastChunk = middlePointOfActualChunk;
            SetNewPredictionValues(middlePointOfActualChunk);
        }
        else if(player.transform.position.z > zPositivePrediction)
        {
            Vector3 middlePointOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z + 100.0f);
            middlePointOfLastChunk = middlePointOfActualChunk;
            SetNewPredictionValues(middlePointOfActualChunk);
        }
        else if(player.transform.position.z < zNegativePrediction)
        {
            Vector3 middlePointOfActualChunk = new Vector3(middlePointOfLastChunk.x, 0.0f, middlePointOfLastChunk.z - 100.0f);
            middlePointOfLastChunk = middlePointOfActualChunk;
            SetNewPredictionValues(middlePointOfActualChunk);
        }
    }

    private void ChunkGenerationSequence(Vector3 middlePointOfActualChunk)
    {
        listOfCenters.Add(middlePointOfActualChunk);
        Vector3 newChunkSpawnPosition = ReturnNewChunkPosition(middlePointOfActualChunk);
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
