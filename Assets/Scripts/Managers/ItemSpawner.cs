using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private Vector3 mapSize;
    public int[] maximums;
    public int[] minimums;

    public float tick;

    public LayerMask mask;

    public GameManager manager;

    [SerializeField] private GameUI UI;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
            {
                //Debug.Log(hitInfo.point);
                //Instantiate(prefabs[0], hitInfo.point, Quaternion.identity);
                if (hitInfo.collider.gameObject.layer != mask)
                {
                    if (UI.mainDropDown.value == 1)
                    {
                        if (UI.vacant == false)
                            manager.CreateCreatures(hitInfo.point,manager.details,false, "Creature");
                    }
                    if (UI.mainDropDown.value == 2)
                    {
                        if (UI.vacant2 == false)
                            manager.CreatePlants(manager.details2,new Vector3(hitInfo.point.x,hitInfo.point.y - 0.3f,hitInfo.point.z),false);
                    }
                }
            }
            /*
            if (Physics.Raycast(ray, out RaycastHit hitpt, Mathf.Infinity))
            {
                Debug.Log(hitInfo.transform.gameObject.layer);
                //Instantiate(prefabs[0], hitInfo.point, Quaternion.identity);
                if (Input.mousePosition.x < 740)
                {
                    if(hitpt.transform.gameObject.tag != "")
                    {
                        ui.scene = 3;
                        Camera.main.GetComponent<FreeLookCam>().target = hitpt.transform;
                    }
                }
            }
            */
        }

    }
}
