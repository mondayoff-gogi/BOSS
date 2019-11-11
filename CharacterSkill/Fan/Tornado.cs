using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado : MonoBehaviour
{
    private Transform thisparent; //케릭터

    private Vector2 dir;

    private const float movespeed = 3f;

    public GameObject skill;
    private GameObject skill_temp;
    public GameObject trail;
    private GameObject trail_temp;
    // Start is called before the first frame update
    void Start()
    {
        thisparent = this.gameObject.transform.parent;
        if (thisparent != null)
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

        StartCoroutine(Generator());
        trail_temp = Instantiate(trail, this.transform);
        Destroy(trail_temp, 2f);
        Destroy(this.gameObject, 3.0f);
    }
    IEnumerator Generator()
    {
        while(true)
        {
            if (thisparent != null)
            {
                skill_temp = Instantiate(skill, thisparent);
            }
            else
                skill_temp = Instantiate(skill, GameManage.instance.Character_other[777+(int)this.transform.position.z].transform);
            skill_temp.transform.position = this.transform.position;
            Destroy(skill_temp, 3f);
            yield return new WaitForSeconds(0.5f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (thisparent != null)
        {
            if (collision.CompareTag("Boss"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    BossStatus.instance.BossGetCriticalDamage(30 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1f, true, thisparent.gameObject);
                }
                else
                {
                    BossStatus.instance.BossGetSkillDamage(30 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1f, true, thisparent.gameObject);
                }
            }
            if (collision.CompareTag("Monster"))
            {
                float temp = Random.Range(1.0f, 100.0f);

                if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage(30 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1f);
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(30 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * 1f);
                }
            }
        }
    }
    IEnumerator Delay()
    {
        thisparent.GetComponent<Character_Control>().Runable = false;
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.1f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
        thisparent.GetComponent<Character_Control>().Runable = true;
    }
}
