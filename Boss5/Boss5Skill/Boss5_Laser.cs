using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Laser : MonoBehaviour
{
    public GameObject[] Laser;
    private GameObject laser_temp;   

    WaitForSeconds waittime;

    private Vector2 pos;
    void Start()
    {
        waittime = new WaitForSeconds(0.05f);
        StartCoroutine(MakeBullet());
        Destroy(this.gameObject, 0.2f);
    }
    IEnumerator MakeBullet()
    {
        while (true)
        {
            for(int i=0;i<6;i++)
            {
                laser_temp = Instantiate(Laser[0], this.gameObject.transform);
                pos = this.transform.position;
                pos.y += i-3;
                laser_temp.transform.position = pos;
            }            
            yield return waittime;
            for (int i = 0; i < 6; i++)
            {
                laser_temp = Instantiate(Laser[1], this.gameObject.transform);
                pos = this.transform.position;
                pos.y += i - 3;
                laser_temp.transform.position = pos;
            }
            yield return waittime;            
        }
    }
}
