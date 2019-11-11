using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill3_2 : MonoBehaviour
{

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        transform.localScale += new Vector3(0.0001f, 0.0001f, 0);
    }
}
