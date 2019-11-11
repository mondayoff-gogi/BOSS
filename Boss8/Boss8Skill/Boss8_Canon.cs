using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_Canon : MonoBehaviour
{
    public GameObject explosionEffect;
    private GameObject explosionEffect_temp;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 2f);

        Collider2D[] players;
        players = Physics2D.OverlapCircleAll(this.transform.position, 4f);
        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].CompareTag("Player"))
            {
                players[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower, true);
                ExplosionEffect(players[i]);
            }
        }
    }
    private void ExplosionEffect(Collider2D player)
    {
        explosionEffect_temp = Instantiate(explosionEffect, player.gameObject.transform.position, Quaternion.identity);
        explosionEffect_temp.transform.localScale = new Vector2(2f, 2f);
        Destroy(explosionEffect_temp, 2f);
    }
}
