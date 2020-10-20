using System.Collections;
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

    private float Speed;
    private float RollSpeed;
    private bool rotateEnable;
    private Vector3 mousePos;

    private void Start()
    {
        Init();
        rotateEnable = false;
        Speed = 1000;
        RollSpeed = 0.2f;
    }

    private void Update()
    {
        Camera.main.transform.Translate(Vector3.forward * Input.GetAxis("Mouse ScrollWheel") * Time.deltaTime * Speed, Space.World);
        if (Input.GetMouseButtonDown(0))
        {
            rotateEnable = true;
            mousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(0))
        {
            rotateEnable = false;
        }
        if (rotateEnable)
        {
            Vector3 newPos = Input.mousePosition;
            transform.RotateAround(new Vector3((SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f), Vector3.right, (newPos.y - mousePos.y) * RollSpeed);
            transform.RotateAround(new Vector3((SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f), Vector3.up, (-newPos.x + mousePos.x) * RollSpeed);
            mousePos = newPos;
        }
    }

    private void Init()
    {
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

        int count = 0;
        for (int x = 0; x < SIZE; x++)
        {
            for (int y = 0; y < SIZE; y++)
            {
                for (int z = 0; z < SIZE; z++)
                {
                    map[x, y, z] = mapxy[y, x] && mapyz[y, z] && mapzx[SIZE - x - 1, z];
                    if (map[x, y, z])
                    {
                        bool create = true;
                        if (x > 0 && x < SIZE - 1 && y > 0 && y < SIZE - 1 && z > 0 && z < SIZE - 1)
                        {
                            create = !(map[x - 1, y, z] && map[x + 1, y, z] && map[x, y - 1, z] && map[x, y + 1, z] && map[x, y, z - 1] && map[x, y, z + 1]);
                        }
                        if (create)
                        {
                            GameObject cube = Instantiate(Resources.Load("Cube", typeof(GameObject))) as GameObject;
                            cube.transform.parent = transform;
                            cube.transform.position = new Vector3(x, y, z);
                            count++;
                        }
                    }
                }
            }
        }
        for (int h = 0; h < SIZE; h++)
        {
            for (int w = 0; w < SIZE; w++)
            {
                if (mapxy[h, w])
                {
                    bool c0 = false;
                    for (int i = 0; i < SIZE; i++)
                    {
                        c0 = c0 || map[w, h, i];
                    }
                    if (!c0)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            GameObject cube = Instantiate(Resources.Load("Cubexy", typeof(GameObject))) as GameObject;
                            cube.transform.parent = transform;
                            cube.transform.position = new Vector3(w, h, Random.Range(0, SIZE));
                            count++;
                        }
                    }
                }

                if (mapyz[h, w])
                {
                    bool c1 = false;
                    for (int i = 0; i < SIZE; i++)
                    {
                        c1 = c1 || map[i, h, w];
                    }
                    if (!c1)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            GameObject cube = Instantiate(Resources.Load("Cubeyz", typeof(GameObject))) as GameObject;
                            cube.transform.parent = transform;
                            cube.transform.position = new Vector3(Random.Range(0, SIZE), h, w);
                            count++;
                        }
                    }
                }

                if (mapzx[h, w])
                {
                    bool c2 = false;
                    for (int i = 0; i < SIZE; i++)
                    {
                        c2 = c2 || map[SIZE - h - 1, i, w];
                    }
                    if (!c2)
                    {
                        for (int j = 0; j < 5; j++)
                        {
                            GameObject cube = Instantiate(Resources.Load("Cubezx", typeof(GameObject))) as GameObject;
                            cube.transform.parent = transform;
                            cube.transform.position = new Vector3(SIZE - 1 - h, Random.Range(0, SIZE), w);
                            count++;
                        }
                    }
                }
            }
        }

        Vector3 rotate = new Vector3(Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f), Random.Range(-1.0f, 1.0f)).normalized;
        float alpha = Random.Range(0, 360.0f);
        transform.RotateAround(new Vector3((SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f, (SIZE - 1) / 2.0f), rotate, alpha);
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
