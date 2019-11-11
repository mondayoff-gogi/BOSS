using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grren_Arrow : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;
    
    private const float movespeed = 15f;

    public GameObject skill;
    private GameObject skill_temp;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(21);
        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
            dir = SkillManager.instance.mouse_pos;
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();
            StartCoroutine(Delay());
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777+(int)this.transform.position.z];
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();
        }
        Destroy(this.gameObject, 3.0f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir*Time.deltaTime*movespeed,Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            
            SoundManager.instance.Play(1);
            skill_temp = Instantiate(skill, this.transform);
            Destroy(skill_temp, 2f);

            if (thisparent != null)
            {
                skill_temp.transform.SetParent(this.transform.parent.parent);

                                                                       //임팩트

                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.3f, true, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.3f, true, thisparent.gameObject);
                }
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                thisparent.GetComponent<Character_Control>().Runable = true;
                BossStatus.instance.GetBuff(2, thisparent.gameObject); //독
                Debug.Log(thisparent.gameObject.name);

            }
            else
                skill_temp.transform.SetParent(this.transform.parent);
            Destroy(this.gameObject);
        }
        if (collision.CompareTag("Monster"))
        {
            SoundManager.instance.Play(1);
            skill_temp = Instantiate(skill, this.transform);
            Destroy(skill_temp, 2f);

            if(thisparent != null)
            {
                skill_temp.transform.SetParent(this.transform.parent.parent);
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.3f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(40 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 0.3f);
                }
                thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
                thisparent.GetComponent<Character_Control>().Runable = true;
            }
            else
                skill_temp.transform.SetParent(this.transform.parent);

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
