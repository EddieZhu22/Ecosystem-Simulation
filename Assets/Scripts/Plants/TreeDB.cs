using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDB : MonoBehaviour
{
    public GameObject[] TreeGameObjectDataBase = new GameObject[1000];
    public float [,] genes = new float[1000,14];
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    public void GenerateTree(GameObject tree, int index)
    {
        GameObject treeObj = Instantiate(tree, transform.position, Quaternion.identity);
        TreeGameObjectDataBase[index] = treeObj;
    }
}
