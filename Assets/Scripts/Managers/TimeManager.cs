using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float day;
    public float time;
    public float accuracy;
    void Start()
    {
        time = 6;
    }

    void Update()
    {
        if (time > 24)
            time = 0;

        time += (Time.deltaTime) * 24 / 60;
        day += 1 * Time.deltaTime / (60);
    }
}
