using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceNode : MonoBehaviour
{
    public enum resourceType
    {
        wood,
        stone,
        food
    }
    public resourceType resource;

    public int maxResources;
    public int currentResources;
    public RandomGen gen;
    public int x;
    public int y;
    public FarmController crops;


    // Start is called before the first frame update
    void Start()
    {
        currentResources = maxResources;
    }

    public void DecrimentResources(int decrimentValue)
    {
        //if (currentResources <= (currentResources -= 20)) return;

        currentResources -= decrimentValue;
        Debug.Log("resources left: " + currentResources);

        if (currentResources <= 0 && resource != resourceType.food)
        {
            gen.clutterMap[x, y] = 0;
            Destroy(gameObject);
        }
        else if ((currentResources <= 0 && resource == resourceType.food))
        {
            Debug.Log("farming done");
           crops.state = FarmController.states.idle;
            currentResources = maxResources;
        }
    }
}
