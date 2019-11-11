using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_GiveDotDamage : MonoBehaviour
{
    public GameObject damage_effect;

    private GameObject player;
    private GameObject damage_temp;
    private GameObject water_background;
    private float time_count;
    private float temp = 0;
    private float water_color;
    private float damage_amount;
    private float multiple = 1.5f;
    // Start is called before the first frame update
    void Start()
    {
        player = this.transform.parent.gameObject;
        damage_amount = BossStatus.instance.PhysicalAttackPower;
        water_background = GameObject.FindGameObjectWithTag("WaterBackGround").gameObject;
    }

    // Update is called once per frame
    void Update()
    {

        water_color = water_background.GetComponent<SpriteRenderer>().color.r;
        
        temp += Time.deltaTime;
        if(temp >= 3f)
        {
            if (player.tag == "Player")
            {
                player.GetComponent<CharacterStat>().GetDamage(water_color * damage_amount * multiple, true);
                damage_temp = Instantiate(damage_effect, player.transform);

                Destroy(damage_temp, 0.5f);
                temp = 0;
            }
        }

        if(player.tag != "Player")
            Destroy(this.gameObject);

    }
}
