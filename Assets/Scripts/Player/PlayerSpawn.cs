using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawn : MonoBehaviour
{
    [SerializeField] private GameObject prefabToSpawn;
    [SerializeField] private Vector3 spawnPosition;

    [HideInInspector] public GameObject spawnedPlayer { get; private set; }

    void Awake()
    {
        //Send also MapGenerator. Because I shouldn't touch prefabs.
        spawnedPlayer = Instantiate(prefabToSpawn, spawnPosition, Quaternion.identity);
        PlayerController playerController = spawnedPlayer.GetComponent<PlayerController>();
        playerController.SetGameController(gameObject);
    }
}
