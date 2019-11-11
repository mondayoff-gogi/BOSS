using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNormalAttacktoMonster : MonoBehaviour
{
    private Transform thisparent;
    
    private const float movespeed = 8f;

    GameObject Monster;

    Vector2 vector_temp;

    // Start is called before the first frame update
    void Start()
    {
        thisparent = this.gameObject.transform.parent;
        if (thisparent != null)
            this.gameObject.transform.SetParent(thisparent.parent);
        Monster = thisparent.GetComponent<Character_Control>().monster_col.gameObject;
        Destroy(this.gameObject,0.5f);
    }

    // Update is called once per frame
    void Update()
    {
        if(BossStatus.instance.HP<=0||GameManage.instance.num_char<=0)
        {
            Destroy(this.gameObject);
        }

        vector_temp = Monster.transform.position - this.transform.position;
        
        vector_temp.Normalize();

        this.transform.Translate(vector_temp * Time.deltaTime * movespeed, Space.World);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Boss"))
        {
            if (thisparent != null)
            {
                thisparent.GetComponent<CharacterStat>().GetMana(50f);
                BossStatus.instance.BossGetDamage(thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.3f, false, thisparent.gameObject);
            }            
            Destroy(this.gameObject, 0.2f);
        }
        if (collision.CompareTag("Monster"))
        {
            if (thisparent != null)
            {
                thisparent.GetComponent<CharacterStat>().GetMana(50f);
                Monster.GetComponent<Monster>().GetDamage(thisparent.gameObject.GetComponent<CharacterStat>().PhysicalAttackPower * 0.3f);
            }
            Destroy(this.gameObject, 0.2f);
        }

    }
}
