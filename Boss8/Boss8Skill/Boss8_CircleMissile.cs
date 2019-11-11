using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_CircleMissile : MonoBehaviour
{
    public GameObject[] missile_temp;

    private float speed = 1.5f;
    private Vector2 _dir = new Vector2();
    private float x, y;
    private float angle = 20f;
    private float radius = 3f;
    private float flag = 0;
    private float direction = 0;
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < 10; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 10f);
            missile_temp[i].SetActive(true);
            missile_temp[i].transform.localPosition = new Vector2(x, y);
        }
    }

    // Update is called once per frame
    void Update()
    {
        direction += Time.deltaTime * 4f;
        _dir = new Vector2(_dir.x, Mathf.Sin(direction));
        flag += Time.deltaTime;
        speed += Time.deltaTime * Mathf.Cos(flag) * 10f;
        this.transform.Rotate(0, 0, Time.deltaTime * 100f);
        this.transform.Translate(_dir * speed * Time.deltaTime, Space.World);
    }

    public void GetDir(Vector2 dir)
    {
        _dir = dir;
    }
}
