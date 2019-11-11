using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pillar : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (NetworkManager.instance.is_multi)
            this.transform.SetParent(BossStatus.instance.gameObject.transform);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
