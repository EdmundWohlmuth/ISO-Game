using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.AI.Navigation;
using UnityEngine.AI;

public class RandomGen : MonoBehaviour
{
    public static RandomGen randomGen;
    [SerializeField] GameObject[] tile;
    public NavMeshSurface surface;
    GameObject tileToAdd;
    public int[,] tileMap;
    public int[,] resourceMap;
    public int smoothAmmount;

    [Header("Generation Values")]
    [SerializeField] int height;
    [SerializeField] int depth;
    [SerializeField] string seed;
    [SerializeField] [Range(0, 100)] int fillPercent;
    [SerializeField] [Range(0, 100)] int resourceFillPercent;
    [SerializeField] int clutterAmmount;

    [Header("Resource Values")]
    [SerializeField] int woodAmmount = 90;

    private void Awake()
    {
        if (randomGen == null)
        {
            randomGen = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (randomGen != this && randomGen != null)
        {
            Destroy(this.gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        seed = System.DateTime.Now.ToString();
        GenMap();
        surface.BuildNavMesh();
    }

    public void GenNavMesh()
    {
        // Create a new GameObject to represent the new walkable surface
        GameObject surface = new GameObject("WalkableSurface");

        // Create a NavMeshData object to hold the new walkable surface data
        NavMeshData navMeshData = new NavMeshData();

        // Collect the new walkable surface sources
        NavMeshBuildSource navMeshBuildSource = new NavMeshBuildSource();
        navMeshBuildSource.sourceObject = surface.GetComponent<MeshFilter>();
        navMeshBuildSource.shape = NavMeshBuildSourceShape.Mesh;

        // Build the new NavMesh data
        //Broken->//NavMeshBuilder.BuildNavMeshData(navMeshBuildSource, NavMesh.GetSettingsByID(0), navMeshData, transform.position, transform.rotation);

        // Add the new NavMesh data to the existing NavMesh
        NavMesh.AddNavMeshData(navMeshData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //-INITAL MAP GENERATION------
    void GenMap()
    {
        tileMap = new int[height, depth];
        resourceMap = new int[height, depth];
        FillMap();
        for (int i = 0; i < smoothAmmount; i++)
        {
            SmoothMap(tileMap);
        }
        GenerateMap();

        SetTrees();
        for (int i = 0; i < smoothAmmount; i++)
        {
            SmoothMap(resourceMap);
        }
        GenerateClutter();

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
                    tileMap[x, y] = 0;
                }
                else if (randomNum.Next(0, 100) > fillPercent)
                {                   
                    tileMap[x, y] = 1;
                }
                else tileMap[x, y] = 0;
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
                    tileToAdd = Instantiate(tile[0], new Vector3(x, -1, y), transform.rotation);
                    tileToAdd.transform.parent = this.gameObject.transform;
                    tileToAdd.AddComponent<BoxCollider>();
                    tileToAdd.layer = 11;
                    tileToAdd.tag = "ground";
                    tileToAdd.name = "Grass: " + x + "," + y.ToString();
                }           
            }
        }
    }

    void GenerateClutter()
    {
        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                if (resourceMap[x,y] == 1 && tileMap[x,y] == 1)
                {
                    if (Random.Range(0, 10) < 2)
                    {
                        tileToAdd = Instantiate(tile[2], new Vector3(x, 0, y), transform.rotation);
                        CreateTree(x, y);
                    }
                    else if (Random.Range(0, 10) < 3)
                    {
                        tileToAdd = Instantiate(tile[3], new Vector3(x, 0, y), transform.rotation);
                        CreateTree(x, y);
                    }
                }
            }
        }
    }
    void CreateTree(int x, int y)
    {
        tileToAdd.transform.parent = this.gameObject.transform;
        tileToAdd.name = "Tree: " + x + "," + y.ToString();
        tileToAdd.tag = "tree";
        tileToAdd.AddComponent<MeshCollider>();
        tileToAdd.AddComponent<ResourceNode>();
        tileToAdd.GetComponent<ResourceNode>().resource = 0;
        tileToAdd.GetComponent<ResourceNode>().maxResources = woodAmmount;
        tileToAdd.GetComponent<ResourceNode>().gen = this;
        tileToAdd.GetComponent<ResourceNode>().x = x;
        tileToAdd.GetComponent<ResourceNode>().y = y;
    }

    void SmoothMap(int[,] map)
    {
        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                int nearbyTiles = GetSurroundingTileCount(x, y, map);
                if (nearbyTiles > 4)
                {
                    map[x, y] = 1;
                    Debug.Log(x + ", " + y + " = " + map[x, y]);
                }
                else if (nearbyTiles < 4)
                {
                    map[x, y] = 0;
                    Debug.Log(x + ", " + y + " = " + map[x, y]);
                }
            }
        }
    }

    public int GetSurroundingTileCount(int mapX, int mapY, int[,] map) // checks what the value of the surrounding tiles are
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

                        count += map[x, y];

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
        //Debug.Log(count);
        return count;
    }
    
    //-POPULATE MAP---------------
    void SetTrees()
    {
        System.Random randomNum = new System.Random(seed.GetHashCode()/2);

        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                //Debug.Log(x + ", " + y);
                if (x == 0 || x == height - 1 || y == 0 || y == depth - 1) // encourages empty edges
                {
                    tileToAdd = null;
                    resourceMap[x, y] = 0;
                }
                else if (randomNum.Next(0, 100) > resourceFillPercent)
                {
                    resourceMap[x, y] = 1;
                }
                else resourceMap[x, y] = 0;
            }
        }
    }

    void SetClutter() // probs should move this to its own thing, repeat the Cellualr process again to get clumps of trees
    {
        for (int x = 0; x < height - 1; x++)
        {
            for (int y = 0; y < depth - 1; y++)
            {
                if (tileMap[x, y] == 1)
                {
                    if (Random.Range(0, 50) < 2)
                    {
                        tileToAdd = Instantiate(tile[4], new Vector3(x, 0, y), transform.rotation);
                        tileToAdd.name = "Plant: " + x + "," + y.ToString();
                    }
                    else if (Random.Range(0, 30) < 3)
                    {
                        tileToAdd = Instantiate(tile[5], new Vector3(x, 0, y), transform.rotation);
                        tileToAdd.name = "Plant: " + x + "," + y.ToString();
                    }
                    else if (Random.Range(0, 30) < 4)
                    {
                        tileToAdd = Instantiate(tile[6], new Vector3(x, 0, y), transform.rotation);
                        tileToAdd.name = "Plant: " + x + "," + y.ToString();
                    }
                }
            }
        }
    }

    //-SET-WORLD-VALUES
  /*  public void SetOccupied(int mapX, int mapY)
    {
        for (int x = mapX - 1; x <= mapX + 1; x++)
        {
            for (int y = mapY - 1; y <= mapY + 1; y++)
            {
                if (x >= 0 && x < height && y >= 0 && y < depth)
                {
                    if (x != mapX || y != mapY)
                    {
                        resourceMap[mapX, mapX] = 1;
                    }
                }
            }
        }
    }*/

    //-UPDATE-LEVEL-DATA-
    //-BAKE-NAV-MESH-----
    public void UpdateNavMesh()
    {
        surface.BuildNavMesh();
    }
    public void UpdateResourceMapData(int x, int z)
    {
        resourceMap[x, z] = 0;
        Debug.Log("new Value: " + resourceMap[x, z]);
    }
    public void UpdateTileMapData(int x, int z)
    {

    }
    public void SetOccupied(int x, int z)
    {
        resourceMap[x, z] = 1;
        Debug.Log(resourceMap[x, z]);
    }
}
