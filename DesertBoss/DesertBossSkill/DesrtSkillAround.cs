using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesrtSkillAround : MonoBehaviour
{
    private CharacterStat char_stat;
    // Start is called before the first frame update

    public GameObject skill_around_hit_prefab;

    private GameObject skill_around_prefab_temp;

    private Vector3 vector3_temp;



    private GameObject[] player;

    private void Start()
    {
        player = GameManage.instance.player;

        for(int i=0;i<GameManage.instance.num_char;i++)
        {
            if (Vector3.Distance(player[i].transform.position,this.transform.position)<3)
            {
                char_stat = player[i].gameObject.GetComponent<CharacterStat>();
                if(char_stat.tag=="Player")
                char_stat.GetDamage(10f, false);
                
                vector3_temp = player[i].transform.position;
                vector3_temp.z = -5;

                skill_around_prefab_temp = Instantiate(skill_around_hit_prefab, vector3_temp, Quaternion.identity);//맞은 이펙트
                Destroy(skill_around_prefab_temp, 0.5f);


                vector3_temp = player[i].transform.position - this.transform.position;
                vector3_temp.z = 0;
                player[i].transform.Translate(0.2f * (vector3_temp.normalized)); //닿은 사람 밀어내고
                

            }

        }
    }

}


