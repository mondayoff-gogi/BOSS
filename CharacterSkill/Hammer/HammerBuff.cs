using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HammerBuff : MonoBehaviour
{
    private Transform thisparent;

    private float temp;
    private float attack;
    private float magic_attack;
    private float hp_temp;
    // Start is called before the first frame update
    void Start()
    {
        thisparent = this.gameObject.transform.parent;

        StartCoroutine(HammerBuf());        
    }
    IEnumerator HammerBuf()
    {
        if(thisparent.CompareTag("Player"))
        {
            temp = thisparent.GetComponent<CharacterStat>().MaxHP * 0.5f;

            thisparent.GetComponent<CharacterStat>().MaxHP += temp;
            thisparent.GetComponent<CharacterStat>().HP += temp;

            attack = thisparent.GetComponent<CharacterStat>().PhysicalAttackPower * 0.5f;
            magic_attack = thisparent.GetComponent<CharacterStat>().MagicAttackPower * 0.5f;

            thisparent.GetComponent<CharacterStat>().PhysicalAttackPower += attack;
            thisparent.GetComponent<CharacterStat>().MagicAttackPower += magic_attack;
        }
        yield return new WaitForSeconds(5f);

        if (thisparent.CompareTag("Player"))
        {
            hp_temp = thisparent.GetComponent<CharacterStat>().HP / thisparent.GetComponent<CharacterStat>().MaxHP;


            thisparent.GetComponent<CharacterStat>().MaxHP -= temp;
            thisparent.GetComponent<CharacterStat>().HP = hp_temp * thisparent.GetComponent<CharacterStat>().MaxHP;

            thisparent.GetComponent<CharacterStat>().PhysicalAttackPower -= attack;
            thisparent.GetComponent<CharacterStat>().MagicAttackPower -= magic_attack;
        }
        Destroy(this.gameObject);
    }
    
}
