using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkillPillar : MonoBehaviour
{
    private Animator _animator;
    //private GameObject[] target_player;

    //private LineRenderer _linerender;

    //private Vector3 temp_vec3;

    //private int rand;

    //private bool dot_damage = false;

    //private float temp_time = 0;

    public GameObject prefab;
    private GameObject prefab_Temp;   
    
    //void Update()
    //{
    //    temp_time += Time.deltaTime;
    //    if (target_player == null)
    //        return;

    //    if (Vector3.Distance(target_player[rand].transform.position, this.transform.position) > 12)
    //    {
    //        _linerender.enabled = false;
    //        dot_damage = false;
    //        //이펙트 쾅 (노바같은 이펙트)

    //        //해당 이펙트에서 데미지
    //    }

    //    if(dot_damage&&temp_time>2.0f) //2초마다 데미지
    //    {
    //        target_player[rand].GetComponent<CharacterStat>().GetDamage(10);
    //        temp_time = 0;    
    //    }
    //}

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.tag=="BossAttack")
        {            
            StartCoroutine(DestroyPillar());
        }
    }
    IEnumerator DestroyPillar()
    {
        prefab_Temp = Instantiate(prefab, this.transform);
        yield return new WaitForSeconds(0.5f);
        Destroy(this.gameObject);
    }
   
}