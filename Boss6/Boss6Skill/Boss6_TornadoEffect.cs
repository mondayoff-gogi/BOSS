using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_TornadoEffect : MonoBehaviour
{
    private void Start()
    {
        SoundManager.instance.Play(45);
        StartCoroutine(Delcollider());
        if (BossStatus.instance == null) Destroy(this.gameObject);
        BossStatus.instance.GetComponent<Boss6Move>().main.GetComponent<Camera_move>().VivrateForTime(1f);
    }
    IEnumerator Delcollider()
    {
        yield return new WaitForSeconds(0.1f);
        this.GetComponent<Collider2D>().enabled = false;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            collision.GetComponent<CharacterStat>().GetDamage(BossStatus.instance.MagicAttackPower, true);
        }
    }
}
