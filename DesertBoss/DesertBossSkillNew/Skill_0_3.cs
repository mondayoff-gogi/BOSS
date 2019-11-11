using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill_0_3 : MonoBehaviour
{
    private Vector3 dir;
    public float physical_attack;

    // Start is called before the first frame update
    void Start()
    {
        physical_attack = this.transform.parent.gameObject.GetComponent<BossStatus>().PhysicalAttackPower;
        dir = this.transform.parent.GetComponent<DesertBossMove_koki>().vector3_throw_attack;
        this.transform.SetParent(this.transform.parent.parent);

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Translate(dir * 10f * Time.deltaTime,Space.World);
    }
}
