using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heal_Trail : MonoBehaviour
{
    private Transform thisparent; //케릭터

    private Vector2 dir;

    private float movespeed;
    private const float destroy_time = 1f;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, destroy_time);
        thisparent = this.gameObject.transform.parent;
        this.transform.SetParent(this.transform.parent.parent);
        movespeed = thisparent.GetComponent<Character_Control>().move_speed;
        this.gameObject.transform.SetParent(thisparent.parent);
    }
    // Update is called once per frame
    void Update()
    {
        dir.x = thisparent.position.x - this.gameObject.transform.position.x;
        dir.y = thisparent.position.y-0.2f - this.gameObject.transform.position.y;
        dir.Normalize();

        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
}
