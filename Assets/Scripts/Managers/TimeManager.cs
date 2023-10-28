using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{
    public float day;
    public float time;
    public float accuracy;
    public DayNightCycle cycle;

    public int days, hours, minutes, years;


    void Start()
    {
        time = 6;
    }

    void Update()
    {
        timeCount();
    }
    private void timeCount()
    {
        if (time > 24)
            time = 0;
        if (day > 365)
        {
            day = 0;
            years ++;
        }
        time += (Time.deltaTime) * 24.0f / (60.0f * cycle.cycleInMinutes);
        day += 1.0f * Time.deltaTime / (60.0f * cycle.cycleInMinutes);
        convertToMinutes();
    }

    private void convertToMinutes()
    {
        hours = Mathf.FloorToInt(time);
        minutes = Mathf.FloorToInt((time - hours) * 60f);
        days = Mathf.FloorToInt(day);
    }
}
