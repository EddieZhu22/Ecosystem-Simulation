using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float year;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        year += Time.deltaTime;
    }
}
