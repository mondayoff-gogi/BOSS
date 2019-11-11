using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_AbsorbBlood : MonoBehaviour
{
    private GameObject[] bloods_array;
    // Start is called before the first frame update
    void Start()
    {
        bloods_array = GameObject.FindGameObjectsWithTag("Object");
        for (int i = 0; i < bloods_array.Length; i++)
        {
            bloods_array[i].GetComponent<Boss_BloodMove>().enabled = true;
            bloods_array[i].GetComponent<CircleCollider2D>().enabled = true;
        }
    }

}

