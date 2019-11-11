using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_NormalAttack : MonoBehaviour
{
    public GameObject normalExplosion;
    private GameObject normalExplosion_temp;

    private Collider2D[] player;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(Eruption());
    }
    IEnumerator Eruption()
    {
        yield return new WaitForSeconds(2f);
        normalExplosion_temp = Instantiate(normalExplosion, this.transform.position, Quaternion.identity);
        SoundManager.instance.Play(47);
        BossStatus.instance.GetComponent<Boss6Move>().main.GetComponent<Camera_move>().VivrateForTime(0.5f);


        Destroy(normalExplosion_temp, 3f);
        player = Physics2D.OverlapCircleAll(this.transform.position, 1.5f);
        for (int i = 0; i < player.Length; i++)
        {
            if (player[i].CompareTag("Player"))
            {
                player[i].GetComponent<CharacterStat>().GetDamage(BossStatus.instance.PhysicalAttackPower, false);
            }
        }
        Destroy(this.gameObject);
    }

}
