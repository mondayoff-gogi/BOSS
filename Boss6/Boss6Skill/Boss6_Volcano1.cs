using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss6_Volcano1 : MonoBehaviour
{
    private Camera main;
    public GameObject spread_bubble;
    public GameObject temporary_bubble;
    public GameObject temporary_bubble_fast;
    public GameObject volcano_smoke;
    public GameObject givedamage_obj;
    public GameObject mystic_cloud;

    private GameObject givedamage_obj_temp;
    private GameObject background_water;
    private GameObject spread_bubble_temp;
    private GameObject temporary_bubble_temp;
    private GameObject temporary_bubble_fast_temp;
    private GameObject volcano_smoke_temp;
    private GameObject mystic_cloud_temp;


    private float r;
    private float b;

    private GameObject[] players;

    private Vector2 bubble_position;

    private WaitForSeconds wait_time = new WaitForSeconds(2f);

    // Start is called before the first frame update
    void Start()
    {
        main = Camera.main;
        StartCoroutine(VolcanoEffect());
        players = BossStatus.instance.player;
        background_water = GameObject.FindGameObjectWithTag("WaterBackGround");
    }

    IEnumerator VolcanoEffect()
    {
        yield return wait_time;
        main.GetComponent<Camera_move>().VivrateForTime(1f, 0.1f);
        spread_bubble_temp = Instantiate(spread_bubble, Vector2.zero, Quaternion.identity);

        SoundManager.instance.Play(52);
        for (int i = 0; i < 5; i++)
        {
            bubble_position = Random.insideUnitCircle * 1f;
            temporary_bubble_temp = Instantiate(temporary_bubble, bubble_position, Quaternion.identity);
            Destroy(temporary_bubble_temp, 1f);
        }

        yield return wait_time;
        main.GetComponent<Camera_move>().VivrateForTime(2f, 0.2f);
        SoundManager.instance.Play(52);

        for (int i = 0; i < 5; i++)
        {
            bubble_position = Random.insideUnitCircle * 2f;
            temporary_bubble_temp = Instantiate(temporary_bubble, bubble_position, Quaternion.identity);
            temporary_bubble_temp.transform.localScale = Vector3.one * 2f;
            Destroy(temporary_bubble_temp, 2f);
        }

        for (int i = 0; i < 5; i++)
        {
            bubble_position = Random.insideUnitCircle * 2.5f;
            temporary_bubble_fast_temp = Instantiate(temporary_bubble_fast, bubble_position, Quaternion.identity);
            Destroy(temporary_bubble_fast_temp, 2f);
        }

        yield return new WaitForSeconds(3f);
        SoundManager.instance.Play(52);

        main.GetComponent<Camera_move>().VivrateForTime(2f, 0.5f);
        mystic_cloud_temp = Instantiate(mystic_cloud, new Vector3(0f,0f,-10f), Quaternion.Euler(-60f,0f,0f));
        Destroy(mystic_cloud_temp, 3f);
        for (int i = 0; i < 10; i++)
        {
            bubble_position = Random.insideUnitCircle * 2.5f;
            temporary_bubble_fast_temp = Instantiate(temporary_bubble_fast, bubble_position, Quaternion.identity);
            temporary_bubble_fast_temp.transform.localScale = Vector3.one * 3f;
            Destroy(temporary_bubble_fast_temp, 2f);
        }

        volcano_smoke_temp = Instantiate(volcano_smoke, Vector2.zero, Quaternion.identity);
        Destroy(volcano_smoke_temp, 3f);

        int count;
        if (UpLoadData.boss_level < 2)
            count = 35;
        else
            count = 45;

        int r_count = 0;
        while (r_count <= count)
        {
            Color color = background_water.GetComponent<SpriteRenderer>().color;
            r = color.r;
            r += 0.01f;
            color.r = r;
            background_water.GetComponent<SpriteRenderer>().color = color;
            r_count++;
            yield return new WaitForSeconds(0.01f);
        }

        int b_count = 0;
        while (b_count <= count)
        {
            Color color = background_water.GetComponent<SpriteRenderer>().color;
            b = color.b;
            b -= 0.01f;
            color.b = b;
            background_water.GetComponent<SpriteRenderer>().color = color;
            b_count++;
            yield return new WaitForSeconds(0.01f);
        }

        for(int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponentInChildren<Boss6_GiveDotDamage>() == null)
            {
                givedamage_obj_temp = Instantiate(givedamage_obj, players[i].transform);
            }
            else
                continue;
        }

        Destroy(this.gameObject);
        yield return 0;
    }
}
