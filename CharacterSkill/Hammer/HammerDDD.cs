using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerDDD : MonoBehaviour
{
    private Transform thisparent;
    
    private int Damage;

    void Start()
    {
        SoundManager.instance.Play(28);

        thisparent = this.gameObject.transform.parent;
        Destroy(this.gameObject, 2.5f);
        StartCoroutine(Delay());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(thisparent!=null)
        {
            if (collision.CompareTag("Boss"))
            {
                SkillManager.instance.HammerDDD++;
                Damage = SkillManager.instance.HammerDDD * SkillManager.instance.HammerDDD;

                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage((40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1f) * Damage, false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage((40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1f) * Damage, false, thisparent.gameObject);
                }
            }
            if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1f);
                }
            }
        }
    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;        
    }
}
