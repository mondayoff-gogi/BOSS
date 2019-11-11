using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkill2 : MonoBehaviour
{
    private Vector3 mouse_pos;
    private Vector3 character_position;

    private Vector3 dir;

    private float dx;
    private float dy;

    private float physical_attack;

    private float rotate_degree;

    public float multiple;
    public Vector2 temp;

    // Use this for initialization 
    void Start()
    {
        SoundManager.instance.Play(9);

        if (this.transform.parent != null)
        {
            mouse_pos = SkillManager.instance.mouse_pos;
            mouse_pos.z = this.transform.position.z;
            character_position = BossStatus.instance.player[0].transform.position;

            dx = mouse_pos.x - character_position.x;
            dy = mouse_pos.y - character_position.y;

            dir = (mouse_pos - this.transform.position).normalized;
            physical_attack = this.gameObject.GetComponentInParent<CharacterStat>().PhysicalAttackPower;
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();

            dx = dir.x;
            dy = dir.y;
        }
        rotate_degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;
        Destroy(this.gameObject, 1f);

        /*
        if (dx > 0)
        {
            if(dy > 0)
            {
                transform.rotation = Quaternion.Euler(-1 * rotate_degree, 90, -90);
            }
            else
            {
                transform.rotation = Quaternion.Euler(-1 * rotate_degree, 90, -90);
            }
        }
        else
        {
            if(dy > 0)
            {
                transform.rotation = Quaternion.Euler(180 + rotate_degree, -90, 90);
            }
            else
            {
                transform.rotation = Quaternion.Euler(180 + rotate_degree, -90, 90);
            }
        }*/
    }
}
