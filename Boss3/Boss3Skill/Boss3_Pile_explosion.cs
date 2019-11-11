using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss3_Pile_explosion : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(99999, false);
        }
    }
}
