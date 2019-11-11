using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBurnMissile : MonoBehaviour
{
    private Transform thisparent;

    private const float mutiple = 2.5f;

    private Vector2 dir;

    private const float movespeed = 30f;

    private float BurnManaAmount=10f;

    public GameObject skill;
    private GameObject skill_temp;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(27);

        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);
        }
        else
        {
            this.transform.localScale *= 2.5f;
        }
        dir = BossStatus.instance.transform.position;
        dir.x = dir.x - this.transform.position.x;
        dir.y = dir.y - this.transform.position.y;

        dir.Normalize();

        Destroy(this.gameObject, 1.0f);
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {   
        if(collision.CompareTag("Boss"))
        {
            if(BossStatus.instance.SilentAble)
            {
                skill_temp = Instantiate(skill, this.transform);
                if (thisparent != null)
                {
                    skill_temp.transform.SetParent(thisparent.parent);
                    Destroy(skill_temp, 0.7f);

                    float temp = Random.Range(1.0f, 100.0f);

                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(300 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * mutiple, true, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(300 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * mutiple, true, thisparent.gameObject);
                    }
                    if (BossStatus.instance.MP > BurnManaAmount)
                        BossStatus.instance.MP -= BurnManaAmount;
                    else
                        BossStatus.instance.MP = 0;
                }
                else
                {
                    skill_temp.transform.SetParent(thisparent);
                    Destroy(skill_temp, 0.7f);
                }
                Destroy(this.gameObject);
            }
            else
            {
                skill_temp = Instantiate(skill, this.transform);
                if (thisparent != null)
                {
                    skill_temp.transform.SetParent(thisparent.parent);
                    Destroy(skill_temp, 0.7f);

                    float temp = Random.Range(1.0f, 100.0f);

                    if (temp <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        BossStatus.instance.BossGetCriticalDamage(100 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * mutiple, true, thisparent.gameObject);
                    }
                    else
                    {
                        BossStatus.instance.BossGetSkillDamage(100 + thisparent.gameObject.GetComponent<CharacterStat>().MagicAttackPower * mutiple, true, thisparent.gameObject);
                    }
                }
                else
                {
                    skill_temp.transform.SetParent(thisparent);
                    Destroy(skill_temp, 0.7f);
                }
                Destroy(this.gameObject);
            }
        }
    }
}
