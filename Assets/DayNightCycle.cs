using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public Vector3 RotationVector;
    public float time;
    void Start()
    {
        
    }

    void Update()
    {
        time += Time.deltaTime;
        if(time > 24)
        {
            time = 0;
        }
        transform.Rotate(RotationVector * Time.deltaTime);
    }
}
