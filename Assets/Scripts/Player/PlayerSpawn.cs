using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnPosition;
    public GameObject spawnedPlayer;

    void Awake()
    {
        //Send also MapGenerator. Because I shouldn't touch prefabs.
        spawnedPlayer = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        PlayerController playerController = spawnedPlayer.GetComponent<PlayerController>();
        playerController.SetGameController(gameObject);
    }
}
