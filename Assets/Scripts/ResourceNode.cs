using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

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
        this.gameObject.transform.DOShakeRotation(0.2f, 5f, 1, 90);

        //Debug.Log("resources left: " + currentResources);

        if (currentResources <= 0 && resource != resourceType.food)
        {
            RandomGen.randomGen.UpdateResourceMapData(x, y);
            Debug.Log("Destroyng " + gameObject.name + " at " + x + ", " + y);
            Debug.Log("State of tile " + gameObject.name + ": " + RandomGen.randomGen.resourceMap[x, y]);
            Destroy(gameObject);
        }
        else if ((currentResources <= 0 && resource == resourceType.food))
        {
            Debug.Log("farming done");
            crops.state = FarmController.states.idle;
        }
    }
}
