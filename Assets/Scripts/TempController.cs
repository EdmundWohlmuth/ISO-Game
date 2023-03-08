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
    bool building;
    public GameObject farm;
    bool tillingLand;

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
        mousePos.z = 0f; // set the z-coordinate to 0, since we're working with an orthographic camera
        //Debug.DrawRay(transform.position, mousePos - transform.position, Color.blue);

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

                //Debug.Log(hit.point);
                //Debug.Log("Mouse position in world space: " + worldPos);
                //Debug.Log("ground: " + randGen.GetSurroundingTileCount((int)worldPos.x, (int)worldPos.z, worldMap));
                //Debug.Log("trees: " + randGen.GetSurroundingTileCount((int)worldPos.x, (int)worldPos.z, clutterMap));

                if (PlacementCheck((int)worldPos.x, (int)worldPos.z) && building)
                {

                    Instantiate(dummyObject, new Vector3(worldPos.x, 0.1f, worldPos.z), transform.rotation);
                    Destroy(dummyObject);
                    dummyObject = null;

                    building = false;
                }
                else if (FarmCheck((int)worldPos.x, (int)worldPos.z) && tillingLand)
                {
                    Instantiate(dummyObject, new Vector3(worldPos.x, -1f, worldPos.z), transform.rotation);
                    GameObject grassToRemove = GameObject.Find("Grass: " + worldPos.x + "," + worldPos.z.ToString());
                    Destroy(grassToRemove);
                    Destroy(dummyObject);
                    dummyObject = null;

                    tillingLand = false;
                }

                // set position to unplaceable;
            }
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

    //-BUILDING---------
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
