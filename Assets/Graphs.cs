using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vectrosity;
public class Graphs : MonoBehaviour
{
    VectorLine myLine, myLine2, myLine3, myLine4, myLine5;

    public Vector2 startPos;

    public int i, t;

    public GameManager manager;

    public float[] maxVal;

    public float[] nums;

    public List<float> vals;

    void Start()
    {
        myLine = VectorLine.SetLine(Color.green, Creatures1(), Creatures1());
        myLine5 = VectorLine.SetLine(Color.yellow, Creatures1(), Creatures1());
        myLine2 = VectorLine.SetLine(Color.red, Plants(), Plants());
        myLine3 = VectorLine.SetLine(Color.black, LookRadius(), LookRadius());
        myLine4 = VectorLine.SetLine(Color.blue, Speed(), Speed());
    }
    Vector2 Creatures1()
    {
        nums[0] = manager.prey.Length;
        return new Vector2(i, nums[0]/2) + startPos;
    }
    Vector2 Creatures2()
    {
        nums[0] = manager.predator.Length;
        return new Vector2(i, nums[0] / 2) + startPos;
    }
    Vector2 Plants()
    {
        nums[1] = manager.plants.Length;

        return new Vector2(i, nums[1]/5) + startPos;
    }
    Vector2 LookRadius()
    {
        nums[2] = manager.averages[0];

        return new Vector2(i, nums[2] * 100) + startPos;
    }
    Vector2 Speed()
    {
        nums[3] = manager.averages[1];

        return new Vector2(i, nums[3] * 100) + startPos;
    }
    void Update()
    {
        t--;
        if(t < 0)
        {


            myLine.points2.Add(Creatures1());
            myLine2.points2.Add(Plants());
            myLine3.points2.Add(LookRadius());
            myLine4.points2.Add(Speed());
            myLine5.points2.Add(Creatures2());
            i++;
            myLine.Draw();
            myLine2.Draw();
            myLine3.Draw();
            myLine4.Draw();
            myLine5.Draw();


            if (i > 200)
            {
                vals.RemoveAt(0);
                vals.Add(nums[1]);
                myLine.rectTransform.position -= new Vector3(1f, 0, 0);
                myLine.points2.RemoveAt(0);

                myLine2.rectTransform.position -= new Vector3(1f, 0, 0);
                myLine2.points2.RemoveAt(0);

                myLine3.rectTransform.position -= new Vector3(1f, 0, 0);
                myLine3.points2.RemoveAt(0);

                myLine4.rectTransform.position -= new Vector3(1f, 0, 0);
                myLine4.points2.RemoveAt(0);

                myLine5.rectTransform.position -= new Vector3(1f, 0, 0);
                myLine5.points2.RemoveAt(0);
                //for (int i = 0; i < 200; i++)
                //{
                //    myLine2.points2[i].Set(myLine2.points2[i].x, 0);
                //    Debug.Log(myLine2.points2.Count);
                //}
            }
            else
            {
                vals[i] = nums[1];

            }
        }

        
    }
}
