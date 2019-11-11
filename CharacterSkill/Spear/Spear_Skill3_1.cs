using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spear_Skill3_1 : MonoBehaviour
{
    private GameObject this_parent;
    public float add_attack_speed;
    public float percent = 0.1f;
    // Start is called before the first frame update
    void Start()
    {   
        this_parent = this.transform.parent.gameObject;
        if(this_parent.transform.parent!=null)
        {
            this_parent.GetComponent<CharacterStat>().Attack_speed += add_attack_speed;
            Invoke("ResetAttackSpeed", 4.99f);
        }
        Destroy(this.gameObject, 5f);
    }

    public void ResetAttackSpeed()
    {
        this_parent.GetComponent<CharacterStat>().Attack_speed -= add_attack_speed;
    }

}
