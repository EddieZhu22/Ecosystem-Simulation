using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DNC_StreetLights : MonoBehaviour
{
    public void SwitchStreetLights(bool enabled)
    {
        foreach(Light light in GetComponentsInChildren<Light>())
        {
            light.enabled = enabled;
        }

        foreach(Renderer renderer in GetComponentsInChildren<Renderer>())
        {
            foreach(Material material in renderer.materials)
            {
                if(enabled) 
                {
                    material.EnableKeyword("_EMISSION");
                }
                else 
                { 
                    material.DisableKeyword("_EMISSION");
                }
            }
        }
    }
}
