using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_CreateObj : MonoBehaviour
{
    public GameObject[] objs;
    private float spawn_time;

    // Update is called once per frame
    void Update()
    {
        spawn_time += Time.deltaTime;
        if(spawn_time >= 2f)
        {
            int temp = Random.Range(0, 5);
            Instantiate(objs[temp], (Vector2)this.transform.position, Quaternion.identity);
            spawn_time = 0;
        }
    }
}
