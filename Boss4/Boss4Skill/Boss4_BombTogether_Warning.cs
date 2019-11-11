using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss4_BombTogether_Warning : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetRoot", 3f);
    }

    private void SetRoot()
    {
        this.transform.SetParent(BossStatus.instance.transform.parent);
    }

}
