using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_PillarCircle : MonoBehaviour
{
    bool is_charIn;

    float temp_time = 0;


    private void Start()
    {
        is_charIn = false;
    }
    // Update is called once per frame
    void Update()
    {
        temp_time += Time.deltaTime;
        if (temp_time > 4.5f)
        {
            if (is_charIn)
                Destroy(this.gameObject);
        }
    }



    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="Player")
            is_charIn = true;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.tag=="Player")
            is_charIn = false;
    }
}
