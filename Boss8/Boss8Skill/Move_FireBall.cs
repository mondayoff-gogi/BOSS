using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Move_FireBall : MonoBehaviour
{
    public GameObject explosion_effect;

    private Camera main;
    private GameObject explosion_temp;
    private float speed = 2.5f;
    private Vector2 dir;
    private float damage_amount;
    private float size = 0;
    private bool flag = false;
    private float time = 0;
    // Start is called before the first frame update
    void Start()
    {
        damage_amount = BossStatus.instance.MagicAttackPower;
        dir = Vector2.down;
        main = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        time += Time.deltaTime;
        while(true)
        {
            this.transform.localScale = new Vector3(size, size, size);
            size += 0.1f;
            if (size >= 7)
                break;
        }
        if(size >= 7)
        {
            flag = true;
        }
        if (flag)
        {
            this.transform.Translate(dir * speed * Time.deltaTime);
        }

        if(time >= 3)
        {
            main.GetComponent<Camera_move>().VivrateForTime(2f, 0.1f);
            explosion_temp = Instantiate(explosion_effect, this.transform.position, Quaternion.identity);
            Destroy(explosion_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }
}
