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
    bool isDone;


    void Start()
    {
        progress = 0;
        isDone = false;
    }

    private void Update()
    {
        GameManager.gameManager.DisplayProgress(this.gameObject.transform.position, 1.75f, progress);

        if (progress == 1 && !isDone)
        {
            gameObject.GetComponent<SpawnVillagers>().SpawnVillager();
            isDone = true;
        }
        
    }
    public void DeliverResources(int addedResources)
    {
        if (progress > 1) return;

        currentResources += addedResources;
        progress = Mathf.InverseLerp(0, requiredWood, currentResources);
        float progressMade = currentResources / requiredWood;
        progress += progressMade;
        if (progress > 1) progress = 1;
        transform.localScale = new Vector3(1, progress, 1);
    }
}
