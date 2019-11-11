using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldSkill4 : MonoBehaviour
{
    private float dir = -5;
    private float shield_hp;
    private float character_hp;
    private float character_magical_attack;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 5f);
        character_hp = gameObject.transform.parent.GetComponent<CharacterStat>().MaxHP;
        character_magical_attack = gameObject.transform.parent.GetComponent<CharacterStat>().MagicAttackPower;
        // (최대 체력 * 0.15) *(마법 공격력 * 1.1)
        shield_hp = character_hp * 0.15f * (character_magical_attack * 1.1f);
    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(dir, 0, 0);
        if(shield_hp <= 0)
        {
            if (NetworkManager.instance.is_multi)
            {
                NetworkManager.instance.DeleteObject(1); //0번이 메이지2번
            }
            Destroy(this.gameObject);
        }
    }

    public void GetShieldDamage(float damage)
    {
        shield_hp -= damage;
    }
}
