using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearSkill2_2 : MonoBehaviour
{
    private Vector3 mouse_pos;
    private Vector3 this_position;

    private Vector3 dir;

    private float dx;
    private float dy;

    // Start is called before the first frame update
    void Start()
    {
        Destroy(this.gameObject, 0.6f);
    }
}
