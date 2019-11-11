using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_RandomExplosion2 : MonoBehaviour
{
    public GameObject temp;
    private GameObject prefab;
    // Start is called before the first frame update
    void Start()
    {
        Invoke("SetActivate", 2f);
    }

    private void SetActivate()
    {
        SoundManager.instance.Play(28);
        if(BossStatus.instance)
            BossStatus.instance.GetComponent<Boss2Move>().main.GetComponent<Camera_move>().VivrateForTime(0.1f);

        prefab = Instantiate(temp, this.transform.position, Quaternion.identity);
        prefab.transform.localScale = this.transform.localScale*0.15f;
        Destroy(prefab, 1f);
        Destroy(this.gameObject);
    }

}
