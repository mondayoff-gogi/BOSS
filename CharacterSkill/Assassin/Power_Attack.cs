using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Attack : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private float temp;

    public GameObject skill;
    private GameObject skill_temp;
    private Character_Control _character;
    void Start()
    {
        SoundManager.instance.Play(4);

        Destroy(this.gameObject, 0.6f);
        StartCoroutine(DeleteCollider()); //바로 collider 삭제

        thisparent = this.gameObject.transform.parent;
        if (thisparent != null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            dir.x = thisparent.transform.position.x - SkillManager.instance.mouse_pos.x;
            dir.y = thisparent.transform.position.y - SkillManager.instance.mouse_pos.y;
            dir.Normalize();
            this.transform.Translate(-dir * 2);
            temp = BossStatus.instance.BuffAmount(0);
            temp = temp * 0.5f + 1;    //버프 중첩 많이 할수록 쌤
            _character = thisparent.gameObject.GetComponent<Character_Control>();
            _character.RunningStop();
            _character._animator.SetFloat("DirX", -dir.x);
            _character._animator.SetFloat("DirY", -dir.y);
            StartCoroutine(Delay());
        }
        else
        {
            dir.x = this.transform.position.x - NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z].x;
            dir.y = this.transform.position.y - NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z].y;
            dir.Normalize();
            this.transform.Translate(-dir * 2);
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                float critical = Random.Range(1.0f, 100.0f);

                if (critical <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(150 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * temp * 2f, false, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(150 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * temp * 2f, false, thisparent.gameObject);
                }
                if (NetworkManager.instance.is_multi)
                {
                    NetworkManager.instance.BossDeleteBuff(0);
                }else
                    BossStatus.instance.BossBuffDelete(0);
                skill_temp = Instantiate(skill, collision.transform);
                skill_temp.transform.localScale *= temp;
                skill_temp.transform.SetParent(skill_temp.transform.parent.parent);
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                _character.Runable = true;
                Destroy(skill_temp, 2f);
            }
            if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(150 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * temp * 2f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(150 + thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * temp * 2f);
                }
                skill_temp = Instantiate(skill, collision.transform);
                skill_temp.transform.SetParent(skill_temp.transform.parent);
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                _character.Runable = true;
                Destroy(skill_temp, 2f);
            }
        }
        else
        {
            if (collision.CompareTag("Boss") || collision.CompareTag("Monster"))
            {
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 2f);
            }

        }
    }
    IEnumerator DeleteCollider()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;
    }
    IEnumerator Delay()
    {
        _character.Runable = false;
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(Time.deltaTime);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
        _character.Runable = true;
    }

}
