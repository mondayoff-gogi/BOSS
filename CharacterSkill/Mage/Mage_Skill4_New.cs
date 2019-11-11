using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_Skill4_New : MonoBehaviour
{
    public GameObject fire_ball;
    private GameObject fire_ball_temp;
    private GameObject player;

    IEnumerator GenerateFireBall()
    {
        WaitForSeconds waiting_time = new WaitForSeconds(2f);
        int count = 0;
        while (count < 4)
        {
            Vector3 temp = new Vector3(this.transform.position.x + Random.Range(-5.0f, 5.0f), this.transform.position.y + 20f,this.transform.position.z);
            fire_ball_temp = Instantiate(fire_ball, temp, Quaternion.identity);
            if(this.transform.parent!=null)
            {
                fire_ball_temp.GetComponent<MageSKill4_2>().GetParameter(this.transform.position, player);
            }
            else
            {
                fire_ball_temp.GetComponent<MageSKill4_2>().GetParameter(this.transform.position, null);
            }
            count++;
            yield return waiting_time;
        }
        yield return waiting_time;

        yield return 0;
    }

    private void Start()
    {
        StartCoroutine(GenerateFireBall());
    }

    public void GetPlayer(GameObject using_skill_player)
    {
        player = using_skill_player;
    }
}
