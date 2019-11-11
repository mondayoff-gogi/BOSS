using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss5_Mine : MonoBehaviour
{
    private PolygonCollider2D _collider;
    private Vector2 dir;
    private Vector2 temp;
    public GameObject body;
    private GameObject boss;

    public GameObject bomb_effect;
    private GameObject bomb_temp;
    // Start is called before the first frame update
    void Start()
    {
        _collider = this.GetComponent<PolygonCollider2D>();
        temp = this.transform.position;
        dir = this.transform.position;
        dir.y += 3f;
        StartCoroutine(EmptyAlpha());
        boss = BossStatus.instance.gameObject;

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            _collider.enabled = false;
            StartCoroutine(TrapEffect());
        }

    }

    IEnumerator TrapEffect()
    {
        StartCoroutine(Twist());
        yield return new WaitForSeconds(2f);
    }

    IEnumerator Twist()
    {
        int count = 0;
        StartCoroutine(FullAlpha());
        SoundManager.instance.Play(51);
        while (count <= 10)
        {
            float temp = Random.Range(-30.0f, 0f);
            this.transform.rotation = Quaternion.Euler(0, 0, temp);
            yield return new WaitForSeconds(0.03f);
            this.transform.rotation = Quaternion.Euler(0, 0, -temp);
            yield return new WaitForSeconds(0.03f);
            count++;
        }
        this.transform.rotation = Quaternion.Euler(0, 0, 0);

        yield return 0;
    }

    IEnumerator FullAlpha()
    {
        Color color = body.GetComponent<SpriteRenderer>().color;
        while (color.a <= 1)
        {
            color.a += 0.03f;
            body.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.01f);
        }
        float t = 0;
        float y = 1f;
        while (t <= 1)
        {
            color = new Color(t, y, y, 1f);
            body.GetComponent<SpriteRenderer>().color = color;
            t += 0.05f;
            y -= 0.05f;
            yield return new WaitForSeconds(0.01f);
        }
        StartCoroutine(GenerateBombs());
        yield return 0;
    }

    IEnumerator GenerateBombs()
    {
        int radius = 1;
        float x, y, z;
        z = 0f;
        float angle = 20f;
        Color color1 = this.GetComponent<SpriteRenderer>().color;
        color1.a = 0;
        this.GetComponent<SpriteRenderer>().color = color1;
        Color color = body.GetComponent<SpriteRenderer>().color;
        color.a = 0;
        body.GetComponent<SpriteRenderer>().color = color;
        for (int i = 0; i < 6; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 6f);
            bomb_temp = Instantiate(bomb_effect, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
            Destroy(bomb_temp, 1f);
        }
        yield return new WaitForSeconds(0.5f);

        radius += 2;
        for (int i = 0; i < 6; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 6f);
            bomb_temp = Instantiate(bomb_effect, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
            bomb_temp.transform.localScale = new Vector3(0.6f, 0.6f, 0.6f);
            Destroy(bomb_temp, 1f);
        }
        yield return new WaitForSeconds(0.5f);

        radius += 1;
        for (int i = 0; i < 6; i++)
        {
            x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            y = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;

            angle += (360f / 6f);
            bomb_temp = Instantiate(bomb_effect, new Vector3(x + this.gameObject.transform.position.x, y + this.gameObject.transform.position.y, z), Quaternion.identity) as GameObject;
            bomb_temp.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);
            Destroy(bomb_temp, 1f);
        }
        Destroy(this.gameObject);
    }

    IEnumerator EmptyAlpha()
    {
        Color color = body.GetComponent<SpriteRenderer>().color;
        while (color.a >= 0)
        {
            color.a -= 0.03f;
            body.GetComponent<SpriteRenderer>().color = color;
            yield return new WaitForSeconds(0.001f);
        }
        yield return 0;
    }
}
