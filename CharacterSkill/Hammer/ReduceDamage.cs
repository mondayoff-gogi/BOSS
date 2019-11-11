using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReduceDamage : MonoBehaviour
{
    private Transform thisparent;

    private float temp;
    private float temp_magic;

    private const float Bufftimer = 5f;

    private const float Armor_Buf = 0.25f;
    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(16);

        thisparent = this.gameObject.transform.parent;
        
        StartCoroutine(DelayAnimate());
        
    }

    IEnumerator DelayAnimate()
    {
        if (thisparent.parent != null)
        {
            temp = thisparent.GetComponent<CharacterStat>().PhysicalArmor * Armor_Buf;
            temp_magic = thisparent.GetComponent<CharacterStat>().MagicArmorPower * Armor_Buf;

            thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
            thisparent.GetComponent<Character_Control>().Runable = false;
            yield return new WaitForSeconds(0.1f);
            thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
            yield return new WaitForSeconds(0.7f);
            thisparent.GetComponent<Character_Control>().Runable = true;
            thisparent.GetComponent<CharacterStat>().PhysicalArmor += temp;
            thisparent.GetComponent<CharacterStat>().MagicArmorPower += temp_magic;
        }

        yield return new WaitForSeconds(5f);

        if (thisparent.parent != null)
        {
            thisparent.GetComponent<CharacterStat>().PhysicalArmor -= temp;
            thisparent.GetComponent<CharacterStat>().MagicArmorPower -= temp_magic;
        }
        Destroy(this.gameObject);

    }

   
}
