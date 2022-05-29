using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Data : MonoBehaviour
{
    string filename = "";
    private string[] DataStr;
    [SerializeField] private int i, k;
    PlayerPrefs iVal;
    public GameManager manager;

    [System.Serializable]
    public class Statistics
    {
        public float Time;
        public float CreaturesTotal;
        public float CreaturesPredators;
        public float CreaturesPrey;
        public float Trees;
        public float speed;
        public float lookRadius;
    }

    [System.Serializable]
    public class StatisticsList
    {
        public Statistics[] stats;
    }
    public StatisticsList list = new StatisticsList();

    void Start()
    {
        DataStr = new string[100000];
        i = PlayerPrefs.GetInt("iVal");

        //list.stats = new Statistics[k + 1];
        filename = Application.dataPath + "/run" + i + ".csv";
        i++;
        PlayerPrefs.SetInt("iVal", i);

        StartCoroutine(waitForSeconds());
    }


    public void WriteCSV()
    {
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Time, Total Creatures, Predators, Prey, Trees, Speed, Look Radius");
            tw.Close();

            tw = new StreamWriter(filename, true);

            {

                list.stats[0].Time = Time.time;
                list.stats[0].CreaturesTotal = manager.creatures.Length;
                list.stats[0].CreaturesPrey = manager.prey.Length;
                list.stats[0].CreaturesPredators = manager.creatures.Length - manager.prey.Length;
                list.stats[0].Trees = manager.plants.Length;
                list.stats[0].speed = manager.averages[0];
                list.stats[0].lookRadius = manager.averages[1];

                DataStr[k] = list.stats[0].Time + "," + list.stats[0].CreaturesTotal + "," +
                        list.stats[0].CreaturesPredators + "," + list.stats[0].CreaturesPrey + "," +
                        list.stats[0].Trees + "," + list.stats[0].speed + "," +
                        list.stats[0].lookRadius;
                for (int i = 0; i < k; i++)
                {
                    tw.WriteLine(DataStr[i]);
                }
                k++;
            }



            tw.Close();
        }
    }
    public IEnumerator waitForSeconds()
    {

        WriteCSV();

        yield return new WaitForSeconds(1 / Time.timeScale);

        StartCoroutine(waitForSeconds());
    }

}
