using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsUI : MonoBehaviour
{
    public Dropdown resolutionDropdown;
    public GameUI ui;

    void Start()
    {
        // Initialize dropdown options (You can also do this in the Unity editor)
        List<string> options = new List<string> { "Fullscreen", "1920x1080", "3840x2160", "2560x1440", "1600x900", "1366x768", "1280x720" };
        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);

        // Add listener for resolution dropdown
        resolutionDropdown.onValueChanged.AddListener(delegate
        {
            DropdownValueChanged(resolutionDropdown);
        });
    }

    void Update()
    {

    }

    void DropdownValueChanged(Dropdown change)
    {
        // Get the selected value
        string selected = change.options[change.value].text;

        // Check for Fullscreen option
        if (selected == "Fullscreen")
        {
            Screen.fullScreen = true;
            return;
        }

        // Split to get width and height
        string[] dimensions = selected.Split('x');

        // Convert to integers
        int width = int.Parse(dimensions[0]);
        int height = int.Parse(dimensions[1]);

        // Set the screen resolution and turn off fullscreen
        Screen.SetResolution(width, height, false);
    }
}
