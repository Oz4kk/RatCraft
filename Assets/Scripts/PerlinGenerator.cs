using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGenerator : MonoBehaviour
{
    [SerializeField] private int perlinTextureSizeX;
    [SerializeField] private int perlinTextureSizeY;
    [SerializeField] private bool randomizeNoiseOffset;
    [SerializeField] private Vector2 perlinOffset;
    [SerializeField] private float noiseScale = 1.0f;
    [SerializeField] private int perlinGridStepSizeX;
    [SerializeField] private int perlinGridStepSizeY;

    [SerializeField] private bool visualizeGrid = false;
    [SerializeField] private GameObject visualizationCube;
    [SerializeField] private float visualisationHeightScale = 5.0f;

    private Texture2D perlinTexture;

    private void GenerateNoise()
    {
        if (randomizeNoiseOffset)
        {
            perlinOffset = new Vector2(Random.Range(0, 99999), Random.Range(0, 99999));
        }

        perlinTexture = new Texture2D(perlinTextureSizeX, perlinTextureSizeY);

        for (int x = 0; x < perlinTextureSizeX; x++)
        {
            for (int y = 0; y < perlinTextureSizeY; y++)
            {
                //perlinTexture.SetPixel(x, y SampleNoise(x, y));
            }
        }
    }
}
