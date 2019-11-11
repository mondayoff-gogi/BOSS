using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Sandstorm : MonoBehaviour
{
    private GameObject[] player;

    private Vector3 vec3_temp;

    private float time_temp;

    private bool trigger;

    // Start is called before the first frame update
    void Start()
    {
        trigger = false;
        player = GameObject.FindGameObjectsWithTag("Player");
        vec3_temp = new Vector3(0.5f,0,0);
        StartCoroutine(Delay());
        time_temp = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (time_temp > 30)
            Destroy(this.gameObject);
        time_temp += Time.deltaTime;
        if(trigger)
        {
            for (int i = 0; i < player.Length; i++)
            {
                if(player[i].CompareTag("Player"))
                    player[i].transform.Translate(vec3_temp * Time.deltaTime);
            }
        }        
    }    
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(3f);
        trigger = true;
    }
}
