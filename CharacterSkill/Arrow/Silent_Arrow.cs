using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Silent_Arrow : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private const float movespeed = 10f;

    public GameObject skill;
    private GameObject skill_temp;
    // Start is called before the first frame update
    void Start()
    {
        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            dir = SkillManager.instance.mouse_pos;
            StartCoroutine(Delay());
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
        }
        dir.x = dir.x - this.transform.position.x;
        dir.y = dir.y - this.transform.position.y;

        dir.Normalize();

        Destroy(this.gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            if(BossStatus.instance.SilentAble) //silent맞으면
            {
                //임팩트
                SoundManager.instance.Play(18);
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 2f);                
                BossStatus.instance.BossGetSilent();
                if (thisparent != null)
                {
                    float temp = Random.Range(1.0f, 100.0f);
                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(500 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 2.5f, true, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(500 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 2.5f, true, thisparent.gameObject);
                    }

                    thisparent.GetComponent<CharacterStat>().GetMana(SkillManager.instance.skill_mana[7]);
                    thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                    thisparent.GetComponent<Character_Control>().Runable = true;
                }
                Destroy(this.gameObject);
            }
            else if(BossStatus.instance.GetComponent<Animator>().GetBool("Attack")|| BossStatus.instance.GetComponent<Animator>().GetBool("Magic")|| BossStatus.instance.GetComponent<Animator>().GetBool("Active"))
            {
                SoundManager.instance.Play(18);
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 2f);
                if (thisparent != null)
                {
                    float temp = Random.Range(1.0f, 100.0f);
                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(220 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1.5f, true, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(220 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1.5f, true, thisparent.gameObject);
                    }
                    thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                    thisparent.GetComponent<Character_Control>().Runable = true;
                }
                Destroy(this.gameObject);
            }
            else
            {
                SoundManager.instance.Play(18);
                skill_temp = Instantiate(skill, collision.transform);
                Destroy(skill_temp, 2f);
                if (thisparent != null)
                {
                    float temp = Random.Range(1.0f, 100.0f);
                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.5f, true, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.5f, true, thisparent.gameObject);
                    }
                    thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                    thisparent.GetComponent<Character_Control>().Runable = true;
                }
                Destroy(this.gameObject);
            }
        }
        if (collision.CompareTag("Monster"))
        {
            if (thisparent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1.5f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(120 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1.5f);
                }
            }
        }
    }
    IEnumerator Delay()
    {
        thisparent.GetComponent<Character_Control>().Runable = false;
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(Time.deltaTime);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
        thisparent.GetComponent<Character_Control>().Runable = true;
    }
}
