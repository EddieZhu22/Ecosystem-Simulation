using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class CreatureProceduralAnimation : MonoBehaviour
{
    public Transform Torso, torsoMesh;
    public GameObject[] targets, limbTargets;
    public GameObject[] legs;
    public Vector3 normal;
    public float offset;
    public float[] moveDist, ogmoveDist;
    public int smoothness = 1;
    public LayerMask rayLayer;
    private RaycastHit hit;

    [SerializeField] private CreatureDetails stats;

    void Start()
    {
        if (transform.parent != null)
        {
            if (transform.parent.name == "Editor")
                Editor();
        }
        else
            stats = GetComponent<CreatureBehavior>().stats;
        //if (transform.root == this.gameObject)
        //    Set();
    }

    void Update()
    {
        float averagePos = 0.0F;
        float sumPos = 0;
        if (transform.root.gameObject.name == "Editor")
        {
            for (int i = 0; i < targets.Length; i++)
            {
                //Debug.Log(rayLayer.value);
                if (Physics.Raycast(targets[i].transform.position + new Vector3(0, 50, 0), -Vector3.up, out hit, Mathf.Infinity, rayLayer))
                {
                    targets[i].transform.position = hit.point;
                    Debug.DrawLine(targets[i].transform.position + new Vector3(0, 50, 0), hit.point, Color.cyan);
                }
                // Calculate the journey length.
                //Debug.Log(Vector3.Distance(legs[i].position, targets[i].position));
                if (Vector3.Distance(legs[i].transform.position, targets[i].transform.position) > moveDist[i])
                {
                    moveDist[i] = 0;
                    float speed = 50f;
                    float smooth = 1.0f - Mathf.Pow(0.5f, Time.deltaTime * speed);
                    legs[i].transform.position = Vector3.Lerp(legs[i].transform.position, targets[i].transform.position, smooth);
                }
                if (Vector3.Distance(legs[i].transform.position, targets[i].transform.position) <= 0.05)
                {
                    moveDist[i] = ogmoveDist[i];
                }
                sumPos += targets[i].transform.position.y;
            }
            averagePos = (float)sumPos / targets.Length;
            //Torso.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            try
            {
                Torso.position = new Vector3(Torso.position.x, averagePos + offset, Torso.position.z);
            }
            catch
            {
                Destroy(gameObject);
            }
        }
        else
        {

            for (int i = 0; i < stats.genes[13]; i++)
            {
                if (Physics.Raycast(targets[i].transform.position + new Vector3(0, 50, 0), -Vector3.up, out hit, Mathf.Infinity, rayLayer))
                {
                    targets[i].transform.position = hit.point;
                    Debug.DrawLine(targets[i].transform.position + new Vector3(0, 50, 0), hit.point, Color.cyan);
                }
                // Calculate the journey length.
                //Debug.Log(Vector3.Distance(legs[i].position, targets[i].position));
                if (Vector3.Distance(legs[i].transform.position, targets[i].transform.position) > moveDist[i])
                {
                    moveDist[i] = 0;
                    float speed = 50f;
                    float smooth = 1.0f - Mathf.Pow(0.5f, Time.deltaTime * speed);
                    legs[i].transform.position = Vector3.Lerp(legs[i].transform.position, targets[i].transform.position, smooth);
                }
                if (Vector3.Distance(legs[i].transform.position, targets[i].transform.position) <= 0.05)
                {
                    moveDist[i] = ogmoveDist[i];
                }
                sumPos += targets[i].transform.position.y;
            }
            averagePos = (float)sumPos / stats.genes[13];
            //Torso.rotation = Quaternion.FromToRotation(Vector3.up, hit.normal);
            try
            {
                Torso.position = new Vector3(Torso.position.x, averagePos + offset, Torso.position.z);
            }
            catch
            {
                Destroy(gameObject);
            }
        }
    }
    public void Set()
    {
        targets = GameObject.FindGameObjectsWithTag("mt");
        legs = GameObject.FindGameObjectsWithTag("ar");
        for (int i = 0; i < legs.Length; i++)
        {
            if (targets[i].transform.root.gameObject.name == "Editor" || targets[i].transform.root != transform)
            {
                legs[i] = null;
            }
        }
        List<GameObject> gameObjectList = new List<GameObject>(legs);
        gameObjectList.RemoveAll(x => x == null);
        legs = gameObjectList.ToArray();
        for (int i = 0; i < targets.Length; i++)
        {
            //Debug.Log(targets[i].transform.root);
            if (targets[i].transform.root.gameObject.name == "Editor" || targets[i].transform.root != transform)
            {
                targets[i] = null;
            }
        }
        List<GameObject> gameObjectList2 = new List<GameObject>(targets);
        gameObjectList2.RemoveAll(x => x == null);
        targets = gameObjectList2.ToArray();
        GameObject[] gameObjectList3 = new GameObject[targets.Length];
        for (int i = 0; i < targets.Length; i++)
        {
            for (int k = 0; k < targets.Length; k++)
            {
                if (targets[i].name.Contains(k.ToString()))
                {
                    gameObjectList3[k] = targets[i];
                }
            }
            targets[i].SetActive(true);
        }
        targets = gameObjectList3;
        legs = GetComponent<CreatureBehavior>().armatures;

        offset = stats.genes[15];

    }
    public void Editor()
    {
        targets = GameObject.FindGameObjectsWithTag("mt");
        legs = GameObject.FindGameObjectsWithTag("ar");
        for (int i = 0; i < legs.Length; i++)
        {
            if (legs[i].transform.root.gameObject.name != "Editor")
            {
                legs[i] = null;
            }
        }
        List<GameObject> gameObjectList = new List<GameObject>(legs);
        gameObjectList.RemoveAll(x => x == null);
        legs = gameObjectList.ToArray();
        for (int i = 0; i < targets.Length; i++)
        {
            if (targets[i].transform.root.gameObject.name != "Editor")
            {
                targets[i] = null;
            }
        }
        List<GameObject> gameObjectList2 = new List<GameObject>(targets);
        gameObjectList2.RemoveAll(x => x == null);
        targets = gameObjectList2.ToArray();
    }
}
