using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreatureProceduralAnimation : MonoBehaviour
{
    public Transform Torso, torsoMesh;
    public GameObject[] limbTargets;
    public Legs[] legs;
    public Vector3 normal;
    public float offset;
    public float[] moveDist, ogmoveDist;
    public int smoothness = 1;
    public LayerMask rayLayer;
    private RaycastHit hit;
    private Creature creature;
    [SerializeField] private CreatureDetails stats;
    [SerializeField] private CreatureEditor editor;
    [SerializeField] private bool _iseditor;
    private int _legNum;
    void Start()
    {
        if (transform.parent != null)
        {
            editor = transform.parent.GetComponent<CreatureEditor>();
            _iseditor = true;
        }
        else
        {
            creature = Torso.GetComponent<Creature>();
            editor = creature.editor;
            stats = creature.stats;
        }

        Set();
    }

    void Update()
    {
        // variables
        float averagePos = 0.0F;
        float sumPos = 0;

        // loop through each leg
        for (int i = 0; i < stats.genes[13]; i++)
        {
            // raycast a hit from the top to find the current ground position

            if (Physics.Raycast(legs[_legNum - 1].TargetObjects[i].transform.position + new Vector3(0, 10, 0), -Vector3.up, out hit, Mathf.Infinity, rayLayer))
            {
                legs[_legNum - 1].TargetObjects[i].transform.position = hit.point;
                //Debug.DrawLine(legs[_legNum - 1].TargetObjects[i].transform.position + new Vector3(0, 30, 0), hit.point, Color.cyan);
            }

            // Calculate the journey length.

            if (Vector3.Distance(legs[_legNum - 1].Armatures[i].position, legs[_legNum - 1].TargetObjects[i].transform.position) > moveDist[i])
            {
                moveDist[i] = 0;
                float speed = 50f;
                float smooth = 1.0f - Mathf.Pow(0.5f, Time.deltaTime * speed);
                legs[_legNum - 1].Armatures[i].position = Vector3.Lerp(legs[_legNum - 1].Armatures[i].position, legs[_legNum - 1].TargetObjects[i].transform.position, smooth);
            }

            // set the move distance to the normal move distance

            if (Vector3.Distance(legs[_legNum - 1].Armatures[i].position, legs[_legNum - 1].TargetObjects[i].transform.position) <= 0.05)
            {
                moveDist[i] = ogmoveDist[i];
            }

            // sum all positions for avg position

            sumPos += legs[_legNum - 1].TargetObjects[i].transform.position.y;
        }

        // average all positions for torso positions
        averagePos = (float)sumPos / (stats.genes[13]) + offset;

        Torso.position = new Vector3(Torso.position.x, averagePos, Torso.position.z);
    }
    public void Set()
    {
        for (int i = 0; i < legs.Length; i++)
        {
            for (int k = 0; k < 6; k++)
            {
                legs[i].LegObjects[k].SetActive(false);
                legs[i].TargetObjects[k].SetActive(false);
                if(editor.manager.settings.CreatureLOD == 1 && _iseditor == false)
                    legs[i].BoneEnd[k].GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = false;
            }
        }
        if (_iseditor == true)
        {
            _legNum = editor.legNum;
            stats.genes[13] = editor.legs;
            offset = editor.torsoHeight;
            for (int i = 0; i < 6; i++)
            {
                if (i < editor.legs) // i =2, legs=2
                {
                    legs[editor.legNum - 1].LegObjects[i].SetActive(true);
                    legs[editor.legNum - 1].TargetObjects[i].SetActive(true);
                }
            }
        }
        else
        {
            _legNum = Convert.ToInt32(stats.genes[12]);
            offset = stats.genes[15];
            for (int i = 0; i < 6; i++)
            {
                if (i < stats.genes[13]) // i =2, legs=2
                {
                    legs[editor.legNum - 1].LegObjects[i].SetActive(true);
                    legs[editor.legNum - 1].TargetObjects[i].SetActive(true);
                }
            }
        }
    }
}
