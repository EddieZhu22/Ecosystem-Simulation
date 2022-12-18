using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class CreatureProceduralAnimation : MonoBehaviour
{
    public Transform Torso, torsoMesh;
    public Transform Head, Neck, NeckPosition;
    public GameObject[] limbTargets;
    public Legs[] legs;
    public float offset;
    public float[] moveDist, ogmoveDist;
    public int smoothness = 1;
    public LayerMask rayLayer;
    [SerializeField] private CreatureGenes genes;
    [SerializeField] private CreatureEditor editor;
    [SerializeField] private bool _iseditor;

    private int _legNum;
    private Creature creature;
    void Start() => Set();

    void Update()
    {
        // variables
        float averagePos = 0.0F;
        float sumPos = 0;
        // loop through each leg
        for (int i = 0; i < Convert.ToDecimal(genes.Genes["legs"]); i++)
        {
            // raycast a hit from the top to find the current ground position

            if (Physics.Raycast(legs[_legNum - 1].TargetObjects[i].transform.position + new Vector3(0, 10, 0), -Vector3.up, out RaycastHit hit, Mathf.Infinity, rayLayer))
            {
                legs[_legNum - 1].TargetObjects[i].transform.position = hit.point - new Vector3(0, 0.05f, 0);
            }

            // Calculate the journey length.

            if (Vector3.Distance(legs[_legNum - 1].Armatures[i].position, legs[_legNum - 1].TargetObjects[i].transform.position) > moveDist[i])
            {
                moveDist[i] = 0;
                float speed = 90f;
                float smooth = 1.0f - Mathf.Pow(0.5f, Time.deltaTime * speed);
                Vector3 startPoint = legs[_legNum - 1].Armatures[i].position;
                Vector3 endPoint = legs[_legNum - 1].TargetObjects[i].transform.position;
                Vector3 centerPoint = (startPoint + endPoint) / 2;
                centerPoint += transform.up * Vector3.Distance(startPoint, endPoint) / 2;
                legs[_legNum - 1].Armatures[i].position = Vector3.Lerp(Vector3.Lerp(startPoint, centerPoint, smooth), Vector3.Lerp(centerPoint, endPoint, smooth), smooth);
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
        averagePos = (float)sumPos / (Convert.ToInt32(genes.Genes["legs"])) + offset;

        Torso.position = new Vector3(Torso.position.x, averagePos, Torso.position.z);
    }
    public void SetPosition(Transform obj, Vector3 toPos, Vector3 fromPos)
    {
        float y = toPos.y - fromPos.y; // delta y
        float x = toPos.x - fromPos.x; // delta x
        float r = Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2)); // Calculate Total Distance Using Pythagorean Thereom

        obj.localScale = new Vector3(1f, r*10f, 1f);
        Vector3 m = new Vector3((toPos.x + fromPos.x) / 2, (toPos.y + fromPos.y) / 2, obj.position.z);
        obj.position = m;
        if(x != 0)
        {
           //print((Mathf.Atan(y / x) * 180 / Mathf.PI));
            obj.localEulerAngles = new Vector3(0, 0, (Mathf.Atan(y / x) * 180 / Mathf.PI) + 90);
        }
    }
    public void Set()
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
            genes = creature.genes;
        }
        for (int i = 0; i < legs.Length; i++)
        {
            for (int k = 0; k < 6; k++)
            {
                legs[i].LegObjects[k].SetActive(false);
                legs[i].TargetObjects[k].SetActive(false);
                if (editor.manager.settings.CreatureLOD == 1 && _iseditor == false)
                    legs[i].BoneEnd[k].GetComponent<DitzelGames.FastIK.FastIKFabric>().enabled = false;
            }
        }
        if (_iseditor == true)
        {
            SetPosition(Neck.transform,Head.transform.position,NeckPosition.position);
            genes = GetComponent<CreatureGenes>();
            genes.Genes.Clear();
            genes.Genes.Add("legs", 2);
            _legNum = editor.legNum;
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
            _legNum = editor.legNum;
            for (int i = 0; i < 6; i++)
            {
                if (i < Convert.ToInt32(genes.Genes["legs"])) // i =2, legs=2
                {
                    legs[editor.legNum - 1].LegObjects[i].SetActive(true);
                    legs[editor.legNum - 1].TargetObjects[i].SetActive(true);
                }
            }
        }
    }
}
