using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_BloodPoolDamage : MonoBehaviour
{
    public GameObject hit_effect;
    public GameObject blood;

    private GameObject blood_temp;
    private GameObject hit_effect_temp;
    private float damage_amount;
    private Color player_color;
    private Color temp_color;
    private GameObject player;
    private int count = 0;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.gameObject;
        damage_amount = BossStatus.instance.MagicAttackPower;

        player_color = new Color(1f, 1f, 1f, 1f);

    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;

        if (!player.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }

        if (time >= 2f)
        {
            hit_effect_temp = Instantiate(hit_effect, player.transform);
            Destroy(hit_effect_temp, 0.5f);

            player.GetComponent<CharacterStat>().GetDamage(damage_amount * 0.5f, true);
            StartCoroutine(ChangeColor());
            float percent = Random.Range(0f, 100.0f);
            if(percent <= 20)
            {
                blood_temp = Instantiate(blood, this.transform.position, Quaternion.identity);
            }
            count++;
            time = 0;
        }

        if(count == 3)
        {
            Destroy(this.gameObject);
        }
    }

    IEnumerator ChangeColor()
    {
        temp_color = player.GetComponent<SpriteRenderer>().color;

        while (temp_color.b <=0.5)
        {
            temp_color.b -= 0.1f;
            temp_color.g -= 0.1f;
            player.GetComponent<SpriteRenderer>().color = temp_color;
            yield return new WaitForSeconds(0.03f);
        }
        player.GetComponent<SpriteRenderer>().color = player_color;

        yield return 0;
    }
}
