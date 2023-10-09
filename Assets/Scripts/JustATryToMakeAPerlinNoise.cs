using System;
using UnityEngine;

public class JustATryToMakeAPerlinNoise : MonoBehaviour
{
    [SerializeField] private int width;
    [SerializeField] private int height;



    private void Update()
    {
        Renderer renderer = GetComponent<Renderer>();
        renderer.material.mainTexture = GenereateTexture();
    }

    private Texture2D GenereateTexture()
    {
        Texture2D texture = new Texture2D(width, height);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Color color = CalculateColor(x, y);
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        return texture;
    }

    private Color CalculateColor(int x, int y)
    {
        float xCoords = (float)x / width;
        float yCoords = (float)y / height;

        float sample = Mathf.PerlinNoise(xCoords, yCoords);
        return new Color(sample, sample, sample);
    }
}
