using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss8_Fog : MonoBehaviour
{
    public GameObject icon;

    private GameObject icon_temp;
    private Collider2D[] Players;
    private ParticleSystem ps;
    // Start is called before the first frame update
    void Start()
    {
        ps = this.GetComponent<ParticleSystem>();
        StartCoroutine(ChangeColor());
        StartCoroutine(SkillInit());
        //StartCoroutine(BossInvade());
    }
    IEnumerator ChangeColor()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);
        float alpha = 0;
        var main = ps.main;
        Color newColor;
        newColor = main.startColor.color;
        while (alpha<1)
        {
            newColor.a = alpha;
            main.startColor = newColor;
            alpha += 0.01f;
            yield return waittime;
        }
    }
    IEnumerator SkillInit()
    {
        WaitForSeconds waittime;
        waittime = new WaitForSeconds(0.01f);

        while(true)
        {
            Players = Physics2D.OverlapCircleAll(this.transform.position, 4f);
            for(int i=0;i<Players.Length;i++)
            {
                if(Players[i].CompareTag("Player"))
                {
                    Players[i].GetComponent<Character_Control>().Init_skill();
                }
            }
            yield return waittime;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.GetChild(3).gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.transform.GetChild(3).gameObject.SetActive(false);
        }
    }

    //IEnumerator BossInvade()
    //{
    //    WaitForSeconds waittime;
    //    waittime = new WaitForSeconds(0.01f);
    //    Collider2D[] Boss;
    //    while (true)
    //    {
    //        Boss = Physics2D.OverlapCircleAll(this.transform.position, 4f);
    //        for (int i = 0; i < Boss.Length; i++)
    //        {
    //            if (Boss[i].CompareTag("Boss"))
    //            {
    //                Boss[i].tag
    //            }
    //        }
    //        yield return waittime;
    //    }
    //}
}
