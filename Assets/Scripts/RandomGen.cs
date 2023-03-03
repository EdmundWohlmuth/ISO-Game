using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RandomGen : MonoBehaviour
{
    [SerializeField] GameObject[] tile;
    GameObject tileToAdd;
    int[,] tileMap;
    public int smoothAmmount;

    [SerializeField] int height;
    [SerializeField] int depth;
    [SerializeField] string seed;
    [SerializeField] [Range(0, 100)] int fillPercent;

    // Start is called before the first frame update
    void Start()
    {
        seed = System.DateTime.Now.ToString();
        GenMap();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //-INITAL MAP GENERATION------
    void GenMap()
    {
        tileMap = new int[height, depth];
        FillMap();
        for (int i = 0; i < smoothAmmount; i++)
        {
            SmoothMap();
        }
        GenerateMap();
        SetClutter();
    }

    void FillMap() // First Pass, randomly sets values to tiles
    {
        System.Random randomNum = new System.Random(seed.GetHashCode());

        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                //Debug.Log(x + ", " + y);
                if (x == 0 || x == height - 1 || y == 0 || y == depth - 1) // encourages empty edges
                {
                    tileToAdd = null; 
                    tileMap[x, y] = 1;
                }
                else if (randomNum.Next(0, 100) > fillPercent)
                {                   
                    tileMap[x, y] = 0;
                }
                else tileMap[x, y] = 1;
            }
        }
    }

    void GenerateMap()
    {
        for (int x = 0; x < height -1; x++)
        {
            for (int y = 0; y < depth -1; y++)
            {
                if (tileMap[x, y] == 1)
                {
                    tileToAdd = Instantiate(tile[0], new Vector3(x - (height / 2), -1, y - (depth / 2)), transform.rotation);
                    tileToAdd.transform.parent = this.gameObject.transform;
                    tileToAdd.AddComponent<BoxCollider>();
                }           
            }
        }
    }

    void SmoothMap()
    {
        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                int nearbyTiles = GetSurroundingTileCount(x, y);
                if (nearbyTiles > 4)
                {
                    tileMap[x, y] = 1;
                }
                else if (nearbyTiles < 4)
                {
                    tileMap[x, y] = 0;
                }
            }
        }
    }

    int GetSurroundingTileCount(int mapX, int mapY) // checks what the value of the surrounding tiles are
    {
        int count = 0;

        for (int x = mapX - 1; x <= mapX + 1; x++)
        {
            for (int y = mapY - 1; y <= mapY + 1; y++)
            {
                if (x >= 0 && x < height && y >= 0 && y < depth)
                {
                    if (x != mapX || y != mapY)
                    {

                        count += tileMap[x, y];

                        // Example:
                        //  
                        //    1 1 1
                        //    1 + 1
                        //    0 0 0
                        //
                        // Value > 4, center object will be land
                    }
                }
            }
        }

        return count;
    }
    
    //-POPULATE MAP---------------
    void SetClutter() // probs should move this to its own thing, repeat the Cellualr process again to get clumps of trees
    {
        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                if (tileMap[x,y] == 1)
                {
                    if (Random.Range(0, 30) < 2)
                    {
                        Instantiate(tile[2], new Vector3(x - (height / 2), 0, y - (depth / 2)), transform.rotation);
                    }
                    else if (Random.Range(0, 30) < 3)
                    {
                        Instantiate(tile[3], new Vector3(x - (height / 2), 0, y - (depth / 2)), transform.rotation);
                    }
                }
            }
        }
    }
}
