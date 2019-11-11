using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSkill1_2 : MonoBehaviour
{
    public float heal_amount;
    public float magic_atttack;
    public GameObject thisparent;
    private GameObject parentOjbect;

    public GameObject enchant_effect;
    private GameObject enchant_temp;
    public GameObject explosion_effect;
    private GameObject explosion_temp;
    private Vector3 parent_position;

    public GameObject explostion_enchant_effect;
    private GameObject explostion_enchant_temp;
    public GameObject explosion_damage_effect;
    private GameObject explosion_damage_temp;

    // Start is called before the first frame update
    void Start()
    {
        if (this.gameObject.transform.parent.parent!=null)
        {
            heal_amount = this.gameObject.GetComponentInParent<CharacterStat>().MagicAttackPower * 0.2f;
            magic_atttack = this.gameObject.GetComponentInParent<CharacterStat>().MagicAttackPower;
            thisparent = this.gameObject.GetComponentInParent<CharacterStat>().gameObject;
            parentOjbect = this.transform.parent.gameObject;
            parent_position = parentOjbect.transform.position;
            parentOjbect.transform.SetParent(this.transform.parent.parent.parent.parent);
        }
        else
        {
            heal_amount = GameManage.instance.Character_other[777 + (int)transform.position.z].GetComponent<CharacterStat>().MagicAttackPower * 0.2f;
            magic_atttack = GameManage.instance.Character_other[777 + (int)transform.position.z].GetComponent<CharacterStat>().MagicAttackPower;
            thisparent = GameManage.instance.Character_other[777 + (int)transform.position.z].GetComponent<CharacterStat>().gameObject;
            parentOjbect = this.transform.parent.gameObject;
            parent_position = parentOjbect.transform.position;
            parentOjbect.transform.SetParent(this.transform.parent.parent);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("HealerTrigger"))
        {
            Debug.Log("Here");
            StartCoroutine(MakeHealingEffect());
        }

        if (collision.CompareTag("HealerDamageTrigger"))
        {
            Debug.Log("There");
            StartCoroutine(MakeExplosionEffect());
        }
    }

    IEnumerator MakeHealingEffect()
    {
        enchant_temp = Instantiate(enchant_effect, parent_position, Quaternion.Euler(-60, 0, 0));
        Destroy(enchant_temp, 2f);
        yield return new WaitForSeconds(0.5f);
        explosion_temp = Instantiate(explosion_effect, parent_position, Quaternion.identity);
        explosion_temp.transform.SetParent(this.transform);
        yield return new WaitForSeconds(0.5f);
        Destroy(parentOjbect);
        yield return 0;
    }

    IEnumerator MakeExplosionEffect()
    {
        explostion_enchant_temp = Instantiate(explostion_enchant_effect, parent_position, Quaternion.Euler(-60, 0, 0));
        Destroy(enchant_temp, 2f);
        yield return new WaitForSeconds(0.5f);
        explosion_damage_temp = Instantiate(explosion_damage_effect, parent_position, Quaternion.Euler(-90, 0, 0));
        explosion_damage_temp.transform.SetParent(this.transform);
        yield return new WaitForSeconds(0.5f);
        Destroy(parentOjbect);
        yield return 0;
    }
}
