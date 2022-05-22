using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float day;
    public float time;
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        time += (Time.deltaTime) * 24 / 60;
        day += 1 * Time.deltaTime / (60);
    }
}
