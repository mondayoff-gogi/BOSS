using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill4 : MonoBehaviour
{
    private const float multiple = 3.3f;
    private GameObject player;
    private CircleCollider2D _col;

    private void Start()
    {
        Destroy(this.gameObject, 1f);
        _col = this.GetComponent<CircleCollider2D>();
        Invoke("DeActivateCol", 0.1f);
        SoundManager.instance.Play(32);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (this.transform.position.z>-774)
        {
            if (collision.CompareTag("Boss"))
            {
                // 마법 공격력 * 타이머 * 3
                if (Random.Range(1.0f, 100.0f) <= GameManage.instance.player[0].GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<BossStatus>().BossGetCriticalDamage((GameManage.instance.player[0].GetComponent<CharacterStat>().MagicAttackPower * multiple), true, GameManage.instance.player[0]);
                }
                else
                {
                    collision.GetComponent<BossStatus>().BossGetSkillDamage((GameManage.instance.player[0].GetComponent<CharacterStat>().MagicAttackPower * multiple), true, GameManage.instance.player[0]);
                }


            }
            if (collision.CompareTag("Monster"))
            {
                if (Random.Range(1.0f, 100.0f) <= GameManage.instance.player[0].GetComponent<CharacterStat>().ciritical)
                {
                    collision.GetComponent<Monster>().GetCritialDamage((GameManage.instance.player[0].GetComponent<CharacterStat>().MagicAttackPower * multiple));
                }
                else
                {
                    collision.GetComponent<Monster>().GetDamage(GameManage.instance.player[0].GetComponent<CharacterStat>().MagicAttackPower * multiple);
                }
            }
        }
    }
    public void DeActivateCol()
    {
        _col.enabled = false;
    }
}
