using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill3 : MonoBehaviour
{
    float temp = 6;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 2.0f);
    }

    // Update is called once per frame
    void Update()
    { 

        temp -= 0.1f;
        this.transform.Translate(Vector3.up * temp * Time.deltaTime);
        transform.localScale += new Vector3(0.01f, 0.01f, 0);

    }
}
