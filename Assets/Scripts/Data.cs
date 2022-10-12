using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Data : MonoBehaviour
{
    string filename = "";
    private string[] DataStr;
    [SerializeField] private int i, k;
    private readonly PlayerPrefs iVal;
    public GameManager manager;

    [System.Serializable]
    public class Statistics
    {
        public float Time;
        public float CreaturesTotal;
        public float CreaturesPredators;
        public float CreaturesPrey;
        public float Trees;

        //Creature Genes
        public float[] speed = new float[3];
        public float[] lookRadius = new float[3];
        public float[] Gestation = new float[3];
        public float[] Size = new float[3];
        public float[] Storage = new float[3];
        public float[] ReproductiveUrge = new float[3];
        public float[] eyeColor = new float[3];

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
        filename = Application.dataPath + "/Runs" + "/run" + i + ".csv";
        i++;
        PlayerPrefs.SetInt("iVal", i);

        StartCoroutine(waitForSeconds());
    }


    public void WriteCSV()
    {
        {
            TextWriter tw = new StreamWriter(filename, false);
            tw.WriteLine("Time, Total Creatures, Predators, Prey, Trees, Speed, Look Radius, Max Offspring, Size, Storage, ReproductiveUrge, Eye Color, " +
                                                                        "p Speed, p Look Radius, p Max Offspring, p Size, p Storage, p ReproductiveUrge, p Eye Color, " +
                                                                        "P Speed, P Look Radius, P Max Offspring, P Size, P Storage, P ReproductiveUrge, P Eye Color");
            tw.Close();

            tw = new StreamWriter(filename, true);

            {

                list.stats[0].Time = Time.time;
                list.stats[0].CreaturesTotal = manager.creatures.Count;
                list.stats[0].CreaturesPrey = manager.prey.Length;
                list.stats[0].CreaturesPredators = manager.creatures.Count - manager.prey.Length;
                list.stats[0].Trees = manager.plants.Length;

                list.stats[0].speed[0] = manager.averages1[3];
                list.stats[0].lookRadius[0] = manager.averages1[4];
                list.stats[0].Gestation[0] = manager.averages1[5];
                list.stats[0].Size[0] = manager.averages1[6];
                list.stats[0].eyeColor[0] = manager.averages1[7];
                list.stats[0].Storage[0] = manager.averages1[8];
                list.stats[0].ReproductiveUrge[0] = manager.averages1[9];

                list.stats[0].speed[1] = manager.averages2[3];
                list.stats[0].lookRadius[1] = manager.averages2[4];
                list.stats[0].Gestation[1] = manager.averages2[5];
                list.stats[0].Size[1] = manager.averages2[6];
                list.stats[0].eyeColor[1] = manager.averages2[7];
                list.stats[0].Storage[1] = manager.averages2[8];
                list.stats[0].ReproductiveUrge[1] = manager.averages2[9];

                list.stats[0].speed[2] = manager.averages3[3];
                list.stats[0].lookRadius[2] = manager.averages3[4];
                list.stats[0].Gestation[2] = manager.averages3[5];
                list.stats[0].Size[2] = manager.averages3[6];
                list.stats[0].eyeColor[2] = manager.averages3[7];
                list.stats[0].Storage[2] = manager.averages3[8];
                list.stats[0].ReproductiveUrge[2] = manager.averages3[9];

                DataStr[k] = list.stats[0].Time + "," + list.stats[0].CreaturesTotal + "," +
                        list.stats[0].CreaturesPredators + "," + list.stats[0].CreaturesPrey + "," +
                        list.stats[0].Trees + "," + 
                        list.stats[0].speed[0] + "," +
                        list.stats[0].lookRadius[0] + "," +
                        list.stats[0].Gestation[0] + "," +
                        list.stats[0].Size[0] + "," +
                        list.stats[0].Storage[0] + "," +
                        list.stats[0].ReproductiveUrge[0] + "," +
                        list.stats[0].eyeColor[0] + "," +
                        list.stats[0].speed[1] + "," +
                        list.stats[0].lookRadius[1] + "," +
                        list.stats[0].Gestation[1] + "," +
                        list.stats[0].Size[1] + "," +
                        list.stats[0].Storage[1] + "," +
                        list.stats[0].ReproductiveUrge[1] + "," +
                        list.stats[0].eyeColor[1] + "," +
                        list.stats[0].speed[2] + "," +
                        list.stats[0].lookRadius[2] + "," +
                        list.stats[0].Gestation[2] + "," +
                        list.stats[0].Size[2] + "," +
                        list.stats[0].Storage[2] + "," +
                        list.stats[0].ReproductiveUrge[2] + "," +
                        list.stats[0].eyeColor[2];

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
