using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class RotateWarning : MonoBehaviour
{
    private Vector2 dir;

    void Start()
    {
        if (NetworkManager.instance.is_multi)
            this.transform.SetParent(BossStatus.instance.gameObject.transform);
        else
            this.transform.SetParent(this.transform.parent.parent);

    }

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(0, 0, 0.3f);
    }

}
