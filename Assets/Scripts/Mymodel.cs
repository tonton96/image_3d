﻿using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Mymodel : MonoBehaviour
{
    private const int SIZE = 52;
    public Texture2D TextureXY;
    public Texture2D TextureYZ;
    public Texture2D TextureZX;

    private bool[,] mapxy;
    private bool[,] mapyz;
    private bool[,] mapzx;

    private bool[,,] map;

    private void Start()
    {
        //TextureScale.Bilinear(TextureXY, SIZE, SIZE);
        //TextureScale.Bilinear(TextureYZ, SIZE, SIZE);
        //TextureScale.Bilinear(TextureZX, SIZE, SIZE);

        mapxy = new bool[SIZE, SIZE];
        mapyz = new bool[SIZE, SIZE];
        mapzx = new bool[SIZE, SIZE];
        map = new bool[SIZE, SIZE, SIZE];

        for (int h = 0; h < SIZE; h++)
        {
            for (int w = 0; w < SIZE; w++)
            {
                Color c0 = TextureXY.GetPixel(w, h);
                mapxy[h, w] = (2 * c0.r + 5 * c0.g + c0.b) / 8.0f < 0.3f;
                Color c1 = TextureYZ.GetPixel(w, h);
                mapyz[h, w] = (2 * c1.r + 5 * c1.g + c1.b) / 8.0f < 0.3f;
                Color c2 = TextureZX.GetPixel(w, h);
                mapzx[h, w] = (2 * c2.r + 5 * c2.g + c2.b) / 8.0f < 0.3f;
            }
        }

        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    map[x, y, z] = mapxy[y, x] && mapyz[y, z] && mapzx[SIZE - x - 1, z];
                    if (map[x, y, z])
                    {
                        GameObject cube = Instantiate(Resources.Load("Cube", typeof(GameObject))) as GameObject;
                        cube.transform.parent = transform;
                        cube.transform.position = new Vector3(x, y, z);
                    }
                }
            }
        }
    }



    private void Update()
    {

    }

    private void SavePNG(Texture2D img)
    {
        byte[] bytes = img.EncodeToPNG();
        string filename = string.Format("screen_{0}x{1}_{2}.png",
                             img.width, img.height,
                             System.DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss"));
        File.WriteAllBytes(Application.persistentDataPath + "/" + filename, bytes);
    }
}
