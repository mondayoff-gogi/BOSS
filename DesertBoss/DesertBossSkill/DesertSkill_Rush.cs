using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Rush : MonoBehaviour
{
    private Vector2 temp;

    Collider2D[] _collider;

    private float damage_amount;
    private float multiple = 0.1f;

    private void Start()
    {
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        StartCoroutine(Rush());
    }
    IEnumerator Rush()
    {
        while(true)
        {
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 2.5f);
            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Player")
                {
                    temp = this.transform.position - _collider[i].transform.position;
                    temp.Normalize();
                    _collider[i].transform.Translate(temp / 16);
                    _collider[i].GetComponent<CharacterStat>().GetDamage(damage_amount * multiple, false);
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }
}
