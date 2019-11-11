using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManaBurn : MonoBehaviour
{
    private Transform thisparent;

    private Vector2 dir;

    private const float movespeed = 2f;

    public GameObject skill;
    private GameObject skill_temp;

    Collider2D[] _collider;
    // Start is called before the first frame update

    void Start()
    {
        SoundManager.instance.Play(12);

        thisparent = this.gameObject.transform.parent;

        if(thisparent!=null)
        {
            this.gameObject.transform.SetParent(thisparent.parent);

            dir = SkillManager.instance.mouse_pos;

            StartCoroutine(MagicMotion());
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
        }
        dir.x = dir.x - this.transform.position.x;
        dir.y = dir.y - this.transform.position.y;
        dir.Normalize();

        StartCoroutine(ManaBurnAttack());

        Destroy(this.gameObject, 10.0f);

    }
    IEnumerator ManaBurnAttack()
    {
        int count = 0;
        while(true)
        {
            _collider = Physics2D.OverlapCircleAll(this.transform.position, 6f);

            for (int i = 0; i < _collider.Length; i++)
            {
                if (_collider[i].tag == "Boss")
                {
                    if (BossStatus.instance.SilentAble) //silentable 일때  
                    {
                        skill_temp = Instantiate(skill, thisparent);
                        skill_temp.transform.position = this.transform.position;
                    }
                    else
                    {
                        if(count++%3==0)
                        {
                            skill_temp = Instantiate(skill, thisparent);
                            skill_temp.transform.position = this.transform.position;
                        }                       
                    }
                }
            }
            yield return new WaitForSeconds(0.5f);
        }
    }
    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * Time.deltaTime * movespeed, Space.World);
    }
    IEnumerator MagicMotion()
    {
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", true);
        yield return new WaitForSeconds(0.1f);
        thisparent.GetComponent<Character_Control>()._animator.SetBool("IsMagic", false);
    }
}
