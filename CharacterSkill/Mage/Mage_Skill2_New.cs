using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mage_Skill2_New : MonoBehaviour
{
    public GameObject fire_shield;
    private GameObject fire_shield_temp;
    private GameObject player_obj;
    // Start is called before the first frame update
    private void Start()
    {
        Destroy(this.gameObject, 0.7f);
        Debug.Log("skill 2 intantiate");
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            fire_shield_temp = Instantiate(fire_shield, collision.transform);
            fire_shield_temp.GetComponent<MageSkill2>().GetTargetPlayer(player_obj);
            Debug.Log("me!!");
        }
        if(collision.CompareTag("OtherPlayer"))
        {
            fire_shield_temp = Instantiate(fire_shield, collision.transform);
            Debug.Log("Other player effect");
            //fire_shield_temp.GetComponent<MageSkill2>().GetTargetPlayer(player_obj);
        }
    }

    public void GetPlayer(GameObject player)
    {
        player_obj = player;
    }
}
