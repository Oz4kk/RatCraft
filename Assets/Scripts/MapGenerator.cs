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

    public static Dictionary<Vector3, CubeParametres> mapField = new Dictionary<Vector3, CubeParametres>();

    private void Start()
    {
        for (int x = 0; x < gridSize.x; x++)
        {
            for (int y = 0; y < gridSize.y; y++)
            {
                for (int z = 0; z < gridSize.z; z++)
                {
                    Vector3 spawnPosition = new Vector3(x, y, z);

                    CubeParametres newCube = new CubeParametres(cubePrefab);

                    ulong totalOfDictionaryBefore = CountValuesInCollection(mapField);

                    mapField.Add(spawnPosition, newCube);

                    ulong totalOfDictionaryAfter = CountValuesInCollection(mapField);

                    if (totalOfDictionaryAfter > totalOfDictionaryBefore)
                    {
                        Instantiate(cubePrefab, spawnPosition, Quaternion.identity);
                    }
                }
            }
        }
    }

    private ulong CountValuesInCollection(Dictionary<Vector3, CubeParametres> mapField)
    {
        ulong total = 0;
        foreach (KeyValuePair<Vector3, CubeParametres> item in mapField)
        {
            total++;
        }
        return total;
    }
}
