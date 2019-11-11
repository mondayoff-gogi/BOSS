using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_SameDir : MonoBehaviour
{
    private GameObject boss;
    private float existing_time;
    public bool is_triggered = false;
    private Color now_color;

    public GameObject explosion_effect;
    private GameObject explosion_effect_temp;
    private float speed = 1f;
    private Vector3 dir_temp = new Vector3(0,0,0);
    private Vector2 temp;
    // Start is called before the first frame update
    void Start()
    {
        boss = GameObject.FindGameObjectWithTag("Boss");
    }
    private void Update()
    {
        temp = this.transform.position;
        existing_time += Time.deltaTime;
        now_color = this.GetComponent<SpriteRenderer>().color;

        Move(dir_temp);
        if(existing_time > 5.5f)
        {
            speed = 0;
        }
        if(existing_time > 8.5f)
        {
            explosion_effect_temp = Instantiate(explosion_effect, temp, Quaternion.identity);
            Destroy(explosion_effect_temp, 0.5f);
            Destroy(this.gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            is_triggered = true;
            this.GetComponent<SpriteRenderer>().color = new Color(255, 0, 0, 255);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            is_triggered = false;
            this.GetComponent<SpriteRenderer>().color = new Color(255, 255, 255, 255);

        }
    }

    public void Move(Vector3 dir)
    {
        dir_temp = dir;
        this.transform.Translate(dir_temp * speed * Time.deltaTime);
    }
}
