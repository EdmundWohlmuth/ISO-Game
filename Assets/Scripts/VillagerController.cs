using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerController : MonoBehaviour
{
    [Header("Refrences")]
    [SerializeField] NavMeshAgent agent;
    public Camera cam;   
    public TempController controller;
    [SerializeField] GameObject currentNode;
    public GameObject townHall;
    public GameManager GM;
    public enum currentResource
    {
        none,
        wood,
        stone,
        food
    }

    public enum playerState
    {
        walking,
        chopingWood,
        miningStone,
        harvestingCrops,
        dropSupplies,
        planting
    }
    [Header("states")]
    public playerState state;
    public currentResource resource;
    public bool isSelected;

    [Header("Resource colletion")]
    [SerializeField] int harvestValue = 2;
    [SerializeField] int carryCapacity = 20;
    [SerializeField] float harvestTime = 5;
    float maxHarvestTime = 5;
    [SerializeField] int collected;
    int woodCollectionAmmount = 5;
    int stoneCollectionAmmount = 2;
    int foodCollectionAmmount = 10;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Value: " + harvestValue);
        harvestTime = maxHarvestTime;
        state = playerState.walking;
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GameObject.Find("Controller").GetComponent<TempController>();
        GM = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Update is called once per frame
    void Update()
    {
        PlayerInput();
        StateController();
    }

    void PlayerInput()
    {
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            //Debug.Log("Right Click!");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "tree" && collected == 0 || hit.transform.tag == "tree" && resource == currentResource.wood)
                {
                    //Debug.Log("chop");
                    // harvest
                    agent.SetDestination(hit.transform.position);
                    state = playerState.chopingWood;
                    resource = currentResource.wood;
                    harvestValue = woodCollectionAmmount;
                    Debug.Log("Value: " + harvestValue);
                }
                else if (hit.transform.tag == "rock" && collected == 0 || hit.transform.tag == "rock" && resource == currentResource.stone)
                {
                    //Debug.Log("mine");
                    agent.SetDestination(hit.transform.position);
                    state = playerState.miningStone;
                    resource = currentResource.stone;
                    harvestValue = stoneCollectionAmmount;
                    // mine
                }
                else if (hit.transform.tag == "farm" && collected == 0 || hit.transform.tag == "farm" && state == playerState.planting)
                {
                    //Debug.Log("plant");
                    agent.SetDestination(hit.transform.position);
                    state = playerState.planting;
                }
                else if (hit.transform.tag == "food" && collected == 0 || hit.transform.tag == "food" && resource == currentResource.food)
                {
                    //Debug.Log("farm");
                    agent.SetDestination(hit.transform.position);
                    state = playerState.harvestingCrops;
                    resource = currentResource.food;
                    harvestValue = foodCollectionAmmount;
                    // farm
                }
                else if (hit.transform.tag == "townhall")
                {
                    //Debug.Log("return");
                    Return();
                }
                else if (hit.transform.tag == "ground")
                {
                    //Debug.Log("moving");
                    agent.SetDestination(hit.point);
                    state = playerState.walking;
                }

                if (state != playerState.walking)
                {
                    currentNode = hit.transform.gameObject;
                }
                else currentNode = null;

            }
        }
    }

    void StateController()
    {
        switch (state)
        {
            case playerState.walking:

                break;

            case playerState.chopingWood:

                if (currentNode == null) return;
                else if (Vector3.Distance(gameObject.transform.position, currentNode.transform.position) <= 1)
                {
                    CollectionTimer();
                }

                if (collected == carryCapacity)
                {
                    Return();
                }
                break;

            case playerState.miningStone:

                if (currentNode == null) return;
                else if (Vector3.Distance(gameObject.transform.position, currentNode.transform.position) <= 1)
                {
                    CollectionTimer();
                }

                if (collected == carryCapacity)
                {
                    Return();
                }
                break;

            case playerState.harvestingCrops:
                if (currentNode == null) return;

                //Debug.Log(Vector3.Distance(gameObject.transform.position, currentNode.transform.position));
                if (Vector3.Distance(gameObject.transform.position, currentNode.transform.position) <= 1.4)
                {
                    CollectionTimer();
                }

                if (collected == carryCapacity)
                {
                    Return();
                }

                break;

            case playerState.planting:

                if (currentNode.GetComponent<FarmController>().state == FarmController.states.idle)
                {
                    //Debug.Log("idle");
                    if (Vector3.Distance(gameObject.transform.position, currentNode.transform.position) <= 1.4)
                    {
                        //Debug.Log("farm");
                        currentNode.GetComponent<FarmController>().seeded = true;
                    }
                }

                break;
        }
    }

    void Harvest()
    {
        if (collected < carryCapacity)
        {
           // Debug.Log("Value: " + harvestValue);
            currentNode.GetComponent<ResourceNode>().DecrimentResources(harvestValue);
            collected += harvestValue;
        } 
        else if (collected >= carryCapacity || currentNode == null)
        {
            collected = carryCapacity;
            Return();
        }
    }
    void Return()
    {
        //Debug.Log("return");
        agent.SetDestination(townHall.transform.position);
        //Debug.Log("Distance: " + Vector3.Distance(gameObject.transform.position, townHall.transform.position));
        if (Vector3.Distance(gameObject.transform.position, townHall.transform.position) <= 2.35f)
        {         
            switch (resource)
            {
                // add to stockpile
                case currentResource.none:
                    break;

                case currentResource.wood:
                    //Debug.Log("Deposited wood");
                    GM.woodPool += collected;
                    collected = 0;
                    
                    break;

                case currentResource.stone:
                    break;

                case currentResource.food:
                    GM.foodPool += collected;
                    collected = 0;
                    break;

                default:
                    break;
            }
            agent.SetDestination(currentNode.transform.position);
        }
    }
    void CollectionTimer()
    {
        if (harvestTime <= 0)
        {
            Harvest();
            harvestTime = maxHarvestTime;
        }
        else
        {
            harvestTime -= Time.deltaTime;
        }
    }

    bool GenaricTimer()
    {
        if (harvestTime <= 0)
        {           
            harvestTime = maxHarvestTime;
            return true;
        }
        else
        {
            harvestTime -= Time.deltaTime;
            return false;
        }
    }

}
