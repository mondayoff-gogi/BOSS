using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boss2_NormalAttack_Destroy : MonoBehaviour
{
    public GameObject destroy_effect;
    private GameObject destroy_temp;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (collision.tag == "PlayerAttack")
        {
            destroy_temp = Instantiate(destroy_effect, this.transform.position, Quaternion.identity);
            Destroy(destroy_temp, 0.5f);
            Destroy(this.gameObject);
        }


    }
}
