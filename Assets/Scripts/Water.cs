using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour
{
    [SerializeField] private GameManager manager;
    [SerializeField] private StylizedWater.StylizedWaterURP waterSettings;
    [SerializeField] private float yint = 0, intensitymultiplier;
    [SerializeField] private Vector2 initialDeepWaterColor, initialShallowWaterColor;

    void Start()
    {
        initialShallowWaterColor = new Vector2(waterSettings.shallowColor.g,waterSettings.shallowColor.b);
        initialDeepWaterColor = new Vector2(waterSettings.deepColor.g,waterSettings.deepColor.b);
    }
    void FixedUpdate()
    {
        transform.position = new Vector3(0,height(),0);
        waterSettings.shallowColor = waterColor(initialShallowWaterColor);
        waterSettings.deepColor = waterColor(initialDeepWaterColor);
        waterSettings.WriteMaterialProperties();
    }
    private Color waterColor(Vector2 initialColor)
    {
        float colorValG = (( -((manager.tManager.time - 6) * (manager.tManager.time - 18))/75) + initialColor.x *yint* intensitymultiplier) ;
        float colorValB = (( -((manager.tManager.time - 6) * (manager.tManager.time - 18))/75) + initialColor.y *yint* intensitymultiplier) ;
        if(manager.tManager.time < 6 || manager.tManager.time > 18)
        {
            return new Color(0, yint * initialColor.x * intensitymultiplier, yint * initialColor.y * intensitymultiplier);
        }
        return new Color(0, colorValG, colorValB );
    }
    private float height() => manager.settings.waterHeight; 

}
