using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [Serializable]
    private struct GridSize
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

    [SerializeField] private GameObject cubePrefab;
    [SerializeField] private GridSize gridSize = new GridSize(5, 5, 5);
    private static List<Cube> mapList;

    public void SetList(Cube newCube)
    {
        mapList.Add(newCube);
    }    
    public List<Cube> GetList()
    {
        return mapList;
    }

    void Start()
    {
        mapList = new List<Cube>();
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 spawnPosition = new Vector3(x, y, z);

                    Cube newCube = new Cube(spawnPosition, cubePrefab);

                    if (!newCube.doesCoordinateExist(newCube.coordinates))
                    {
                        mapList.Add(newCube);
                        Instantiate(newCube.chosenCube, newCube.coordinates, Quaternion.identity);
                    }
                }
            }
        }
    }
}
