using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill3_4 : MonoBehaviour
{
    float temp = 6;

    // Update is called once per frame
    void Update()
    {
        temp -= 0.1f;
        this.transform.Translate(Vector3.up * temp * Time.deltaTime);
        transform.localScale += new Vector3(0.01f, 0.01f, 0);
    }
}
