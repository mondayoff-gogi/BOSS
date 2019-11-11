﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_SeaStorm : MonoBehaviour
{
    public GameObject objects;

    private GameObject objects_temp;
    private const float speed = 1.5f;
    private Vector2 dir;
    private Vector2 random_vec;
    private int count = 0;
    private float t = 0.1f;
    private WaitForSeconds wait_time = new WaitForSeconds(3f);
    // Start is called before the first frame update
    private void Start()
    {
        count = UpLoadData.boss_level * 5 + 10;

        StartCoroutine(Generate());
        Destroy(this.gameObject, 12f);
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            dir = this.transform.position - collision.transform.position;
            dir.Normalize();
            collision.transform.Translate(dir * speed * Time.deltaTime);
        }
    }

    private void Update()
    {
        this.transform.localScale = new Vector3(t, t, t);
        t += 0.03f;
        if(t >= 1f)
        {
            t = 1f;
        }
        this.transform.Rotate(new Vector3(0, 0, Time.deltaTime * 1000));
    }

    IEnumerator Generate()
    {
        while (true)
        {
            for(int i = 0; i < count; i++)
            {

                random_vec = Random.insideUnitCircle.normalized * 17f;

                objects_temp = Instantiate(objects, random_vec, Quaternion.identity);
            }
            yield return wait_time;
        }
    }
}
