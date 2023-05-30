using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FarmController : MonoBehaviour
{
    [Header("GameObjects")]
    [SerializeField] GameObject crops;
    [SerializeField] GameObject seedlings;

    public bool seeded;
    public bool harvested;
    public int foodAmmount;
    int totalFood = 60;
    public ResourceNode node;
    [SerializeField] float currentTime;
    [SerializeField] float totalTime = 60;

    public enum states
    {
        idle,
        growing,
        grown,
        disabled
    }
    public states state;

    // Start is called before the first frame update
    void Start()
    {
        node = GetComponentInChildren<ResourceNode>();
        state = states.disabled;

        crops.SetActive(false);
        seedlings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case states.idle:

                crops.SetActive(false);

                if (seeded)
                {
                    seedlings.SetActive(true);
                    state = states.growing;
                    seeded = false;
                }

                break;

            case states.growing:

                if (currentTime <= totalTime)
                {
                    currentTime += Time.deltaTime;
                    GameManager.gameManager.DisplayProgress(gameObject.transform.position, 1.5f, Mathf.InverseLerp(totalTime, 0, currentTime));
                }
                else
                {                    
                    seedlings.SetActive(false);
                    crops.SetActive(true);
                    node = GetComponentInChildren<ResourceNode>();
                    currentTime = 0;
                    node.currentResources = totalFood;
                    state = states.grown;
                }

                break;

            case states.grown:
                if (node.currentResources == 0)
                {
                    state = states.idle;
                }
                break;

            case states.disabled:
                break;

            default:
                break;
        }
    }
}
