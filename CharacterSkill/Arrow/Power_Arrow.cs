using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Power_Arrow : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private const float movespeed = 20f;

    private float timer = 1f;

    public GameObject skill;
    private GameObject skill_temp;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(9);
        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            dir = SkillManager.instance.mouse_pos;
            timer = SkillManager.instance.timer_temp;
            StartCoroutine(Delay());
        }
        else
        {
            dir=NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
            timer = (this.transform.localScale.x - 1)*5;
        }
        dir.x = dir.x - this.transform.position.x;
        dir.y = dir.y - this.transform.position.y;
        dir.Normalize();

        Destroy(this.gameObject, 2.0f);
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
            SoundManager.instance.Play(26);
            skill_temp = Instantiate(skill, this.transform);
            skill_temp.transform.localScale*= (1 + timer / 5);
            if (thisparent != null)
            {
                skill_temp.transform.SetParent(this.transform.parent.parent);
            }
            else
                skill_temp.transform.SetParent(this.transform.parent);


            Destroy(skill_temp, 2f);
            //임팩트
            if (thisparent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(100 + (thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower) * 1.25f * (1 + timer / 2), true, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(100 + (thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower) * 1.25f * (1 + timer / 2), true, thisparent.gameObject);
                }
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                thisparent.GetComponent<Character_Control>().Runable = true;
            }
            Destroy(this.gameObject);
        }
        if (collision.CompareTag("Monster"))
        {
            SoundManager.instance.Play(26);
            skill_temp = Instantiate(skill, this.transform);
            skill_temp.transform.localScale *= (1 + timer / 5);
            if (thisparent != null)
            {
                skill_temp.transform.SetParent(this.transform.parent.parent);
            }
            else
            {
                skill_temp.transform.SetParent(this.transform.parent);
            }
            Destroy(skill_temp, 2f);
            if (thisparent != null)
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(100 + (thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower) * 1.25f * (1 + timer / 2));
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(100 + (thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower) * 1.25f * (1 + timer / 2));
                }
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                thisparent.GetComponent<Character_Control>().Runable = true;
            }
            Destroy(this.gameObject);
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
