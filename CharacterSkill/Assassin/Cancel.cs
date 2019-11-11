using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cancel : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    public GameObject skill;
    private GameObject skill_temp;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(9);

        thisparent = this.gameObject.transform.parent;
       
        StartCoroutine(DeleteCollider());
        
        Destroy(this.gameObject, 0.5f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                SoundManager.instance.Play(25);

                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 1f);
                //임팩트

                if (BossStatus.instance.SilentAble)
                {
                    BossStatus.instance.BossGetSilent();
                    float temp = Random.Range(1.0f, 100.0f);

                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(500 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.5f, false, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(500 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 2.5f, false, thisparent.gameObject);
                    }
                    thisparent.GetComponent<CharacterStat>().GetMana(SkillManager.instance.skill_mana[3]);
                }
                else
                {
                    float temp = Random.Range(1.0f, 100.0f);

                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f, false, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f, false, thisparent.gameObject);
                    }
                }
            }
            else if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f);
                }
            }
        } 
        else
        {
            if (collision.CompareTag("Boss")|| collision.CompareTag("Monster"))
            {
                SoundManager.instance.Play(25);
                skill_temp = Instantiate(skill, collision.transform);
                BossStatus.instance.BossGetSilent();
                Destroy(skill_temp, 1f);
            }
        }
    }
    IEnumerator DeleteCollider()
    {
        yield return new WaitForSeconds(0.1f);
        this.gameObject.GetComponent<Collider2D>().enabled = false;
    }
}
