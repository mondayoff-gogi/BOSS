using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_NormalAttack : MonoBehaviour
{
    private Vector2 dir;
    private const float movespeed = 2f;

    private void Update()
    {
        if (BossStatus.instance == null) Destroy(this.gameObject);
        dir = BossStatus.instance.transform.position - this.transform.position;
        this.transform.Translate(dir * movespeed * Time.deltaTime);
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            Destroy(this.gameObject);
        }
            
    }
}
