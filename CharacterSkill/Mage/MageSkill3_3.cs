using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MageSkill3_3 : MonoBehaviour
{
    private Vector3 mouse_pos;
    private Vector3 character_position;

    private Vector3 dir;

    private float dx;
    private float dy;

    public Transform player;

    private float rotate_degree;

    // Start is called before the first frame update
    void Start()
    {
        if(this.transform.parent!=null)
        {
            player = this.transform.parent.gameObject.transform;

            mouse_pos = SkillManager.instance.mouse_pos;
            mouse_pos.z = this.transform.position.z;

            character_position = SkillManager.instance.temp;

            dx = mouse_pos.x - character_position.x;
            dy = mouse_pos.y - character_position.y;

            dir = (mouse_pos - this.transform.position).normalized;
        }
        else
        {
            player = GameManage.instance.Character_other[777 + (int)this.transform.position.z].transform;

            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();

            dx = dir.x;
            dy = dir.y;
        }
        rotate_degree = Mathf.Atan2(dy, dx) * Mathf.Rad2Deg;

        if (dx > 0)
        {
            if (dy > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90 + rotate_degree);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, -90 + rotate_degree);
            }
        }
        else
        {
            if (dy > 0)
            {
                transform.rotation = Quaternion.Euler(0, 0, -90 + rotate_degree);
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, -90 + rotate_degree);
            }
        }

    }
}
