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
    public bool isDone;


    void Start()
    {
        progress = 0;
        isDone = false;
    }

    private void Update()
    {
        if (!isDone) GameManager.gameManager.DisplayProgress(this.gameObject.transform.position, 1.75f, progress);

        if (progress == 1 && !isDone)
        {
            if (gameObject.GetComponent<SpawnVillagers>()) gameObject.GetComponent<SpawnVillagers>().SpawnVillager();
            else if (gameObject.GetComponent<FarmController>())
            {
                gameObject.transform.position = new Vector3(gameObject.transform.position.x, -1f, gameObject.transform.position.z);
                gameObject.gameObject.GetComponent<FarmController>().state = FarmController.states.idle;
                gameObject.gameObject.GetComponent<FarmController>().seeded = true;
            }
            

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
