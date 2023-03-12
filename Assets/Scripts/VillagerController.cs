using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    public Camera cam;
    public bool isSelected;
    public TempController controller;

    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera").GetComponent<Camera>();
        controller = GameObject.Find("Controller").GetComponent<TempController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
            Debug.Log("Right Click!");

            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.tag == "tree")
                {
                    // harvest
                }
                else if (hit.transform.tag == "rock")
                {
                    // mine
                }
                else if (hit.transform.tag == "farm")
                {
                    // farm
                }
                else if (hit.transform.tag == "ground")
                {
                    Debug.Log("ground hit");
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    
}
