using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkill2_3 : MonoBehaviour
{
    public GameObject hit_effect;
    private GameObject temp_effect;

    private GameObject character;

    private float physical_attack;
    private float multiple = 1.5f;

    private GameObject thisparent;

    private void Start()
    {
        if(this.transform.parent.parent!=null)
        {
            physical_attack = this.transform.parent.parent.GetComponent<CharacterStat>().PhysicalAttackPower;
            thisparent = this.transform.parent.parent.gameObject;
        }        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.CompareTag("Boss"))
        {
            temp_effect = Instantiate(hit_effect, collision.transform);
            temp_effect.transform.SetParent(this.transform.parent.parent);
            Destroy(temp_effect, 0.2f);
            if (this.transform.parent.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage((270 + physical_attack * multiple), false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(270 + physical_attack * multiple, false, thisparent.gameObject);
                }
            }
            Destroy(this.gameObject);
        }
        if (collision.CompareTag("Monster"))
        {
            temp_effect = Instantiate(hit_effect, collision.transform);
            Destroy(temp_effect, 0.2f);
            temp_effect.transform.SetParent(this.transform.parent.parent);
            if (this.transform.parent.parent != null)
            {
                if (Random.Range(1.0f, 100.0f) <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(270 + physical_attack * multiple);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(270 + physical_attack * multiple);
                }
            }
            Destroy(this.gameObject);
        }
    }
}
