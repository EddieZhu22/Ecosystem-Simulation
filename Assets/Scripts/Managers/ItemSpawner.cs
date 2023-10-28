using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
using UnityEngine.EventSystems;  // Add this namespace

public class ItemSpawner : MonoBehaviour
{
    [SerializeField] private GameObject[] prefabs;

    [SerializeField] private Vector3 mapSize;
    public int[] maximums;
    public int[] minimums;

    public float tick;

    public LayerMask mask, groundMask;

    public GameManager manager;

    [SerializeField] private GameUI UI;

    public GameObject cannotSpawn;

    private void Update()
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject())
        {
            if (Input.GetMouseButtonDown(0))
            {
                try
                {
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                    if (Physics.Raycast(ray, out RaycastHit hitInfo, Mathf.Infinity))
                    {
                        if (hitInfo.collider.gameObject.layer != mask)
                        {
                            // Check if the terrain is not water or steep
                            
                                if (UI.Page == GameUI.PageView.Creature)
                                {
                                    if (hitInfo.point.y > manager.settings.waterHeight && !IsSteepTerrain(hitInfo))
                                    {
                                        if (UI.vacant == false)
                                        {
                                            manager.CreateCreatures(hitInfo.point, manager.details, false, "Creature", 0);
                                            UI.globalUISound.PlaySound2();

                                        }
                                    }
                                    else if (UI.vacant == false)
                                    {

                                        if (IsSteepTerrain(hitInfo))
                                        {
                                            cannotSpawn.SetActive(true);
                                            cannotSpawn.GetComponent<Text>().text = "Cannot Spawn (too steep)!";
                                            StartCoroutine(SetActiveCantSpawn());
                                        }
                                    }


                                }
                                if (UI.Page == GameUI.PageView.Plants)
                                {
                                    if (hitInfo.point.y > manager.settings.waterHeight && !IsSteepTerrain(hitInfo))
                                    {
                                        if (UI.vacant2 == false)
                                        {
                                            manager.CreatePlants(manager.details2, new Vector3(hitInfo.point.x, hitInfo.point.y - 0.3f, hitInfo.point.z), false, 0);
                                            UI.globalUISound.PlaySound2();
                                        }
                                    }
                                    else if (UI.vacant2 == false)
                                    {

                                        if (IsSteepTerrain(hitInfo))
                                        {
                                            cannotSpawn.SetActive(true);
                                            cannotSpawn.GetComponent<Text>().text = "Cannot Spawn (too steep)!";
                                            StartCoroutine(SetActiveCantSpawn());
                                        }
                                    }
                                }

                            }
                        

                    }
                }
                catch
                {
                    // Log the exception for debugging
                    //Debug.LogError("An error occurred: " + e.Message);
                }
            }
        }
    }
    IEnumerator SetActiveCantSpawn()
    {
        cannotSpawn.SetActive(true);
        Vector2 mousePosition = Input.mousePosition;


        cannotSpawn.GetComponent<Text>().rectTransform.position = mousePosition;
        yield return new WaitForSeconds(0.25f);
        cannotSpawn.SetActive(false);
    }
    public static bool IsSteepTerrain(RaycastHit hitInfo)
    {
        // Check the slope of the terrain
        if(hitInfo.collider.gameObject.layer == 7)
        {
            float slopeAngle = Vector3.Angle(Vector3.up, hitInfo.normal);
            return slopeAngle > 45f; // Adjust 45 to the maximum slope angle your creature can handle
        }
        else
        {
            return false;
        }

    }   
}
