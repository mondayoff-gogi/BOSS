using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    private Transform thisparent;

    public GameObject skill;
    private GameObject skill_temp;

    void Start()
    {
        SoundManager.instance.Play(23);
        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            StartCoroutine(Delay());
        }
        Destroy(this.gameObject, 0.2f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            SoundManager.instance.Play(33);
            skill_temp = Instantiate(skill, collision.transform);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(126, skill_temp.transform, 2f);
            }
            Destroy(skill_temp, 2f);
            if (thisparent != null)
            {
                BossStatus.instance.GetBuff(0); //방어력감소
                BossStatus.instance.GetBuff(1); //연계
                skill_temp.transform.SetParent(this.transform.parent.parent);

                //임팩트

                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f, false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 1.2f, false, thisparent.gameObject);
                }
                //this.GetComponent<Collider2D>().enabled = false; //하나만 때려짐
            }
            else
            {
                skill_temp.transform.SetParent(thisparent);
            }
        }

        if (collision.CompareTag("Monster"))
        {
            SoundManager.instance.Play(33);
            skill_temp = Instantiate(skill, collision.transform);
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.InstantiateOtherPlayerSkill(126, skill_temp.transform, 2f);
            }
            Destroy(skill_temp, 2f);
            //임팩트
            if (thisparent != null)
            {
                skill_temp.transform.SetParent(this.transform.parent.parent);

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
            else
            {
                skill_temp.transform.SetParent(thisparent);
            }
        }
    }
    IEnumerator Delay()
    {
        thisparent.GetComponent<Character_Control>()._animator.SetBool("Attack", true);
        yield return new WaitForSeconds(0.1f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("Attack", false);
    }
}
