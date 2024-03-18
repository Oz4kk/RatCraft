using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerHUD : MonoBehaviour
{
    [SerializeField] private PlayerSpawn playerSpawn;
    [SerializeField] private TextMeshProUGUI pointerHitLocation;

    private GameObject spawnedPlayer;
    private PlayerController playerController;

    void Start()
    {
        spawnedPlayer = playerSpawn.spawnedPlayer;
        playerController = spawnedPlayer.GetComponent<PlayerController>();
    }

    void Update()
    {
        pointerHitLocation.text = playerController.raycastHitLocationOfPointer?.GetStringOfVector3();
    }
}
