using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private GameManager manager;
    void Update()
    {
        transform.position = new Vector3(0,height(),0);
    }
    private float height()
    {
        return manager.settings.waterHeight; 
    }
}
