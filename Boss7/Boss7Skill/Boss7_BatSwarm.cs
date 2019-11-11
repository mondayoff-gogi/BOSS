using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss7_BatSwarm : MonoBehaviour
{
    public GameObject[] bats;
    private WaitForSeconds temp = new WaitForSeconds(0.1f);
    private float speed = 3f;
    private float time = 0;
    private Vector2 _dir = new Vector2();
    private float x, y;
    private float angle = 20f;
    private float radius = 3f;
    private float flag = 0;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(TurnOnBats());
    }

    // Update is called once per frame
    void Update()
    {
        flag += Time.deltaTime;
        speed += Time.deltaTime * Mathf.Cos(flag) * 10f;
        this.transform.Rotate(0, 0, Time.deltaTime * 100f);
    }

    IEnumerator TurnOnBats()
    {
        for(int i = 0; i < bats.Length; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 10f);
            bats[i].SetActive(true);
            bats[i].transform.localPosition = new Vector2(x, y);
            yield return temp;
        }

        yield return temp;
        while (time < 6.5f)
        {
            time += Time.deltaTime;
            this.transform.Translate(_dir * speed * Time.deltaTime, Space.World);
            yield return null;
        }
        time = 0;
        yield return temp;

        Destroy(this.gameObject, 0.5f);
        yield return 0;
    }

    public void GetDir(Vector2 dir)
    {
        _dir = dir;
    }
}
