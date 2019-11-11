using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NormalAttack : MonoBehaviour
{
    private float damage;


    private void Start()
    {
        damage = BossStatus.instance.PhysicalAttackPower;

        Destroy(this.gameObject, 0.1f);
    }

    private void OnTriggerEnter2D(Collider2D collision) //노말 공격에 맞은 경우
    {
        if(collision.tag == "Player")
        {
            collision.gameObject.GetComponent<CharacterStat>().GetDamage(damage, false);
            
            BossStatus.instance.MP += 5;  //보스 마나 채우기
            return;
        }
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Player")
            this.gameObject.GetComponent<PolygonCollider2D>().enabled = false;
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        this.gameObject.GetComponent<PolygonCollider2D>().enabled = true;
    }
}
