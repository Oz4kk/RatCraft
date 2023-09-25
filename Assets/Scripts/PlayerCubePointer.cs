using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCubePointer : MonoBehaviour
{
    [SerializeField] private GameObject baseCubePrefab;
    [SerializeField] private GameObject gameController;
    private MapGenerator mapGenerator;
    private PlayerCubePlacement playerCubePlacement;
    private GameObject pointerCube;

    void Start()
    {
        mapGenerator = baseCubePrefab.GetComponent<MapGenerator>();
        playerCubePlacement = GetComponent<PlayerCubePlacement>();

        baseCubePrefab.GetComponent<BoxCollider>().enabled = false;
        baseCubePrefab.GetComponent<MeshRenderer>().enabled = true;
        pointerCube = Instantiate(baseCubePrefab, new Vector3(0, 0, 0), Quaternion.identity);
    }

    public void ShowCubePosition()
    {
        Vector3 pointerPosition = playerCubePlacement.PrepareBlock();
        pointerCube.transform.position = pointerPosition;
    }
}
