using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_Volcano : MonoBehaviour
{
    private GameObject bubble;
    public float force;

    private GameObject bubble_temp;
    private float dir_x;
    private float dir_y;
    private Vector2 dir;
    private WaitForSeconds wait_time = new WaitForSeconds(30f/UpLoadData.boss_level+3);
    // Start is called before the first frame update
    private void Start()
    {
        bubble = BossStatus.instance.gameObject.GetComponent<Boss6Move>().skill_prefab[11];
        if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
        {
            StartCoroutine(Generate());

        }
        else if (NetworkManager.instance.is_multi && !NetworkManager.instance.is_host)
            return;
        else if(!NetworkManager.instance.is_multi && UpLoadData.boss_level >= 1)
            StartCoroutine(Generate());
    }
    // Update is called once per frame
    void Update()
    {
        if (GameManage.instance.IsGameEnd)
        {
            StopAllCoroutines();
        }
    }

    IEnumerator Generate()
    {
        yield return new WaitForSeconds(8f);

        while (true)
        {
            dir_x = Random.Range(-1.0f, 1f);
            dir_y = Random.Range(-1.0f, 1f);
            dir = new Vector2(dir_x, dir_y);
            dir.Normalize();
            bubble_temp = Instantiate(bubble, this.transform.position, Quaternion.identity);
            bubble_temp.GetComponent<Rigidbody2D>().AddForce(dir * force, ForceMode2D.Force);
            if (NetworkManager.instance.is_multi && NetworkManager.instance.is_host)
            {
                NetworkManager.instance.Boss6Bubble(11, dir,force);
            }
            yield return wait_time;
        }
    }
}
