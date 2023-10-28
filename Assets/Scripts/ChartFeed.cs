using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;

public class ChartFeed : MonoBehaviour
{

    public List<double> yData1;
    public List<double> yData2;
    public List<double> xData;


    // is y value previous
    public bool prev1, prev2;
    // Start is called before the first frame update
    public bool isDataPageActive = true; // Set this to true when the data page is active and false otherwise

    private void OnEnable()
    {
        StartCoroutine(UpdateChart());
    }

    IEnumerator UpdateChart()
    {
        print(prev1);
        if (isDataPageActive)
        {
            if (prev1 == false)
            {
                for (int i = 0; i < yData1.Count; i++)
                {
                }
            }
            if (prev1 == true)
            {

            }

            if (prev2 == false)
            {
                for (int i = 0; i < yData2.Count; i++)
                {
                }
            }
            if (prev2 == true)
            {
            }

        }
        yield return new WaitForSeconds(60f);
        StartCoroutine(UpdateChart());
    }
    // Update is called once per frame
    void Update()
    {

    }
}
