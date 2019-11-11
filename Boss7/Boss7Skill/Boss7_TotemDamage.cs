using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_TotemDamage : MonoBehaviour
{
    public GameObject hit_effect;
    public GameObject blood_obj;

    private GameObject hit_temp;
    private GameObject blood_temp;
    private float damage_amount;

    private Collider2D[] player;
    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;

        StartCoroutine(GiveDamage());
    }



    IEnumerator GiveDamage()
    {
        while(true)
        {
            if (GameManage.instance.IsGameEnd)
                break;

            player = Physics2D.OverlapCircleAll(this.transform.position, 3f);

            for (int i = 0; i < player.Length; i++)
            {
                if (player[i].CompareTag("Player"))
                {
                    float temp = Random.Range(1.0f, 100.0f);
                    player[i].GetComponent<CharacterStat>().GetDamage(damage_amount * 0.1f, true);
                    hit_temp = Instantiate(hit_effect, player[i].transform);
                    Destroy(hit_temp, 1f);
                    if (temp <= 20)
                        blood_temp = Instantiate(blood_obj, player[i].transform.position, Quaternion.identity);
                }
            }
            yield return new WaitForSeconds(0.3f);
        }
        
    }
}
