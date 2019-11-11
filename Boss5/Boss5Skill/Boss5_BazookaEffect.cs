using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_BazookaEffect : MonoBehaviour
{
    Collider2D[] players;
    // Start is called before the first frame update
    void Start()
    {
        BossStatus.instance.GetComponent<Boss5Move>().main.GetComponent<Camera_move>().VivrateForTime(1f);
        SoundManager.instance.Play(28);

        Destroy(this.gameObject, 2f);


        players = Physics2D.OverlapCircleAll(this.gameObject.transform.position, 1f);

        for(int i=0;i<players.Length;i++)
        {
            if (players[i].CompareTag("Player"))
                players[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower, false);
        }
    }

}
