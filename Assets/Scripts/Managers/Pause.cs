using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    public void PauseObj()
    {
        if(gameObject.tag == "creature")
        {
            //set scripts all false! (add more as more scripts are added)
            GetComponent<CreatureBehavior>().enabled = false;
            GetComponent<Brain>().enabled = false;
        }
        if(gameObject.tag == "collectible")
        {
            //set scripts all false! (add more as more scripts are added)
            GetComponent<Collectible>().enabled = false;
        }
        if(gameObject.tag == "manager")
        {
            GetComponent<ItemSpawner>().enabled = false;
        }
    }
    public void UnPauseObj()
    {
        if(gameObject.tag == "creature")
        {
            //set scripts all true! (add more as more scripts are added)
            GetComponent<CreatureBehavior>().enabled = true;
            GetComponent<Brain>().enabled = true;
        }
        else if(gameObject.tag == "food")
        {
            //set scripts all true! (add more as more scripts are added)
            GetComponent<Collectible>().enabled = true;
        }
        if(gameObject.tag == "manager")
        {
            GetComponent<ItemSpawner>().enabled = true;
        }
    }
}
