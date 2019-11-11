using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerDown : MonoBehaviour
{
    private Transform thisparent;


    void Start()
    {
        SoundManager.instance.Play(10);

        thisparent = this.gameObject.transform.parent;

        if (thisparent != null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            StartCoroutine(DeleteCollider());
        }
        this.GetComponent<Collider2D>().enabled = false;
        Destroy(this.gameObject, 1.0f);
        StartCoroutine(Delay());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                BossStatus.instance.GetBuff(4);
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.5f, false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.5f, false, thisparent.gameObject);
                }
            }
            if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.5f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.5f);
                }
            }
        }
    }
    IEnumerator DeleteCollider()
    {
        thisparent.GetComponent<Character_Control>().Runable = false;
        thisparent.GetComponent<Character_Control>()._animator.SetBool("Attack", true);

        yield return new WaitForSeconds(0.6f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("Attack", false);
        thisparent.GetComponent<Character_Control>().Runable = true;

    }
    IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.4f);
        this.GetComponent<Collider2D>().enabled = true;
        yield return new WaitForSeconds(Time.deltaTime);
        this.GetComponent<Collider2D>().enabled = false;

    }
}
