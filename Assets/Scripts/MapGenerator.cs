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

        Vector3 initialSpawnPosition = new Vector3(playerLocation.x-50.0f, playerLocation.y, playerLocation.z-50.0f);
        chunkGenerator.GenerateChunk(initialSpawnPosition);
    }

    private void Update()
    {
        playerLocation = player.transform.position;
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
