using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill3_3 : MonoBehaviour
{
    public GameObject hit_effect;
    private GameObject hit_temp;

    private float physical_attack;
    private float multiple;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 3f);
        StartCoroutine(Delay());
        multiple = 1f;
        if(this.transform.parent.parent!=null)
        {
            physical_attack = BossStatus.instance.player[0].GetComponent<CharacterStat>().PhysicalAttackPower;
        }
            
    }

    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);            
            Destroy(hit_temp, 1f);
            if (this.transform.parent.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= BossStatus.instance.player[0].GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(200 + physical_attack * multiple, false, BossStatus.instance.player[0].gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(200 + physical_attack * multiple, false, BossStatus.instance.player[0].gameObject);

                }
                BossStatus.instance.GetBuff(7, this.transform.parent.parent.gameObject);
            }
        }
        if(collision.CompareTag("Monster"))
        {
            hit_temp = Instantiate(hit_effect, collision.transform);
            Destroy(hit_temp, 1f);
            if (this.transform.parent.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= BossStatus.instance.player[0].GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(200 + physical_attack * multiple);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(200 + physical_attack * multiple);

                }
            }
        }
    }
}
