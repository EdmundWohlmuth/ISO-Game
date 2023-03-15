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


    // Start is called before the first frame update
    void Start()
    {
        currentResources = maxResources;
    }

    public void DecrimentResources(int decrimentValue)
    {
        if (currentResources <= (currentResources -= 20)) return;
        currentResources -= decrimentValue;
        Debug.Log(currentResources);

        if (currentResources <= 0)
        {
            gen.clutterMap[x, y] = 0;
            Destroy(gameObject);           
        }
    }
}
