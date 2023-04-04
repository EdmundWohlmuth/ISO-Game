using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreationManager : MonoBehaviour
{
    // Start is called before the first frame update
    public Renderer refrence;
    public int currentResources;
    public int requiredWood;
    public int requiredStone;
    public float progress;


    void Start()
    {
        progress = 0;
    }

    void Progress()
    {
        if (currentResources == requiredWood)
        {
            transform.localScale = new Vector3(1, 1, 1);
        }
        //Debug.Log("Progress = " + progress);
        
    }
    public void DeliverResources(int addedResources)
    {
        currentResources += addedResources;        
        float progressMade = currentResources / requiredWood;
        progress += progressMade; 

        Progress();
    }
}
