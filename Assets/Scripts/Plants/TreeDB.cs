using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TreeDB : MonoBehaviour
{
    public List<GameObject> TreeGameObjectDataBase;
    public List<List<float>> genes = new List<List<float>>();
    public void GenerateTree(GameObject tree)
    {
        GameObject treeObj = Instantiate(tree, transform.position, Quaternion.identity);
        treeObj.layer = LayerMask.NameToLayer("TreeDB");
        TreeGameObjectDataBase.Add(treeObj);
    }
}
