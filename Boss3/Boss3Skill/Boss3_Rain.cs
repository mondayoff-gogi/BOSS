using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Rain : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    private float multiple = 1.5f;
    private float damage_amount;
    void Start()
    {
        dir = new Vector2(1, 0);
        movespeed = UpLoadData.boss_level * 2 + 10f;  //12
        Destroy(this.gameObject, 3); //5초뒤에 없어짐
        damage_amount = BossStatus.instance.MagicAttackPower;
        //this.transform.SetParent(this.transform.parent.parent);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
        }
    }
}
