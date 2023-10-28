using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public bool isTransitioning;
    public GameObject transitionObject;
    void Start()
    {
        Screen.fullScreen = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Play()
    {
        StartCoroutine(transition());
    }
    public void Quit()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }
    IEnumerator transition()
    {
        if (isTransitioning)
        {
            yield break;  // Exit the coroutine
        }
        GetComponent<Animator>().SetBool("transition", true);
        //pauseSim();
        isTransitioning = true;
        yield return new WaitForSeconds(2f);
        SceneManager.LoadScene("Ecosystem");

    }
}
