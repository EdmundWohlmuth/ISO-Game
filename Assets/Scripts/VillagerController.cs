using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class VillagerController : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] Camera cam;
    public bool isSelected;
    [SerializeField] TempController controller;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(1) && isSelected)
        {
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
                    agent.SetDestination(hit.point);
                }
            }
        }
    }

    
}
