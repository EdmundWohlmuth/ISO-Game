using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using DG.Tweening;

public class TempController : MonoBehaviour
{
    static TempController tempController;

    [Header("Variables")]
    [SerializeField] float speed;
    [SerializeField] float fastSpeed;
    [SerializeField] float zoomSpeed;
    [SerializeField] float rotationSpeed;
    [SerializeField] float maxHeight = 75f;
    [SerializeField] float minHeight = 5f;
    public GameObject anchor;
    public Camera cam;
    //rotation values
    Quaternion newRotation;
    Vector3 rotateStartPos;

    [Header("tilemap")]
    [SerializeField] RandomGen randGen;
    int[,] worldMap;
    int[,] resourceMap;
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
        cam = gameObject.GetComponent<Camera>();
        newRotation = anchor.transform.localRotation;

        //cam = Camera.main;
        dummyObject = Instantiate(dummyObject, transform.position, transform.rotation);
        dummyObject.name = "TownHall";
        buildingHouse = true;        
    }

    // Update is called once per frame
    void Update()
    {
        worldMap = RandomGen.randomGen.tileMap;
        resourceMap = RandomGen.randomGen.resourceMap;

        // camera movement
        KeyboardControls();
        MouseControls();
        BuildingControl();


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

    void KeyboardControls()
    {
        // set speeds
        speed = gameObject.transform.position.y / 150; // faster when farther out, slower while closer
        if (Input.GetKey(KeyCode.LeftShift)) fastSpeed = 1.5f;
        else fastSpeed = 1f;

        // Movement
        float horizontal = speed * fastSpeed * Input.GetAxis("Horizontal");
        float vertical = speed * fastSpeed * Input.GetAxis("Vertical");
        float scroll = -zoomSpeed * fastSpeed * Input.mouseScrollDelta.y;

        Vector3 horizontalMove = horizontal * transform.right;
        Vector3 forwardMove = transform.forward;
        Vector3 verticalMove = new Vector3(0, scroll, 0);

        forwardMove.y = 0;
        forwardMove.Normalize();
        forwardMove *= vertical;

        Vector3 move = verticalMove + horizontalMove + forwardMove;
        anchor.transform.localPosition += move;

        // Camera Scroll
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
    void MouseControls() // Imperfect - Look into later
    {
        if (Input.GetMouseButtonDown(2))
        {
            rotateStartPos = Input.mousePosition;
        }
        else if (Input.GetMouseButton(2)) // else if avoids executing both conditions at the same time
        {
            Vector3 rotateCurrentPos = Input.mousePosition;
            Vector3 difference = rotateStartPos - rotateCurrentPos;
            rotateStartPos = rotateCurrentPos;

            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));

            anchor.transform.localRotation = newRotation;
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
                Debug.Log("Clicked at " + (int)worldPos.x + ", " + (int)worldPos.z);
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

            var newObj = Instantiate(dummyObject, new Vector3(x, -0.5f, z), transform.localRotation);

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
            RandomGen.randomGen.UpdateNavMesh();
        }
        RandomGen.randomGen.SetOccupied(x, z);
    }

    bool PlacementCheck(int x, int z)
    {
        if (RandomGen.randomGen.GetSurroundingTileCount(x, z, RandomGen.randomGen.tileMap) == 8 &&
        RandomGen.randomGen.GetSurroundingTileCount(x, z, RandomGen.randomGen.resourceMap) == 0)
        {
            return true;
        }
        return false;
    }
    bool SingleTilePlacementCheckint(int x, int z)
    {
        if (RandomGen.randomGen.tileMap[x, z] == 0) return true;
        else return false;
    }

    //-UI-BUILDING-SELECTION-----
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

