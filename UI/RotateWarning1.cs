using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateWarning1 : MonoBehaviour
{
    private Vector2 dir;

    void Start()
    {
    
        this.transform.SetParent(this.transform.parent.parent);

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, 0.3f);
    }

}
