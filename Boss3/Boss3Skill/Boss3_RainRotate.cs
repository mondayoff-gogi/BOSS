using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_RainRotate : MonoBehaviour
{
    private Vector2 dir;
    private float movespeed;
    private GameObject Boss;
    private float timer;
    private float farawaySpeed;
    private float damage_amount;
    void Start()
    {
        timer = 0;
        Boss = BossStatus.instance.gameObject;
        dir = new Vector2(1, 0);
        movespeed = 0f;
        farawaySpeed = 100f;
        Destroy(this.gameObject, 10); //15초뒤에 없어짐
        //this.transform.SetParent(this.transform.parent.parent);
        this.transform.SetParent(Boss.transform);
        damage_amount = BossStatus.instance.MagicAttackPower;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, farawaySpeed * Time.deltaTime);


        this.transform.Translate(dir * Time.deltaTime * movespeed);
        movespeed += Time.deltaTime*3;
        timer += Time.deltaTime;
        if (timer>3)
        {
            farawaySpeed += 30*Time.deltaTime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount, true);
        }
    }
}
