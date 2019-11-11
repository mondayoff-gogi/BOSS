using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkillPillar_Explosion : MonoBehaviour
{
    Collider2D[] _collider;

    private Vector2 temp;

    private void Start()
    {
        _collider = Physics2D.OverlapCircleAll(this.transform.position, 2f*3.7f);
        for (int i = 0; i < _collider.Length; i++)
        {
            if (_collider[i].tag == "Player")
            {
                _collider[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower*50f/100f, true);
            }
        }
    }
}
