using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Back_attack_trail : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private const float movespeed = 20f;
    private const float destroy_time = 0.4f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroy_time);

        thisparent = this.gameObject.transform.parent;
        this.gameObject.transform.SetParent(thisparent.parent);
    }
    // Update is called once per frame
    void Update()
    {
        dir.x = thisparent.position.x - this.gameObject.transform.position.x;
        dir.y = thisparent.position.y - this.gameObject.transform.position.y;
        dir.Normalize();

        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
}
