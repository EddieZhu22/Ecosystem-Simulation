using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.IO;
using System.Collections;
using XCharts.Runtime;
using System.Linq;
using System.Text;
using TMPro;

public class DataManager : MonoBehaviour
{
    public GameManager manager;
    List<List<double>> dataList = new List<List<double>>();
    List<List<double>> detailedDataList_predator = new List<List<double>>();
    List<List<double>> detailedDataList_prey = new List<List<double>>();
    List<List<double>> detailedDataList_plants = new List<List<double>>();

    public List<string> dataNames = new List<string>();

    public double currentTime = 0;
    public double currentPlantCount = 0;
    public double currentHerbivoreCount = 0;
    public double currentPredatorCount = 0;

    public int y1Value, y2Value;
    public Dropdown y1DropDown, y2DropDown;
    public Dropdown y1DropDown_prey, y2DropDown_prey, y1DropDown_predator, y2DropDown_predator, y1DropDown_plant, y2DropDown_plant;
    public LineChart chart;

    public List<Creature> Predator;
    public List<Creature> Prey;
    public List<Plant> Plants;

    private int prev1, prev2;
    private bool enterChart;
    public TextMeshProUGUI infoText;

    void Start()
    {
        for (int i = 0; i < 100; i++)
        {
            dataList.Add(new List<double>());
        }

        for (int i = 0; i < 50; i++)
        {
            detailedDataList_predator.Add(new List<double>());
            detailedDataList_prey.Add(new List<double>());
            detailedDataList_plants.Add(new List<double>());
        }

        var tooltip = chart.EnsureChartComponent<Tooltip>();
        tooltip.trigger = Tooltip.Trigger.Axis;

        var xAxis = chart.EnsureChartComponent<XAxis>();
        var yAxis = chart.EnsureChartComponent<YAxis>();
        xAxis.splitNumber = 10;
        xAxis.boundaryGap = true;
        xAxis.show = true;
        yAxis.show = true;
        CalculateAverageAttributes();
        ChartData();
        StartCoroutine(AddData());
        PlayerPrefs.SetInt("runNum", PlayerPrefs.GetInt("runNum", 0)+1);

    }
    private bool hasInitializedDropdowns = false;

    // Update is called once per frame
    void Update()
    {
        if (manager.UI.Page == GameUI.PageView.Data)
        {
            SetActiveDropdown(y1DropDown.value, y1DropDown_prey, y1DropDown_predator, y1DropDown_plant);
            SetActiveDropdown(y2DropDown.value, y2DropDown_prey, y2DropDown_predator, y2DropDown_plant);
        }
        else
        {
            y1DropDown_prey.gameObject.SetActive(false);
            y1DropDown_predator.gameObject.SetActive(false);
            y1DropDown_plant.gameObject.SetActive(false);
            y2DropDown_prey.gameObject.SetActive(false);
            y2DropDown_predator.gameObject.SetActive(false);
            y2DropDown_plant.gameObject.SetActive(false);
        }
    }

    void SetActiveDropdown(int value, Dropdown prey, Dropdown predator, Dropdown plant)
    {
        prey.gameObject.SetActive(value == 0);
        predator.gameObject.SetActive(value == 1);
        plant.gameObject.SetActive(value == 2);
    }

    IEnumerator AddData()
    {
        dataList[0].Add(currentTime);
        //dataList[1].Add(currentHerbivoreCount);
        //dataList[2].Add(currentPredatorCount);
        //dataList[3].Add(currentPlantCount);

        CalculateAverageAttributes();
        ChartData();


        yield return new WaitForSeconds(5f);
        StartCoroutine(AddData());
    }
    void ChartData()
    {
        int y1DropIndex = y1DropDown.value + 1;
        int y2DropIndex = y2DropDown.value + 1;

        int y1DropCounter = (y1DropDown.value << 24) | (y1DropDown_prey.value << 16) | (y1DropDown_predator.value << 8) | y1DropDown_plant.value;
        int y2DropCounter = (y2DropDown.value << 24) | (y2DropDown_prey.value << 16) | (y2DropDown_predator.value << 8) | y2DropDown_plant.value;

        var title = chart.EnsureChartComponent<XCharts.Runtime.Title>();

        if (manager.UI.Page == GameUI.PageView.Data)
        {

            List<double> currList = new List<double>();
            List<double> currList2 = new List<double>();

            if (prev1 != y1DropCounter || prev2 != y2DropCounter || enterChart == false)
            {
                enterChart = true;
                chart.ClearData();
                chart.ClearSerieData();

                int i = 0;
                if (y1DropIndex == 1)
                {
                    foreach (var data in detailedDataList_prey[y1DropDown_prey.value])
                    {
                        chart.AddXAxisData(i.ToString());
                        chart.AddData(0, data);
                        i++;
                    }
                    currList = detailedDataList_prey[y1DropDown_prey.value];
                }
                else if (y1DropIndex == 2)
                {
                    foreach (var data in detailedDataList_predator[y1DropDown_predator.value])
                    {
                        chart.AddXAxisData(i.ToString());
                        chart.AddData(0, data);
                        i++;
                    }
                    currList = detailedDataList_predator[y1DropDown_predator.value];
                }
                else if (y1DropIndex == 3)
                {
                    foreach (var data in detailedDataList_plants[y1DropDown_plant.value])
                    {
                        chart.AddXAxisData(i.ToString());
                        chart.AddData(0, data);
                        i++;
                    }
                    currList = detailedDataList_plants[y1DropDown_plant.value];
                }
                currList2 = currList;
                if (y1DropCounter != y2DropCounter)
                {
                    if (y2DropIndex == 1)
                    {
                        foreach (var data in detailedDataList_prey[y2DropDown_prey.value])
                        {
                            chart.AddData(1, data);
                        }
                        currList2 = detailedDataList_prey[y2DropDown_prey.value];
                    }
                    if (y2DropIndex == 2)
                    {
                        foreach (var data in detailedDataList_predator[y2DropDown_predator.value])
                        {
                            chart.AddData(1, data);
                        }
                        currList2 = detailedDataList_predator[y2DropDown_predator.value];
                    }
                    if (y2DropIndex == 3)
                    {
                        foreach (var data in detailedDataList_plants[y2DropDown_predator.value])
                        {
                            chart.AddData(1, data);
                        }
                        currList2 = detailedDataList_plants[y2DropDown_plant.value];
                    }
                }

            }
            else
            {
                if (y1DropCounter == y2DropCounter)
                {
                    string name1 = "";
                    chart.AddXAxisData(currentTime.ToString());


                    if (y1DropIndex == 1)
                    {
                        chart.AddData(0, detailedDataList_prey[y1DropDown_prey.value][detailedDataList_prey[y1DropDown_prey.value].Count - 1]);
                        name1 = y1DropDown_prey.options[y1DropDown_prey.value].text;
                    }
                    if (y1DropIndex == 2)
                    {
                        chart.AddData(0, detailedDataList_predator[y1DropDown_predator.value][detailedDataList_predator[y1DropDown_predator.value].Count - 1]);
                        name1 = y1DropDown_predator.options[y1DropDown_predator.value].text;
                    }
                    if (y1DropIndex == 3)
                    {
                        chart.AddData(0, detailedDataList_plants[y1DropDown_plant.value][detailedDataList_plants[y1DropDown_plant.value].Count - 1]);
                        name1 = y1DropDown_plant.options[y1DropDown_plant.value].text;
                    }


                    title.text = dataNames[y1DropIndex] + " " + name1 + " vs. Time";
                }
                else
                {
                    chart.AddXAxisData(currentTime.ToString());
                    string name1 = "";
                    string name2 = "";


                    if (y1DropIndex == 1)
                    {
                        chart.AddData(0, detailedDataList_prey[y1DropDown_prey.value][detailedDataList_prey[y1DropDown_prey.value].Count - 1]);
                        name1 = y1DropDown_prey.options[y1DropDown_prey.value].text;
                    }
                    if (y1DropIndex == 2)
                    {
                        chart.AddData(0, detailedDataList_predator[y1DropDown_predator.value][detailedDataList_predator[y1DropDown_predator.value].Count - 1]);
                        name1 = y1DropDown_predator.options[y1DropDown_predator.value].text;
                    }
                    if (y1DropIndex == 3)
                    {
                        chart.AddData(0, detailedDataList_plants[y1DropDown_plant.value][detailedDataList_plants[y1DropDown_plant.value].Count - 1]);
                        name1 = y1DropDown_plant.options[y1DropDown_plant.value].text;
                    }
                    if (y2DropIndex == 1)
                    {
                        chart.AddData(1, detailedDataList_prey[y2DropDown_prey.value][detailedDataList_prey[y2DropDown_prey.value].Count - 1]);
                        name2 = y2DropDown_prey.options[y2DropDown_prey.value].text;
                    }
                    if (y2DropIndex == 2)
                    {
                        chart.AddData(1, detailedDataList_predator[y2DropDown_predator.value][detailedDataList_predator[y2DropDown_predator.value].Count - 1]);
                        name2 = y2DropDown_predator.options[y2DropDown_predator.value].text;
                    }
                    if (y2DropIndex == 3) // Assuming 3 corresponds to plants
                    {
                        chart.AddData(1, detailedDataList_plants[y2DropDown_plant.value][detailedDataList_plants[y2DropDown_plant.value].Count - 1]);
                        name2 = y2DropDown_plant.options[y2DropDown_plant.value].text;
                    }
                    title.text = dataNames[y1DropIndex] + " " + name1 + " and " + dataNames[y2DropIndex] + " " + name2 + " vs. Time";
                }
            }
        }
        else
        {
            enterChart = false;
        }

        prev1 = y1DropCounter;
        prev2 = y2DropCounter;
        currentTime++;
    }
    public void CalculateAverageAttributes()
    {
        RemoveNullValues();
        Dictionary<string, double> sumAttributesPredator = new Dictionary<string, double>();
        Dictionary<string, double> sumAttributesPrey = new Dictionary<string, double>();

        int countPredator = 0;
        int countPrey = 0;

        foreach (var creature in Predator)
        {
            if (creature != null && creature.genes != null)
            {
                countPredator++;
                foreach (var entry in creature.genes.Genes)
                {
                    if (sumAttributesPredator.ContainsKey(entry.Key))
                    {
                        sumAttributesPredator[entry.Key] += entry.Value;
                    }
                    else
                    {
                        sumAttributesPredator[entry.Key] = entry.Value;
                    }
                }
            }
        }

        foreach (var creature in Prey)
        {
            if (creature != null && creature.genes != null)
            {
                countPrey++;
                foreach (var entry in creature.genes.Genes)
                {
                    if (sumAttributesPrey.ContainsKey(entry.Key))
                    {
                        sumAttributesPrey[entry.Key] += entry.Value;
                    }
                    else
                    {
                        sumAttributesPrey[entry.Key] = entry.Value;
                    }
                }
            }
        }

        Dictionary<string, double> averageAttributesPredator = new Dictionary<string, double>();
        foreach (var entry in sumAttributesPredator)
        {
            averageAttributesPredator[entry.Key] = entry.Value / countPredator;
        }

        Dictionary<string, double> averageAttributesPrey = new Dictionary<string, double>();
        foreach (var entry in sumAttributesPrey)
        {
            averageAttributesPrey[entry.Key] = entry.Value / countPrey;
        }


        int offset = 3; // Start genes from index 1

        Dictionary<string, int> geneNameToIndex = new Dictionary<string, int>
        {
            // You can add more preliminary data here like {"someOtherStat", 1}, then update offset
            {"energy", 0 + offset},
            {"weight", 1 + offset},
            {"height", 2 + offset},
            {"speed", 3 + offset},
            {"look radius", 4 + offset},
            {"max offspring", 5 + offset},
            {"size", 6 + offset},
            {"torso dimensions x", 7 + offset},
            {"torso dimensions y", 8 + offset},
            {"torso dimensions z", 9 + offset},
            {"head position x", 10 + offset},
            {"head position y", 11 + offset},
            {"head position z", 12 + offset},
            {"eye color", 13 + offset},
            {"storage", 14 + offset},
            {"reproductive urge", 15 + offset},
            {"head", 16 + offset},
            {"leg", 17 + offset},
            {"legs", 18 + offset},
            {"eyes", 19 + offset},
            {"torso height", 20 + offset},
            {"is predator", 21 + offset},
            {"diet", 22 + offset}
        };

        float sum_age_prey = 0;
        float sum_age_predator = 0;
        float sum_gen_prey = 0;
        float sum_gen_predator = 0;
        foreach (var creature in Prey)
        {
            sum_age_prey += creature.timeAlive;
            sum_gen_prey += creature.generation;
        }
        foreach (var creature in Predator)
        {
            sum_age_predator += creature.timeAlive;
            sum_gen_predator += creature.generation;
        }

        float avg_prey_age = countPrey > 0 ? sum_age_prey / countPrey : 0;
        float avg_predator_age = countPredator > 0 ? sum_age_predator / countPredator : 0;
        float avg_prey_gen = countPrey > 0 ? sum_gen_prey / countPrey : 0;
        float avg_predator_gen = countPredator > 0 ? sum_gen_predator / countPredator : 0;

        detailedDataList_predator[0].Add(countPredator);
        detailedDataList_prey[0].Add(countPrey);
        detailedDataList_predator[1].Add(avg_predator_age);
        detailedDataList_prey[1].Add(avg_prey_age);
        detailedDataList_predator[2].Add(avg_predator_gen);
        detailedDataList_prey[2].Add(avg_prey_gen);

        foreach (var key in geneNameToIndex.Keys)
        {
            averageAttributesPredator[key] = 0.0;
            averageAttributesPrey[key] = 0.0;
        }

        // Calculate averages if there is data
        if (countPredator > 0)
        {
            foreach (var entry in sumAttributesPredator)
            {
                averageAttributesPredator[entry.Key] = entry.Value / countPredator;
            }
            // Update detailedDataList and dataList based on average values

        }
        foreach (var entry in averageAttributesPredator)
        {
            if (geneNameToIndex.TryGetValue(entry.Key, out int index))
            {
                detailedDataList_predator[index].Add(entry.Value);
            }
        }

        // For Prey
        if (countPrey > 0)
        {
            foreach (var entry in sumAttributesPrey)
            {
                averageAttributesPrey[entry.Key] = entry.Value / countPrey;
            }
        }

        foreach (var entry in averageAttributesPrey)
        {
            if (geneNameToIndex.TryGetValue(entry.Key, out int index))
            {
                detailedDataList_prey[index].Add(entry.Value);
            }
        }
        // New code for plants
        double[] sumAttributesPlants = new double[13];  // Assuming you have 12 attributes for plants
        int countPlants = 0;

        foreach (var plant in Plants)
        {
            if (plant != null)
            {
                countPlants++;
                for (int i = 0; i < 13; i++)
                {
                    sumAttributesPlants[i] += plant.details.genes[i];  // Assuming 'Details' is your 2D array
                }
            }
        }

        int offset_plants = 3;

        double[] averageAttributesPlants = new double[13];

        for (int i = 0; i < 13; i++)
        {
            averageAttributesPlants[i] = sumAttributesPlants[i] / countPlants;
        }

        Dictionary<string, int> plantGeneNameToIndex = new Dictionary<string, int>
        {
            {"LightConsumption", 0 + offset_plants},
            {"waterConsumption", 1 + offset_plants},
            {"Weight", 2 + offset_plants},
            {"Height", 3 + offset_plants},
            {"firstbranch", 4 + offset_plants},
            {"distortion", 5 + offset_plants},
            {"trunk", 6 + offset_plants},
            {"floorh", 7 + offset_plants},
            {"twistiness", 8 + offset_plants},
            {"leavesSize", 9 + offset_plants},
            {"hardness", 10 + offset_plants},
            {"gender", 11 + offset_plants},
            {"seed", 12 + offset_plants}
        };

        float sum_plant_age = 0;
        float sum_plant_gen = 0;

        foreach (var plant in Plants)
        {
            sum_plant_age += plant.age;
            sum_plant_gen += plant.generation;

        }

        float avg_plant_age = countPlants > 0 ? sum_plant_age / countPlants : 0;
        float avg_plant_gen = countPlants > 0 ? sum_plant_gen / countPlants : 0;

        detailedDataList_plants[0].Add(countPlants);
        detailedDataList_plants[1].Add(avg_plant_age);
        detailedDataList_plants[2].Add(avg_plant_gen);

        foreach (var entry in plantGeneNameToIndex)
        {
            int index = entry.Value - offset_plants;
            double valueToAdd = Double.IsNaN(averageAttributesPlants[index]) ? 0 : averageAttributesPlants[index];
            detailedDataList_plants[index + offset_plants].Add(valueToAdd);
        }


    }
    public void RemoveNullValues()
    {
        Predator.RemoveAll(item => item == null);
        Prey.RemoveAll(item => item == null);
        Plants.RemoveAll(item => item == null);
    }

    public void ExportToCSV()
    {
        ExportDataToCSV(PlayerPrefs.GetInt("runNum"));
    }
    void ExportDataToCSV(int runNum)
    {
        // Create a new StringBuilder to hold the CSV content

        int offset_plants = 3;

        Dictionary<string, int> plantGeneNameToIndex = new Dictionary<string, int>
        {
            {"LightConsumption", 0 + offset_plants},
            {"waterConsumption", 1 + offset_plants},
            {"Weight", 2 + offset_plants},
            {"Height", 3 + offset_plants},
            {"firstbranch", 4 + offset_plants},
            {"distortion", 5 + offset_plants},
            {"trunk", 6 + offset_plants},
            {"floorh", 7 + offset_plants},
            {"twistiness", 8 + offset_plants},
            {"leavesSize", 9 + offset_plants},
            {"hardness", 10 + offset_plants},
            {"gender", 11 + offset_plants},
            {"seed", 12 + offset_plants}
        };
        int offset = 3; // Start genes from index 1

        Dictionary<string, int> geneNameToIndex = new Dictionary<string, int>
        {
            // You can add more preliminary data here like {"someOtherStat", 1}, then update offset
            {"energy", 0 + offset},
            {"weight", 1 + offset},
            {"height", 2 + offset},
            {"speed", 3 + offset},
            {"look radius", 4 + offset},
            {"max offspring", 5 + offset},
            {"size", 6 + offset},
            {"torso dimensions x", 7 + offset},
            {"torso dimensions y", 8 + offset},
            {"torso dimensions z", 9 + offset},
            {"head position x", 10 + offset},
            {"head position y", 11 + offset},
            {"head position z", 12 + offset},
            {"eye color", 13 + offset},
            {"storage", 14 + offset},
            {"reproductive urge", 15 + offset},
            {"head", 16 + offset},
            {"leg", 17 + offset},
            {"legs", 18 + offset},
            {"eyes", 19 + offset},
            {"torso height", 20 + offset},
            {"is predator", 21 + offset},
            {"diet", 22 + offset}
        };

        Dictionary<string, int> predatorGeneNameToIndex = new Dictionary<string, int>();
        Dictionary<string, int> preyGeneNameToIndex = new Dictionary<string, int>();

        foreach (var pair in geneNameToIndex)
        {
            predatorGeneNameToIndex["Predator_" + pair.Key] = pair.Value;
            preyGeneNameToIndex["Prey_" + pair.Key] = pair.Value;
        }
        StringBuilder csv = new StringBuilder();

        // Add column headers
        List<string> headers = new List<string> { "Time"};

        headers.AddRange(new List<string> {"HerbivoreCount","Prey_Age", "Prey_Generation" });  // Add count and average headers for predator and prey
        headers.AddRange(preyGeneNameToIndex.Keys);  // Add predator and prey gene names

        headers.AddRange(new List<string> { "PredatorCount", "Predator_Age", "Predator_Generation" });  // Add count and average headers for predator and prey
        headers.AddRange(predatorGeneNameToIndex.Keys);  // Add predator and prey gene names

        headers.AddRange(new List<string> { "PlantCount", "Plant_Age", "Plant_Generation" });  // Add count and average headers for plants
        headers.AddRange(plantGeneNameToIndex.Keys);  // Add plant gene names

        csv.AppendLine(string.Join(",", headers));

        // Loop through dataList and add data rows
        for (int i = 0; i < dataList[0].Count; i++) // Assuming that all lists in dataList have the same length
        {
            List<string> row = new List<string>();
            for (int j = 0; j < dataList.Count; j++)
            {
                if (i < dataList[j].Count)
                {
                    row.Add(dataList[j][i].ToString());
                }
                else
                {
                    //row.Add("NA");
                }
            }

            // Add predator, prey, and plant gene data
            foreach (var list in detailedDataList_prey) 
            {
                if (i < list.Count)
                {
                    row.Add(list[i].ToString());
                }
                else
                {
                    //row.Add("NA");
                }
            }
            foreach (var list in detailedDataList_predator) 
            {
                if (i < list.Count)
                {
                    row.Add(list[i].ToString());
                }
                else
                {
                    //row.Add("NA");
                }
            }
            foreach (var list in detailedDataList_plants)
            {
                if (i < list.Count)
                {
                    row.Add(list[i].ToString());
                }
                else
                {
                    //row.Add("NA");
                }
            }

            csv.AppendLine(string.Join(",", row));
        }

        // Save the CSV content to a file
        string directoryPath = Application.persistentDataPath + "/DataFolder";
        string filePath = directoryPath + "/DataRun" + runNum + ".csv";

        // Check if directory exists and create if not
        if (!Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
        File.WriteAllText(filePath, csv.ToString());
        Debug.Log("Data exported to " + filePath);
        StartCoroutine(ShowAndHideText("Data exported to " + filePath));
    }
    IEnumerator ShowAndHideText(string message)
    {
        infoText.text = message;  // Show
        infoText.gameObject.SetActive(true);

        yield return new WaitForSeconds(5);  // Wait for 5 seconds

        infoText.gameObject.SetActive(false);  // Hide
    }

}
