using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TempController : MonoBehaviour
{
    static TempController tempController;

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
    public GameObject bridge;
    [SerializeField] bool bridging;
    [SerializeField] bool buildingHouse;
    public GameObject farm;
    [SerializeField] bool tillingLand;

    [Header("Villager Control")]
    public VillagerController VC = null;

    // Start is called before the first frame update
    void Start()
    {
        cam = Camera.main;
        dummyObject = Instantiate(dummyObject, transform.position, transform.rotation);
        dummyObject.name = "TownHall";
        buildingHouse = true;        
    }

    // Update is called once per frame
    void Update()
    {
        worldMap = randGen.tileMap;
        clutterMap = randGen.resourceMap;

        // camera movement
        Movement();
        BuildingControl();
        CameraScroll();

        // raycast to curosr pos
        MouseToPos();

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Destroy(dummyObject);
            dummyObject = null;
            buildingHouse = false;
            tillingLand = false;
        }
    }

    void Movement()
    {
        verticalSpeed = Input.GetAxis("Vertical") * Time.deltaTime;
        transform.Translate(verticalSpeed * 8f, 0, horizontalSpeed);

        horizontalSpeed = Input.GetAxis("Horizontal") * Time.deltaTime;
        transform.Translate(verticalSpeed, 0, horizontalSpeed * -8f);

        if (Input.GetKeyDown(KeyCode.Q) && !DOTween.IsTweening(this.transform))
        {
            this.transform.DOLocalRotate(new Vector3(0, 90f, 0), 3f).SetRelative();
        }
        else if (Input.GetKeyDown(KeyCode.E) && !DOTween.IsTweening(this.transform))
        {
            this.transform.DORotate(new Vector3(0, -90f, 0), 3f).SetRelative();
        }
    }
    void CameraScroll()
    {
        if (Input.mouseScrollDelta.y >= 1)
        {
            cam.orthographicSize--;

            if (cam.orthographicSize >= 25)
            {
                cam.orthographicSize = 25;
            }
        }
        else if (Input.mouseScrollDelta.y <= -1)
        {
            cam.orthographicSize++;

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
            bool canPlace = PlacementCheck((int)worldPos.x, (int)worldPos.z);

            dummyObject.transform.position = Vector3Int.FloorToInt(worldPos);
            if (tillingLand || buildingHouse)
            {
                if (dummyObject.layer != (canPlace ? 9 : 10)) dummyObject.SetLayerRecursively(canPlace ? 9 : 10);
            }
            else if (bridging)
            {
                if (dummyObject.layer != (SingleTilePlacementCheckint((int)worldPos.x, (int)worldPos.z) ? 9 : 10)) dummyObject.SetLayerRecursively(canPlace ? 9 : 10);
            }


            if (Input.GetMouseButtonDown(0))
            {
                worldPos = Vector3Int.FloorToInt(worldPos);
                //-PLACEMENT-----------
                if (buildingHouse || tillingLand || bridging) PlaceObj((int)worldPos.x, (int)worldPos.z);
            }
        }
        else if (Physics.Raycast(ray, out hit)) // select character
        {
            //-SELECT-VILLAGER-----
            if (Input.GetMouseButtonDown(0) && buildingHouse == false && tillingLand == false && VC == null)
            {
                if (hit.transform.tag == "villager")
                {
                   // Debug.Log("villager");
                    VC = hit.transform.gameObject.GetComponent<VillagerController>();
                    VC.isSelected = true;
                }
            }
            else if(Input.GetMouseButtonDown(0) && buildingHouse == false && tillingLand == false && VC != null)
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

    void BuildingControl()
    {
        if (buildingHouse && Input.GetKeyDown(KeyCode.LeftBracket))
        {
            dummyObject.transform.Rotate(new Vector3(dummyObject.transform.localRotation.x,
                                                 dummyObject.transform.localRotation.y - 90,
                                                 dummyObject.transform.localRotation.z));
        }
        else if (buildingHouse && Input.GetKeyDown(KeyCode.RightBracket))
        {
            dummyObject.transform.Rotate(new Vector3(dummyObject.transform.localRotation.x,
                                                 dummyObject.transform.localRotation.y + 90,
                                                 dummyObject.transform.localRotation.z));
        }
    }

    void PlaceObj(int x, int z) // place building
    {
        if (PlacementCheck(x, z) && buildingHouse)
        {
            dummyObject.SetLayerRecursively(0);
            dummyObject.AddComponent<BoxCollider>();
            dummyObject.GetComponent<BoxCollider>().size = new Vector3(2, 2, 2);
            Instantiate(dummyObject, new Vector3(x, 0.1f, z), dummyObject.transform.rotation);
            if (dummyObject.name == "TownHall") dummyObject.GetComponent<SpawnVillagers>().SpawnVillager(); 
            
            Destroy(dummyObject);
            dummyObject = null;
            //randGen.SetOccupied(x, z);

            buildingHouse = false;
        }
        else if (PlacementCheck(x, z) && tillingLand)
        {
            dummyObject.SetLayerRecursively(0);
            for (int a = x - 1; a <= x + 1; a++)
            {
                for (int b = z - 1; b <= z + 1; b++)
                {
                    GameObject grassToRemove = GameObject.Find("Grass: " + a + "," + b.ToString());
                    Destroy(grassToRemove);
                }
            }

            var newObj = Instantiate(dummyObject, new Vector3(x, -0.5f, z), transform.rotation);

            Destroy(dummyObject);
            dummyObject = null;
            //randGen.SetOccupied(x, z);

            tillingLand = false;
        }
        else if (SingleTilePlacementCheckint(x,z) && bridging)
        {
            dummyObject.SetLayerRecursively(0);
            dummyObject.AddComponent<BoxCollider>();
            var newObj = Instantiate(dummyObject, new Vector3(x, -0.4f, z), dummyObject.transform.rotation);
            newObj.GetComponent<BoxCollider>().size = new Vector3(1, 0.4f, 1);
            newObj.layer = 11;
            newObj.transform.parent = GameObject.Find("LevelGen").transform;

            Destroy(dummyObject);
            dummyObject = null;

            bridging = false;
            randGen.UpdateNavMesh();
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
    bool SingleTilePlacementCheckint(int x, int z)
    {
        if (randGen.tileMap[x, z] == 0) return true;
        else return false;
    }

    //-BUILDING-SELECTION--------
    public void PlaceHouse()
    {
        dummyObject = Instantiate(house, transform.position, transform.rotation);
        buildingHouse = true;
    }
    public void PlaceFarm()
    {
        dummyObject = Instantiate(farm, transform.position, transform.rotation);
        tillingLand = true;
    }
    public void PlaceBridge()
    {
        dummyObject = Instantiate(bridge, transform.position, transform.rotation);
        bridging = true;
    }

    // ------------------SET-LAYER-RECURSIVLY-------------
}

public static class extensionMeathods
{
    public static void SetLayerRecursively(this GameObject obj, int newLayer)
    {
        if (null == obj)
            return;
        obj.layer = newLayer;
        foreach (Transform child in obj.transform)
        {
            if (null == child)
                continue;
            SetLayerRecursively(child.gameObject, newLayer);
        }
    }
}

