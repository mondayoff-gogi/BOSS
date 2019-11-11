using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_BloodPoolMove : MonoBehaviour
{
    public GameObject damage_object;

    private Vector2 dir;
    private GameObject target_player;
    private float speed = 0;

    private void Start()
    {
        Destroy(this.gameObject, 10f);
    }

    // Update is called once per frame
    void Update()
    {
        dir = target_player.transform.position - this.transform.position;
        dir.Normalize();

        speed += Time.deltaTime;
        if (speed >= Mathf.PI / 2)
            speed = 0;
        this.transform.Translate(dir * Mathf.Sin(speed)* 2f * Time.deltaTime);
    }
    public void GetTargetPlayer(GameObject player)
    {
        target_player = player;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            Instantiate(damage_object, collision.transform);
            Destroy(this.gameObject);
        }
    }
}
