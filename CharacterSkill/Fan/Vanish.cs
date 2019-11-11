using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Vanish : MonoBehaviour
{
    private Transform thisparent; //보스일수도 케릭터일수도

    private Vector2 dir;

    private const float movespeed=10f;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(11);

        thisparent = this.gameObject.transform.parent;
        //this.gameObject.transform.SetParent(thisparent.parent);
        

        if (thisparent.CompareTag("Boss"))
        {
            BossStatus.instance.GetBuff(3);
        }
        Destroy(this.gameObject, SkillManager.instance.buff_timer[3]);

    }
    private void Update()
    {
        if(thisparent.CompareTag("Boss"))
        {
            dir.x = this.transform.position.x - thisparent.position.x;
            dir.y = this.transform.position.y-4 - thisparent.position.y;
        }
        else
        {
            dir.x = this.transform.position.x - thisparent.position.x;
            dir.y = this.transform.position.y-2 - thisparent.position.y;
        }
  

        this.transform.Translate(-dir * movespeed*Time.deltaTime);
    }
    
}
