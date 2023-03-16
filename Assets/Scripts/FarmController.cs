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
    [SerializeField] float currentTime;
    [SerializeField] float totalTime = 60;

    public enum states
    {
        idle,
        growing,
        grown
    }
    public states state;

    // Start is called before the first frame update
    void Start()
    {
        state = states.idle;

        crops.SetActive(false);
        seedlings.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        switch (state)
        {
            case states.idle:

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
                }
                else
                {                    
                    seedlings.SetActive(false);
                    crops.SetActive(true);
                    currentTime = 0;
                    state = states.grown;
                }

                break;

            case states.grown:

                if (foodAmmount <= 0)
                {
                    crops.SetActive(false);
                    state = states.idle;
                }

                break;

            default:
                break;
        }
    }
}
