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

    private void Update()
    {
        GameManager.gameManager.DisplayPorgress(transform.position, 0.75f, progress);
    }
    public void DeliverResources(int addedResources)
    {
        

        currentResources += addedResources;
        progress = Mathf.InverseLerp(0, requiredWood, currentResources);
        float progressMade = currentResources / requiredWood;
        progress += progressMade;
        transform.localScale = new Vector3(1, progress, 1);


    }
}
