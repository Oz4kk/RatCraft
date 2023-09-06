using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cube : MonoBehaviour
{
    [SerializeField] private float brittleness = 5.0f;
    public Vector3 coordinates;
    public GameObject chosenCube;

    MapGenerator mapGenerator = new MapGenerator();

    public Cube(Vector3 coordinates, GameObject chosenCube)
    {
        if (doesCoordinateExist(coordinates))
        {
            return;
        }
        this.coordinates = coordinates;
        this.chosenCube = chosenCube;
    }

    public bool doesCoordinateExist(Vector3 coordinates)
    {
        List<Cube> listOfCubes = mapGenerator.GetList();
        foreach (Cube cube in listOfCubes)
        {
            if (cube.coordinates == coordinates)
            {
                return true;
            }
        }
        return false;
    }
}
