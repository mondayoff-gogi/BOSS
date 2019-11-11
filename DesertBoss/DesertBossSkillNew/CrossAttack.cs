using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossAttack : MonoBehaviour
{
    private float multiple = 1f;
    private Vector2 dir;
    private float movespeed;
    private float damage_amount;
    
    void Start()
    {
        dir = new Vector2(1, 0);
        movespeed = 15f;
        Destroy(this.gameObject, 5); //5초뒤에 없어짐
        damage_amount = BossStatus.instance.MagicAttackPower;
        if (NetworkManager.instance.is_multi)
            this.transform.SetParent(BossStatus.instance.gameObject.transform);
        else
            this.transform.SetParent(this.transform.parent.parent);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir*Time.deltaTime* movespeed);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
        }
    }
}
