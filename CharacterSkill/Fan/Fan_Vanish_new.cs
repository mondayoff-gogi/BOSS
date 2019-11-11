using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fan_Vanish_new : MonoBehaviour
{
    public GameObject vanish_obj;
    private GameObject vanish_temp;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player") || collision.CompareTag("Boss")||collision.CompareTag("OtherPlayer"))
        {
            Instantiate(vanish_obj, collision.transform);
        }
    }
}
