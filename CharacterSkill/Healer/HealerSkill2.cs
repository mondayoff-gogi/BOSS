using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealerSkill2 : MonoBehaviour
{
    private Vector3 mouse_pos;
    private Vector3 character_pos;

    private Vector2 dir;

    private float dx;
    private float dy;

    private float speed = 5f;

    private float rotate_degree;

    private GameObject parant_object;

    // Start is called before the first frame update
    void Start()
    {
        SoundManager.instance.Play(29);

        if(this.transform.parent!=null)
        {
            mouse_pos = SkillManager.instance.mouse_pos;
            mouse_pos.z = this.transform.position.z;
            character_pos = this.transform.parent.GetComponent<Transform>().position;

            dx = mouse_pos.x - character_pos.x;
            dy = mouse_pos.y - character_pos.y;

            dir = new Vector2(dx, dy);
            dir.Normalize();
            this.transform.SetParent(this.transform.parent.parent.parent);
        }
        else
        {
            dir = NetworkManager.instance.Skill_pos[777 + (int)this.transform.position.z];
            dir.x = dir.x - this.transform.position.x;
            dir.y = dir.y - this.transform.position.y;
            dir.Normalize();
        }
        Destroy(this.gameObject, 2f);


    }

    private void Update()
    {
        this.transform.Translate(dir * speed * Time.deltaTime, Space.World);
        speed -= 0.01f;
    }

}
