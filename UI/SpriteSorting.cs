using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSorting : MonoBehaviour
{
    Vector3 temp;
    public float pivot;
    private void Update()
    {
       
        temp = this.transform.position;
        if (this.CompareTag("DeadPlayer"))
        {
            temp.z = -100 + this.transform.position.y;
            return;
        }
        temp.z = -500+this.transform.position.y+ pivot;
        this.transform.position = temp;

    }


}
