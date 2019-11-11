using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesertSkill_Swamp : MonoBehaviour
{

    public GameObject swamp_dust;
    public GameObject swamp;


    private const float out_radius = 2f;
    private float timer;
    private Vector3 vec3_dir;
    private Vector3 temp_vec;
    public float sand_speed;

    private const int out_dust_max = 10;

    BossStatus bossStat;

    private GameObject[] out_dust_array;

    // Start is called before the first frame update
    void Start()
    {
        timer = 0;

        out_dust_array = new GameObject[out_dust_max];

        bossStat = BossStatus.instance;

        StartCoroutine(Exec());
    }

    IEnumerator Exec()
    {
        while (true)
        {
            CreateSwamp(out_radius, out_dust_array);
            yield return new WaitForSeconds(1.0f);
        }
    }

    private void CreateSwamp(float radius, GameObject[] dust_array)
    {
        float x,y,z;
        z = 0f;

        float angle = 20f;
        for(int dustcount = 0; dustcount < dust_array.Length; dustcount++)
        {
            if(out_dust_array[dustcount] == null)
            {
                for(int i = 0; i < out_dust_max; i++)
                {
                    x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                    y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

                    angle += (360f / out_dust_max);
                    dust_array[dustcount] = Instantiate(swamp_dust, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
                    dust_array[dustcount].transform.SetParent(this.gameObject.transform);
                }
                return;
            }
        }
    }


    private void OnTriggerStay2D(Collider2D collision)
    {

        if (collision.gameObject.tag == "HolyKnightFourthSkill")
        {
            Magnitude(collision);
        }

        if (collision.gameObject.tag == "Wisp")
        {
            Magnitude(collision);
        }

        if (collision.gameObject.tag == "Player")
        {
            timer += Time.deltaTime;
            if (timer > 1f)
            {
                timer = 0;
                collision.GetComponent<CharacterStat>().GetDamage(bossStat.MagicAttackPower, true);
            }
            Magnitude(collision);
        }
    }

    private void Magnitude(Collider2D collision)
    {
        vec3_dir = this.transform.position - collision.transform.position;
        vec3_dir.z = 0;
        vec3_dir.Normalize();

        collision.transform.Translate(vec3_dir * sand_speed * Time.deltaTime);
    }

}
