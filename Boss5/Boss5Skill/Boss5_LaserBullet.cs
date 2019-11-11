using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_LaserBullet : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    private Vector2 pos;
    private float damage_amount;
    void Start()
    {
        dir = new Vector2(1, 0);
        movespeed = 100f;
        this.transform.SetParent(this.transform.parent.parent);
        this.transform.localScale=new Vector3(4f, 4f, 4f);
        damage_amount = BossStatus.instance.MagicAttackPower;
        Destroy(this.gameObject, 0.5f);
    }

    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed/*, Space.World*/);
        if(this.transform.position.x>20)
        {
            pos = this.transform.position;
            pos.x = -20;
            this.transform.position = pos;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, true);
            Destroy(this.gameObject);
        }
    }
}
