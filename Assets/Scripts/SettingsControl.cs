using UnityEngine;
using UnityEngine.UI;

public class SettingsControl : MonoBehaviour
{
    // Sliders
    public Slider volumeSlider;
    public Slider waterHeightSlider;
    public Slider creatureLODSlider;
    public Slider numGenesSlider;
    public Slider creatureFoodDecreaseSlider;
    public Slider creatureSpeedMultiplierSlider;
    public Slider predatorFoodGainMultiplierSlider;
    public Slider foodLossMultiplierSlider;
    public Slider refractoryPeriodSlider;
    public Toggle showCreatureDetailsToggle;
    public GameManager manager; // Reference to your GameManager
    public Toggle segregationToggle;
    public Slider creatureDigestionDurationSlider;
    public Slider plantEnergyGainMultiplierSlider;
    public Slider seedRangeMultiplierSlider;
    void Start()
    {
        // Initialize sliders and labels with current settings
        InitializeSlider(volumeSlider, manager.settings.volume, "Volume");
        InitializeSlider(waterHeightSlider, manager.settings.waterHeight, "Water Height");
        InitializeSlider(creatureLODSlider, manager.settings.CreatureLOD, "Creature LOD");
        InitializeSlider(numGenesSlider, manager.settings.NumGenes, "Number of Genes");
        InitializeSlider(creatureFoodDecreaseSlider, manager.settings.CreatureFoodDecrease, "Creature Wander Food Decrease Multiplier");
        InitializeSlider(creatureSpeedMultiplierSlider, manager.settings.CreatureSpeedMultiplier, "Creature Speed Multiplier");
        InitializeSlider(predatorFoodGainMultiplierSlider, manager.settings.PredatorFoodGainMultiplier, "Predator Food Gain Multiplier");
        InitializeSlider(foodLossMultiplierSlider, manager.settings.FoodLossMultiplier, "Creature Eat Food Energy Loss Multiplier");
        InitializeSlider(refractoryPeriodSlider, manager.settings.refractoryPeriod, "Refractory Period");
        // Initialize toggle
        InitializeToggle(showCreatureDetailsToggle, manager.settings.showCreatureDetails, "Show Creature Details");

        //InitializeToggle(segregationToggle, manager.settings.segregation, "Segregation");
        InitializeSlider(creatureDigestionDurationSlider, manager.settings.creatureDigestionDuration, "Creature Digestion Duration");
        InitializeSlider(plantEnergyGainMultiplierSlider, manager.settings.plantEnergyGainMultiplier, "Plant Energy Gain Multiplier");
        InitializeSlider(seedRangeMultiplierSlider, manager.settings.seedRangeMultiplier, "Seed Range Multiplier");

        // Add listeners for new sliders
        creatureDigestionDurationSlider.onValueChanged.AddListener(delegate { manager.settings.creatureDigestionDuration = creatureDigestionDurationSlider.value; });
        plantEnergyGainMultiplierSlider.onValueChanged.AddListener(delegate { manager.settings.plantEnergyGainMultiplier = plantEnergyGainMultiplierSlider.value; });
        seedRangeMultiplierSlider.onValueChanged.AddListener(delegate { manager.settings.seedRangeMultiplier = seedRangeMultiplierSlider.value; }); volumeSlider.onValueChanged.AddListener(delegate { manager.settings.volume = volumeSlider.value; });

        segregationToggle.onValueChanged.AddListener(delegate { manager.settings.segregation = segregationToggle.isOn; });

        volumeSlider.onValueChanged.AddListener(delegate { manager.settings.volume = volumeSlider.value; });
        waterHeightSlider.onValueChanged.AddListener(delegate { manager.settings.waterHeight = waterHeightSlider.value; });
        creatureLODSlider.onValueChanged.AddListener(delegate { manager.settings.CreatureLOD = (int)creatureLODSlider.value; });
        numGenesSlider.onValueChanged.AddListener(delegate { manager.settings.NumGenes = (int)numGenesSlider.value; });
        creatureFoodDecreaseSlider.onValueChanged.AddListener(delegate { manager.settings.CreatureFoodDecrease = creatureFoodDecreaseSlider.value; });
        creatureSpeedMultiplierSlider.onValueChanged.AddListener(delegate { manager.settings.CreatureSpeedMultiplier = creatureSpeedMultiplierSlider.value; });
        predatorFoodGainMultiplierSlider.onValueChanged.AddListener(delegate { manager.settings.PredatorFoodGainMultiplier = predatorFoodGainMultiplierSlider.value; });
        foodLossMultiplierSlider.onValueChanged.AddListener(delegate { manager.settings.FoodLossMultiplier = foodLossMultiplierSlider.value; });
        refractoryPeriodSlider.onValueChanged.AddListener(delegate { manager.settings.refractoryPeriod = refractoryPeriodSlider.value; });

    }

    void InitializeSlider(Slider slider, float initialValue, string settingName)
    {
        slider.value = initialValue;

        // Find the Text component that is a child of this Slider and is named "Label"
        Text label = slider.transform.Find("Label").GetComponent<Text>();
        UpdateLabel(label, settingName, initialValue);

        // Add listener
        slider.onValueChanged.AddListener(delegate
        {
            float newValue = slider.value;
            UpdateLabel(label, settingName, newValue);

            // Update the manager.settings here
            // For example: manager.settings.someValue = newValue;
        });
    }
    void InitializeToggle(Toggle toggle, bool initialValue, string settingName)
    {
        toggle.isOn = initialValue;

        // Add listener
        toggle.onValueChanged.AddListener(delegate
        {
            bool newValue = toggle.isOn;
            // Update the manager.settings here
            manager.settings.showCreatureDetails = newValue;
        });

    }
    void UpdateLabel(Text label, string settingName, float value)
    {
        label.text = $"{settingName}: {value}";
    }
}
