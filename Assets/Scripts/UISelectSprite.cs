using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISelectSprite : MonoBehaviour
{

    public Transform[] UIElements;

    void Start()
    {
        
    }

    public void MoveToObject(Transform buttonTransform)
    {
        for (int i = 0; i < UIElements.Length; i++)
        {
            if(buttonTransform == UIElements[i])
                transform.position = buttonTransform.position;
        }
    }
}
