using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7NormalAttackShow : MonoBehaviour
{
    private Vector2 dir;
    private float dir_plus;
    private float movespeed;
    private float timer;
    private float width;
    
    void Start()
    {
        dir = Vector2.right;
        movespeed = 3f;
        width = 3f;
        dir *= movespeed;
        timer = 0;
        Destroy(this.gameObject, 6f);
    }

    void Update()
    {
        dir_plus = width*Mathf.Cos(timer*2);
        dir.y = dir_plus;
        timer += Time.deltaTime;
        this.transform.Translate(dir*movespeed*Time.deltaTime);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boundary"))
        {
            Destroy(this.gameObject,1f);
        }
    }

}
