using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Destroy_Effect_UI : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
