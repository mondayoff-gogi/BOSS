using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_BatMove : MonoBehaviour
{
    public GameObject destroy_effect;
    public GameObject blood;

    private GameObject destroy_temp;
    private float damage_amount;
    private Vector2 dir;
    private GameObject boss;
    private float speed = 4f;
    private float multiple = 0.02f;
    private float DamageGive;

    //private bool flag;

    private void Start()
    {
        boss = BossStatus.instance.gameObject;
        damage_amount = BossStatus.instance.MagicAttackPower;
        dir = boss.transform.position - this.transform.position;
        dir.Normalize();

    }

    private void Update()
    {
        if (GameManage.instance.IsGameEnd)
        {
            Destroy(this.gameObject);
        }
        speed += 0.02f;
        this.transform.Translate(dir * speed * Time.deltaTime);
        if(Vector2.Distance(this.transform.position, boss.transform.position) <= 0.3f)
        {
            Destroy(this.gameObject);
        }

    }


    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            float percent = Random.Range(0f, 100.0f);
            if (percent <= 10f)
            {
                Instantiate(blood, this.transform.position, Quaternion.identity);
            }
            DamageGive=collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
            boss.GetComponent<BossStatus>().BossGetHeal(DamageGive);
            destroy_temp = Instantiate(destroy_effect, collision.transform);
            Destroy(destroy_temp, 0.5f);
        }
    }

    //public void GetFlag()
    //{
    //    flag = true;
    //}
}
