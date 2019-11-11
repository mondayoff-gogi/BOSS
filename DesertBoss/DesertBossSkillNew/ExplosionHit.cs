using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosionHit : MonoBehaviour
{
    private void Start()
    {
        Invoke("DisableCollider", 0.2f);
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {    
            // 최대 체력의 1/2만큼의 피해량
            collision.GetComponent<CharacterStat>().GetDamage(collision.GetComponent<CharacterStat>().MaxHP / 2f, false);
        }
    }

    private void DisableCollider()
    {
        this.GetComponent<BoxCollider2D>().enabled = false;
    }
}
