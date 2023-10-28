using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pause : MonoBehaviour
{
    private bool isPaused = false;  // Flag to track pause state

    // List to store MonoBehaviour components that will be paused
    private List<MonoBehaviour> pauseableComponents;

    private void Start()
    {
        // Initialize the list and populate it with all MonoBehaviour components in the object
        pauseableComponents = new List<MonoBehaviour>(GetComponents<MonoBehaviour>());
    }

    public void PauseObj()
    {
        if (!isPaused)
        {
            isPaused = true;

            // Disable the components
            foreach (var component in pauseableComponents)
            {
                component.enabled = false;
            }
        }
    }

    public void UnPauseObj()
    {
        if (isPaused)
        {
            isPaused = false;

            // Enable the components
            foreach (var component in pauseableComponents)
            {
                component.enabled = true;
            }
        }
    }
}
