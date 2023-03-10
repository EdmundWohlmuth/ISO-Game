using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TempController : MonoBehaviour
{
    float verticalSpeed = 15f;
    float horizontalSpeed = 15f;
    Camera cam;
    [Header("tilemap")]
    [SerializeField] RandomGen randGen;
    int[,] worldMap;
    int[,] clutterMap;
    [SerializeField] GameObject dummyObject;

    [Header("Build Objects")]
    public GameObject house;
    [SerializeField] bool building;
    public GameObject farm;
    [SerializeField] bool tillingLand;

    [Header("Villager Control")]
    public VillagerController VC = null;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        dummyObject = Instantiate(dummyObject, transform.position, transform.rotation);
        building = true;
    }

    // Update is called once per frame
    void Update()
    {
        worldMap = randGen.tileMap;
        clutterMap = randGen.clutterMap;

        // camera movement
        Movement();
        CameraScroll();

        // raycast to curosr pos
        MouseToPos();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(dummyObject);
            dummyObject = null;
        }
    }

    void Movement()
    {
        verticalSpeed = Input.GetAxis("Vertical") * Time.deltaTime;
        transform.Translate(verticalSpeed * 8f, 0, 0);

        horizontalSpeed = Input.GetAxis("Horizontal") * Time.deltaTime;
        transform.Translate(0, 0, horizontalSpeed * -8f);
    }
    void CameraScroll()
    {
        if (Input.mouseScrollDelta.y >= 1)
        {
            cam.orthographicSize++;

            if (cam.orthographicSize >= 25)
            {
                cam.orthographicSize = 25;
            }
        }
        else if (Input.mouseScrollDelta.y <= -1)
        {
            cam.orthographicSize--;

            if (cam.orthographicSize <= 5)
            {
                cam.orthographicSize = 5;
            }
        }
    }
    void MouseToPos()
    {
        Vector3 mousePos = Input.mousePosition;
        mousePos.z = 1.5f;
        mousePos = cam.ScreenToViewportPoint(mousePos);
        mousePos.z = 0f;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && dummyObject != null) // show building placement
        {
            Vector3 worldPos = new Vector3(hit.point.x, 0.4f, hit.point.z);
            worldPos = Vector3Int.FloorToInt(worldPos);     

            dummyObject.transform.position = Vector3Int.FloorToInt(worldPos);

            if (Input.GetMouseButtonDown(0))
            {
                worldPos = Vector3Int.FloorToInt(worldPos);
                //-PLACEMENT-----------
                if (building || tillingLand) PlaceObj((int)worldPos.x, (int)worldPos.z);
            }
        }
        else if (Physics.Raycast(ray, out hit))
        {
            //-SELECT-VILLAGER-----
            if (Input.GetMouseButtonDown(0) && building == false && tillingLand == false && VC == null)
            {
                if (hit.transform.tag == "villager")
                {
                    Debug.Log("villager");
                    VC = hit.transform.gameObject.GetComponent<VillagerController>();
                    VC.isSelected = true;
                }
            }
            else if(Input.GetMouseButtonDown(0) && building == false && tillingLand == false && VC != null)
            {
                if (hit.transform.tag == "villager")
                {
                    VC.isSelected = false;
                    VC = null;

                    VC = hit.transform.gameObject.GetComponent<VillagerController>();
                    VC.isSelected = true;
                }
                else
                {
                    VC.isSelected = false;
                    VC = null;
                }
            }
            
        }
    }

    void PlaceObj(int x, int z) // place building
    {
        if (PlacementCheck(x, z) && building)
        {
            Instantiate(dummyObject, new Vector3(x, 0.1f, z), transform.rotation);
            dummyObject.GetComponent<SpawnVillagers>().SpawnVillager();
            Destroy(dummyObject);
            dummyObject = null;

            building = false;
        }
        else if (FarmCheck(x, z) && tillingLand)
        {
            Instantiate(dummyObject, new Vector3(x, -1f, z), transform.rotation);
            GameObject grassToRemove = GameObject.Find("Grass: " + x + "," + z.ToString());
            Destroy(grassToRemove);
            Destroy(dummyObject);
            dummyObject = null;

            tillingLand = false;
        }
    }

    bool PlacementCheck(int x, int z)
    {
        if (randGen.GetSurroundingTileCount(x, z, worldMap) == 8 &&
        randGen.GetSurroundingTileCount(x, z, clutterMap) == 0)
        {
            return true;
        }
        return false;
    }
    bool FarmCheck(int x, int z)
    {
        if (worldMap[x,z] == 1 &&
        clutterMap[x, z] == 0)
        {
            return true;
        }
        return false;
    }

    //-BUILDING-SELECTION--------
    public void PlaceHouse()
    {
        dummyObject = Instantiate(house, transform.position, transform.rotation);
        building = true;
    }
    public void PlaceFarm()
    {
        dummyObject = Instantiate(farm, transform.position, transform.rotation);
        tillingLand = true;
    }
}
