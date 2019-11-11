using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterNormalAttack : MonoBehaviour
{
    private Transform thisparent;
    
    private const float movespeed = 16f;

    Vector2 Boss_pos;

    Vector2 vector_temp;

    Vector2 this_pos;
    
    // Start is called before the first frame update
    void Start()
    {   
        thisparent = this.gameObject.transform.parent;
        if(thisparent!=null)
            this.gameObject.transform.SetParent(thisparent.parent);
        Boss_pos = BossStatus.instance.transform.position;
        this_pos = this.transform.position;
        Destroy(this.gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        if(BossStatus.instance.HP<=0||GameManage.instance.num_char<=0)
        {
            Destroy(this.gameObject);
        }

        vector_temp = BossStatus.instance.transform.position - this.transform.position;

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
    }
}
