using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_Fire : MonoBehaviour
{
    private GameObject[] player;
    private int player_target;
    private Vector2 dir;

    public GameObject prefab;
    private GameObject prefab_temp;

    public GameObject destroyEffect;
    private GameObject destroyEffect_temp;



    private WaitForSeconds waittime;

    private const float movespeed = 0.8f;
    private const float SpreadPeriod = 3f;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player");

        player_target =  Random.Range(0, player.Length);
               
        waittime = new WaitForSeconds(0.01f);
    }


    public void StartCor()
    {
        StartCoroutine(Spread());
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManage.instance.num_char<=0)
        {
            Destroy(this.gameObject);
        }
        dir = BossStatus.instance.target_player.transform.position - this.transform.position;

        dir.Normalize();
        //if(player[player_target].GetComponent<CharacterStat>())
        //{
        //    if (player[player_target].GetComponent<CharacterStat>().HP <= 0)
        //    {
        //        player = GameObject.FindGameObjectsWithTag("Player");
        //        player_target = Random.Range(0, player.Length);
        //    }
        //}
        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }

    public void DestoryEffect()
    {
        destroyEffect_temp = Instantiate(destroyEffect, this.transform);
        destroyEffect_temp.transform.SetParent(this.transform.parent);
        Destroy(destroyEffect_temp, 2f);
        this.transform.position = new Vector2(100, 100);
        this.GetComponent<Monster>().HP = this.GetComponent<Monster>().MaxHP;
        this.gameObject.SetActive(false);
    }

    IEnumerator Spread()
    {
        while(true)
        {
            yield return new WaitForSeconds(SpreadPeriod);
            if (GameManage.instance.IsGameEnd) Destroy(this.gameObject);
            prefab_temp = Instantiate(prefab, this.transform);
            prefab_temp.transform.SetParent(this.transform.parent);
        }
    }
}
