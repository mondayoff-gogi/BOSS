using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_FireSpread : MonoBehaviour
{
    private Collider2D[] _collider;
    private float damage_amount;
    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
        StartCoroutine(GetDamage());
    }

    IEnumerator GetDamage()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.5f);
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 1f);
            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Player")
                {
                    _collider[i].GetComponent<CharacterStat>().GetDamage(damage_amount, true);
                }
            }
        }
    }
}

    

