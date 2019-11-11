using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateAttack : MonoBehaviour
{
    private float multiple = 0.1f;
    private float damage_amount;
    private Vector2 dir;
    void Start()
    {
        Destroy(this.gameObject, 4); //5초뒤에 없어짐
        if (NetworkManager.instance.is_multi)
        {
            //this.transform.SetParent(BossStatus.instance.gameObject.transform);
            //this.transform.SetParent(this.transform.parent.parent);
        }
        else
            this.transform.SetParent(this.transform.parent.parent);
        damage_amount = BossStatus.instance.MagicAttackPower;
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, 0.3f);
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, true);
        }
    }
}
