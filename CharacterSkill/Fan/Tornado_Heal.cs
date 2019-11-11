using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tornado_Heal : MonoBehaviour
{
    private Collider2D[] _collider;

    private Transform thisparent;

    public GameObject trail;
    private GameObject trail_temp;

    private void Start()
    {
        SoundManager.instance.Play(0);

        thisparent = this.gameObject.transform.parent;

        this.transform.SetParent(this.transform.parent.parent);
        
        StartCoroutine(Heal());
    }
    

    IEnumerator Heal()
    {
        while(true)
        {
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 1f);
            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Player")
                {
                    if (Random.Range(1.0f, 100.0f) <= thisparent.GetComponent<CharacterStat>().ciritical)
                    {
                        _collider[i].GetComponent<CharacterStat>().GetCriticalHeal(thisparent.GetComponent<CharacterStat>().MagicAttackPower * 0.1f, thisparent.gameObject,true);
                    }
                    else
                    {
                        _collider[i].GetComponent<CharacterStat>().GetHeal(thisparent.GetComponent<CharacterStat>().MagicAttackPower * 0.1f, thisparent.gameObject,true);
                    }

                    if (_collider[i].GetComponent<Character_Control>().SpeedBuf(1.3f + thisparent.GetComponent<CharacterStat>().MagicAttackPower * 0.01f, 1f))
                    {
                        trail_temp = Instantiate(trail, _collider[i].transform);
                    }
                }
            }
            yield return new WaitForSeconds(0.2f);
        }
    }

}
