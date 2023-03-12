using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnVillagers : MonoBehaviour
{
    public GameObject villager;
    public int ammount;
    public Camera cam;
    public TempController player;

    public void SpawnVillager()
    {
        for (int i = 0; i <= ammount; i++)
        {
            Instantiate(villager);
            villager.transform.position = new Vector3(gameObject.transform.position.x + i, 1, gameObject.transform.position.z - 2);
            villager.GetComponent<VillagerController>().cam = cam;
            villager.GetComponent<VillagerController>().controller = player;
        }
        
    }
}
